using JetBrains.Annotations;
using Overcooked.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using OverCooked;

namespace Overcooked
{
    public class PlayerItemController : MonoBehaviour
    {
        [Header("레이 시작 위치")]
        [SerializeField] private Transform _rayPoint;

        [Header("손 위치")]
        [SerializeField] private Transform _holdPoint;

        [Header("상호작용 거리")]
        [SerializeField] private float _interactionDistance = 3f;

        [Header("상호작용 각도(전체)")]
        [SerializeField, Range(1f, 180f)] private float _interactionAngle = 45f;

        [Header("상호작용 대상 레이어")]
        [SerializeField] private LayerMask _interactionLayer = ~0;

        [Header("내려놓기 거리")]
        [SerializeField] private float _dropDistance = 1f;

        [Header("던지기 힘")]
        [SerializeField] private float _throwForce = 8f;

        [Header("던지기 위쪽 힘")]
        [SerializeField] private float _throwUpForce = 1.2f;

        [Header("던지기 시작 전방 보정")]
        [SerializeField] private float _throwStartForwardOffset = 0.45f;

        [Header("던지기 시작 높이 보정")]
        [SerializeField] private float _throwStartUpOffset = 0.1f;

        [Header("조준 회전 속도")]
        [SerializeField] private float _throwAimTurnSpeed = 720f;

        [Header("플레이어 충돌 무시 시간")]
        [SerializeField] private float _ignorePlayerCollisionTime = 0.12f;

        [Header("벽 반사 레이어")]
        [SerializeField] private LayerMask _wallBounceLayer = 0;

        [Header("벽 반사 세기")]
        [SerializeField, Range(0f, 1f)] private float _wallBounceDamping = 0.2f;

        [Header("최대 벽 반사 횟수")]
        [SerializeField] private int _maxWallBounceCount = 1;

        [Header("플레이어 받기 판정 시간")]
        [SerializeField] private float _catchWindow = 0.35f;

        [Header("플레이어 받기 반경")]
        [SerializeField] private float _catchRadius = 1.0f;

        [Header("플레이어 받기 판정 레이어")]
        [SerializeField] private LayerMask _playerCatchLayer = ~0;

        [Header("착지 상호작용 반경")]
        [SerializeField] private float _landingCheckRadius = 0.45f;

        [Header("착지 상호작용 레이어")]
        [SerializeField] private LayerMask _landingInteractionLayer = ~0;

        [Header("던지기 감시 시간")]
        [SerializeField] private float _throwWatchDuration = 2f;

        [Header("착지로 볼 속도 기준")]
        [SerializeField] private float _landingVelocityThreshold = 0.15f;

        // 현재 손에 들고 있는 오브젝트 관련
        private GameObject _currentHeldObject;
        private Ingredient _currentIngredient;
        private Rigidbody _currentHeldRb;
        private Collider[] _currentHeldCols;

        // 같은 플레이어에 붙어있는 다른 컴포넌트들
        private InGameInputInjector _inputInjector;
        private PlayerAnimationController _animationController;
        private InGamePlayerIndicators _playerIndicators;

        // 자기 자신 플레이어의 콜라이더들
        private Collider[] _playerCols;

        // 던지기 상태
        private bool _isThrowing = false;
        private bool _isThrowAiming = false;

        // 현재 던져진 아이템 추적용
        private GameObject _activeThrownObject;
        private Rigidbody _activeThrownRb;
        private Collider[] _activeThrownCols;
        private int _currentWallBounceCount = 0;

        public bool HasIngredient => _currentHeldObject != null;

        // 현재 들고 있는 아이템이 Ingredient일 때만 던질 수 있게 제한
        public bool CanThrowHeldObject => _currentHeldObject != null && _currentHeldObject.GetComponent<Ingredient>() != null;

        public bool IsThrowAiming => _isThrowAiming;

        private IInGameSoundManager _inGameSoundManager;
        private Coroutine _chopSoundCoroutine;

        [Inject]
        public void Construct(IInGameSoundManager inGameSoundManager)
        {
            _inGameSoundManager = inGameSoundManager;
        }

        private void Awake()
        {
            _inputInjector = GetComponent<InGameInputInjector>();
            _animationController = GetComponent<PlayerAnimationController>();
            _playerIndicators = GetComponent<InGamePlayerIndicators>();
            _playerCols = GetComponentsInChildren<Collider>();
        }

        public void TryInteractionIngredient()
        {
            // 현재 선택된 플레이어만 상호작용 가능
            if (_inputInjector != null && !_inputInjector.IsSelected)
            {
                return;
            }

            // 던지는 중 / 조준 중에는 일반 상호작용 막기
            if (_isThrowing || _isThrowAiming)
            {
                return;
            }

            if (_rayPoint == null || _holdPoint == null)
            {
                return;
            }

            Transform target = FindClosestInteractTarget();

            // 빈손 상태
            if (_currentHeldObject == null)
            {
                if (target == null)
                {
                    return;
                }

                // 접시 리스폰 상자에서 집기
                PlateReSpawn respawn = target.GetComponentInParent<PlateReSpawn>();
                if (respawn != null && respawn.HasItem)
                {
                    TryPickUpFromCounter(respawn);
                    return;
                }

                // 조리대 위 아이템 집기
                ItemPlaceAndTake counter = target.GetComponentInParent<ItemPlaceAndTake>();
                if (counter != null && counter.HasItem)
                {
                    TryPickUpFromCounter(counter);
                    return;
                }

                // 박스 열기
                StuffBoxOpen stuffBox = target.GetComponentInParent<StuffBoxOpen>();
                if (stuffBox != null)
                {
                    stuffBox.TryOpenByPlayer(this);
                }

                // 재료 박스에서 생성
                IngredientSource source = target.GetComponentInParent<IngredientSource>();
                if (source != null)
                {
                    TryPickUpIngredientFromSource(source);
                    return;
                }

                // 바닥에 있는 재료/접시 직접 줍기
                TryPickUpDirectObject(target);
            }
            // 손에 뭔가 들고 있는 상태
            else
            {
                if (target != null)
                {
                    ItemPlaceAndTake counter = target.GetComponentInParent<ItemPlaceAndTake>();

                    if (counter != null)
                    {
                        if (_currentHeldObject.TryGetComponent<Cookware>(out Cookware heldCookware))
                        {
                            if (counter.HasDish(out Dish dishOnCounter2))
                            {
                                if (heldCookware.IsCooked && dishOnCounter2.AddCookedRecipe(heldCookware.GetIngredientDataList()))
                                {
                                    heldCookware.GiveFoodToPlate(dishOnCounter2);

                                    return;
                                }
                            }                            
                        }

                        else if (_currentHeldObject.TryGetComponent<Dish>(out Dish heldDish)) 
                        {
                            Cookware counterCookware = target.GetComponentInParent<Cookware>();

                            if (counterCookware != null)
                            {
                                if (counterCookware.IsCooked && heldDish.AddCookedRecipe(counterCookware.GetIngredientDataList()))
                                {
                                    counterCookware.GiveFoodToPlate(heldDish);
                                    return;
                                }
                            }
                        }


                        // 조리대 위 접시에 재료 담기
                        if (counter.HasDish(out Dish dishOnCounter))
                        {
                            if (_currentIngredient != null && dishOnCounter.AddIngredient(_currentIngredient))
                            {
                                _currentHeldObject.transform.SetParent(null);
                                ClearCurrentHeldObject();
                                return;
                            }
                        }

                        // 빈 조리대에 아이템 올려놓기
                        if (counter.CanPlaceItem())
                        {
                            TryPlaceHeldObject(counter);
                            return;
                        }

                        Debug.Log("조리대가 꽉 찼거나 상호작용 불가 상태입니다.");
                        return;
                    }
                }

                // 둘 곳이 없으면 바닥에 내려놓기
                TryDropHeldObject();
            }
        }

        public void TryInteractionCook()
        {
            // 현재 선택된 플레이어만 상호작용 가능
            if (_inputInjector != null && !_inputInjector.IsSelected)
            {
                return;
            }

            // 던지는 중 / 조준 중에는 칼질 막기
            if (_isThrowing || _isThrowAiming)
            {
                return;
            }

            // 손에 아이템 들고 있으면 칼질 막기
            if (HasIngredient)
            {
                _animationController?.SetChopping(false);
                return;
            }

            if (_rayPoint == null)
            {
                _animationController?.SetChopping(false);
                return;
            }

            Transform target = FindClosestInteractTarget();
            if (target == null)
            {
                _animationController?.SetChopping(false);
                return;
            }

            ChopBoard chopBoard = target.GetComponentInParent<ChopBoard>();
            if (chopBoard != null)
            {
                bool isNowChopping = chopBoard.ToggleChop(this);
                _animationController?.SetChopping(isNowChopping);

                if (isNowChopping)
                {
                    if (_chopSoundCoroutine == null)
                    {
                        _chopSoundCoroutine = StartCoroutine(PlayChopSoundLoop());
                    }
                }
                return;
            }

            _animationController?.SetChopping(false);
        }

        public void StopChopSound()
        {
            if (_chopSoundCoroutine != null)
            {
                StopCoroutine(_chopSoundCoroutine);
                _chopSoundCoroutine = null;
                Debug.Log("칼질 사운드 멈춤");
            }
        }
       
        private IEnumerator PlayChopSoundLoop()
        {
            float chopInterval = 0.2f;

            while (true)
            {
                _inGameSoundManager.PlaySFX(SFXType.Chop);
                yield return new WaitForSeconds(chopInterval);
            }
        }

        // 던지기 버튼을 누르는 순간 호출
        public void StartThrowAim()
        {
            // 현재 선택된 플레이어만 가능
            if (_inputInjector != null && !_inputInjector.IsSelected)
            {
                return;
            }

            // 이미 던지는 중이면 조준 시작 불가
            if (_isThrowing)
            {
                return;
            }

            // 던질 수 있는 아이템이 손에 없으면 조준 시작 안 함
            if (!CanThrowCurrentHeldObject())
            {
                return;
            }

            _isThrowAiming = true;

            // 일반 원 -> 방향 표시 원으로 전환
            _playerIndicators?.SetThrowAiming(true);
        }

        // 조준 중에는 입력 방향을 바라보게 함
        public void UpdateThrowAim(Vector2 lookInput)
        {
            if (!_isThrowAiming)
            {
                return;
            }

            if (lookInput.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            Vector3 lookDir = new Vector3(lookInput.x, 0f, lookInput.y);
            if (lookDir.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            Quaternion targetRot = Quaternion.LookRotation(lookDir.normalized, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, _throwAimTurnSpeed * Time.deltaTime);
        }

        // 던지기 버튼을 떼는 순간 호출
        public void ReleaseThrowAimAndThrow()
        {
            if (!_isThrowAiming)
            {
                return;
            }

            _isThrowAiming = false;

            // 손을 떼는 순간 바로 일반 인디케이터로 복귀
            _playerIndicators?.SetThrowAiming(false);

            // 던지기 애니메이션 재생
            _animationController?.PlayThrow();

            // 실제 투척 실행
            TryThrowHeldObject();
        }

        // 외부 상황으로 조준을 취소해야 할 때 호출
        public void CancelThrowAim()
        {
            if (!_isThrowAiming)
            {
                return;
            }

            _isThrowAiming = false;
            _playerIndicators?.SetThrowAiming(false);
        }
               
        public void TryThrowHeldObject()
        {
            // 현재 선택된 플레이어만 가능
            if (_inputInjector != null && !_inputInjector.IsSelected)
            {
                return;
            }

            if (_isThrowing)
            {
                return;
            }

            if (_currentHeldObject == null)
            {
                return;
            }

            // Ingredient만 던지게 제한
            if (!CanThrowCurrentHeldObject())
            {
                return;
            }

            if (_holdPoint == null)
            {
                return;
            }

            GameObject throwObject = _currentHeldObject;
            Rigidbody throwRb = _currentHeldRb;
            Collider[] throwCols = _currentHeldCols;

            _isThrowing = true;
            _isThrowAiming = false;
            _currentWallBounceCount = 0;

            // 손에서 분리
            throwObject.transform.SetParent(null);
            ClearCurrentHeldObject();

            // 플레이어가 바라보는 수평 방향
            Vector3 forward = transform.forward;
            forward.y = 0f;

            if (forward.sqrMagnitude <= 0.0001f)
            {
                forward = Vector3.forward;
            }

            forward.Normalize();

            // 손에서 바로 겹치지 않게 약간 앞/위에서 시작
            Vector3 startPos = _holdPoint.position
                + forward * _throwStartForwardOffset
                + Vector3.up * _throwStartUpOffset;

            throwObject.transform.position = startPos;
            throwObject.transform.rotation = Quaternion.identity;

            // 던져질 수 있는 물리 상태로 전환
            PrepareThrownObject(throwRb, throwCols);

            _activeThrownObject = throwObject;
            _activeThrownRb = throwRb;
            _activeThrownCols = throwCols;

            // 충돌 릴레이 자동 부착
            EnsureThrownRelay(throwObject);

            // 실제 투척 힘 적용
            if (throwRb != null)
            {
                throwRb.velocity = Vector3.zero;
                throwRb.angularVelocity = Vector3.zero;
                throwRb.AddForce(forward * _throwForce + Vector3.up * _throwUpForce, ForceMode.VelocityChange);
            }

            _inGameSoundManager.PlaySFX(SFXType.Throw);

            // 자기 자신과 잠깐 충돌 무시
            StartCoroutine(CoIgnorePlayerCollisionTemporarily(throwCols));

            // 던져진 뒤 받기/착지 처리 감시
            StartCoroutine(CoWatchThrownObject(throwObject, throwRb, throwCols));
        }

        public bool CanReceiveThrownItem()
        {
            return !HasIngredient && !_isThrowing && !_isThrowAiming;
        }

        public void FaceThrowOrigin(Vector3 throwOrigin)
        {
            Vector3 dir = throwOrigin - transform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            transform.forward = dir.normalized;
        }

        public void FaceThrowTarget(Vector3 targetPosition)
        {
            Vector3 dir = targetPosition - transform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            transform.forward = dir.normalized;
        }

        // 던져진 아이템이 충돌했을 때 릴레이가 호출
        public void NotifyThrownObjectCollision(GameObject hitObject, Collision collision)
        {
            if (!_isThrowing)
            {
                return;
            }

            if (hitObject == null || collision == null)
            {
                return;
            }

            if (_activeThrownObject != hitObject || _activeThrownRb == null)
            {
                return;
            }

            // 벽 레이어에 닿았을 때만 반사
            int otherLayerMask = 1 << collision.gameObject.layer;
            if ((_wallBounceLayer.value & otherLayerMask) == 0)
            {
                return;
            }

            if (_currentWallBounceCount >= _maxWallBounceCount)
            {
                return;
            }

            Vector3 currentVelocity = _activeThrownRb.velocity;
            Vector3 flatVelocity = currentVelocity;
            flatVelocity.y = 0f;

            if (flatVelocity.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            ContactPoint contact = collision.GetContact(0);
            Vector3 normal = contact.normal;
            normal.y = 0f;

            if (normal.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            normal.Normalize();

            Vector3 reflectedFlatVelocity = Vector3.Reflect(flatVelocity, normal) * _wallBounceDamping;
            Vector3 finalVelocity = reflectedFlatVelocity;
            finalVelocity.y = Mathf.Max(0f, currentVelocity.y);

            _activeThrownRb.velocity = finalVelocity;
            _currentWallBounceCount++;
        }

        private Transform FindClosestInteractTarget()
        {
            Collider[] hits = Physics.OverlapSphere(_rayPoint.position, _interactionDistance, _interactionLayer);

            Transform bestTarget = null;
            float bestSqrDistance = float.MaxValue;

            float halfAngle = _interactionAngle * 0.5f;
            float minDot = Mathf.Cos(halfAngle * Mathf.Deg2Rad);

            Vector3 origin = _rayPoint.position;
            Vector3 forward = _rayPoint.forward;
            forward.y = 0f;

            if (forward.sqrMagnitude <= 0.0001f)
            {
                return null;
            }

            forward.Normalize();

            for (int i = 0; i < hits.Length; i++)
            {
                Collider col = hits[i];
                if (col == null)
                {
                    continue;
                }

                Transform t = col.transform;

                // 직접 집을 수 있는 재료/접시
                bool isPickable = t.GetComponentInParent<Ingredient>() != null ||
                                  t.GetComponentInParent<Dish>() != null ||
                                  t.GetComponentInParent<Cookware>() != null;

                // 상자/카운터류
                bool isBox = t.GetComponentInParent<ItemPlaceAndTake>() != null ||
                             t.GetComponentInParent<IngredientSource>() != null ||
                             t.GetComponentInParent<StuffBoxOpen>() != null;

                if (!isPickable && !isBox)
                {
                    continue;
                }

                // 실제 상호작용 가능한 대상인지 최종 체크
                bool isInteractable =
                    t.GetComponentInParent<IngredientSource>() != null ||
                    t.GetComponentInParent<ItemPlaceAndTake>() != null ||
                    t.GetComponentInParent<StuffBoxOpen>() != null ||
                    t.GetComponentInParent<PlateReSpawn>() != null ||
                    t.GetComponentInParent<ChopBoard>() != null ||
                    t.GetComponentInParent<Ingredient>() != null ||
                    t.GetComponentInParent<Dish>() != null ||
                    t.GetComponentInParent<Cookware>() != null;

                if (!isInteractable)
                {
                    continue;
                }

                Vector3 closestPoint = col.ClosestPoint(origin);
                Vector3 toTarget = closestPoint - origin;
                toTarget.y = 0f;

                float sqrDistance = toTarget.sqrMagnitude;
                if (sqrDistance <= 0.0001f)
                {
                    continue;
                }

                Vector3 dir = toTarget.normalized;
                float dot = Vector3.Dot(forward, dir);

                if (dot < minDot)
                {
                    continue;
                }

                if (sqrDistance < bestSqrDistance)
                {
                    bestSqrDistance = sqrDistance;
                    bestTarget = t;
                }
            }

            return bestTarget;
        }

        // 던진 직후 자기 자신의 콜라이더와 잠깐 충돌 무시
        private IEnumerator CoIgnorePlayerCollisionTemporarily(Collider[] throwCols)
        {
            SetIgnorePlayerCollisions(throwCols, true);
            yield return new WaitForSeconds(_ignorePlayerCollisionTime);
            SetIgnorePlayerCollisions(throwCols, false);
        }

        // 던져진 뒤 받기/착지/카운터 상호작용 감시
        private IEnumerator CoWatchThrownObject(GameObject throwObject, Rigidbody throwRb, Collider[] throwCols)
        {
            float elapsed = 0f;
            bool landingResolved = false;

            while (elapsed < _throwWatchDuration)
            {
                elapsed += Time.deltaTime;

                // 던져진 오브젝트가 사라졌으면 종료
                if (throwObject == null)
                {
                    ClearActiveThrownObject();
                    _isThrowing = false;
                    yield break;
                }

                // 일정 시간 안에는 다른 플레이어가 받을 수 있음
                if (elapsed <= _catchWindow)
                {
                    TryNotifyPlayersToFaceThrow(throwObject.transform.position);

                    PlayerItemController catchPlayer = FindCatchPlayer(throwObject.transform.position);
                    if (catchPlayer != null)
                    {
                        if (throwRb != null)
                        {
                            throwRb.velocity = Vector3.zero;
                            throwRb.angularVelocity = Vector3.zero;
                            throwRb.isKinematic = true;
                        }

                        SetColliderEnabled(throwCols, false);
                        catchPlayer.SetCurrentHeldObject(throwObject);

                        ClearActiveThrownObject();
                        _isThrowing = false;
                        yield break;
                    }
                }

                // 충분히 느려졌으면 착지 처리
                if (throwRb != null && elapsed > 0.1f)
                {
                    Vector3 flatVelocity = throwRb.velocity;
                    flatVelocity.y = 0f;

                    if (flatVelocity.sqrMagnitude <= _landingVelocityThreshold * _landingVelocityThreshold)
                    {
                        ResolveLandingInteraction(throwObject, throwRb, throwCols, throwObject.transform.position);
                        landingResolved = true;
                        break;
                    }
                }

                yield return null;
            }

            // 감시 시간이 끝났는데 아직 처리 안 되었으면 마지막 착지 처리
            if (!landingResolved && throwObject != null && throwRb != null)
            {
                ResolveLandingInteraction(throwObject, throwRb, throwCols, throwObject.transform.position);
            }

            ClearActiveThrownObject();
            _isThrowing = false;
        }

        private void ClearActiveThrownObject()
        {
            _activeThrownObject = null;
            _activeThrownRb = null;
            _activeThrownCols = null;
            _currentWallBounceCount = 0;
        }

        // 던져진 아이템에 충돌 감지 릴레이 자동 부착
        private void EnsureThrownRelay(GameObject throwObject)
        {
            if (throwObject == null)
            {
                return;
            }

            ThrownItemCollisionRelay relay = throwObject.GetComponent<ThrownItemCollisionRelay>();
            if (relay == null)
            {
                relay = throwObject.AddComponent<ThrownItemCollisionRelay>();
            }

            relay.Initialize(this);
        }

        // 받기 반경 안 플레이어들이 날아오는 아이템을 바라보게 함
        private void TryNotifyPlayersToFaceThrow(Vector3 throwPos)
        {
            Collider[] hits = Physics.OverlapSphere(throwPos, _catchRadius, _playerCatchLayer);

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i] == null)
                {
                    continue;
                }

                PlayerItemController otherPlayer = hits[i].GetComponentInParent<PlayerItemController>();
                if (otherPlayer == null || otherPlayer == this)
                {
                    continue;
                }

                if (otherPlayer.HasIngredient)
                {
                    continue;
                }

                otherPlayer.FaceThrowTarget(throwPos);
            }
        }

        // 받을 수 있는 플레이어가 있는지 탐색
        private PlayerItemController FindCatchPlayer(Vector3 throwPos)
        {
            Collider[] hits = Physics.OverlapSphere(throwPos, _catchRadius, _playerCatchLayer);

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i] == null)
                {
                    continue;
                }

                PlayerItemController otherPlayer = hits[i].GetComponentInParent<PlayerItemController>();
                if (otherPlayer == null || otherPlayer == this)
                {
                    continue;
                }

                if (!otherPlayer.CanReceiveThrownItem())
                {
                    continue;
                }

                otherPlayer.FaceThrowTarget(throwPos);
                return otherPlayer;
            }

            return null;
        }

        // 착지했을 때 쓰레기통/카운터/바닥 중 어디로 처리할지 결정
        private void ResolveLandingInteraction(GameObject throwObject, Rigidbody throwRb, Collider[] throwCols, Vector3 landingPos)
        {
            Collider[] hits = Physics.OverlapSphere(landingPos, _landingCheckRadius, _landingInteractionLayer);

            // 쓰레기통 우선 처리
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i] == null)
                {
                    continue;
                }

                TrashCan trashCan = hits[i].GetComponentInParent<TrashCan>();
                if (trashCan != null)
                {
                    trashCan.PlaceItem(throwObject);
                    return;
                }
            }

            // 카운터 위에 올라갈 수 있는지 확인
            ItemPlaceAndTake counter = FindBestLandingCounter(landingPos);
            if (counter != null)
            {
                if (counter is TrashCan)
                {
                    return;
                }

                if (counter.CanPlaceItem())
                {
                    if (throwRb != null)
                    {
                        if (!throwRb.isKinematic)
                        {
                            throwRb.velocity = Vector3.zero;
                            throwRb.angularVelocity = Vector3.zero;
                        }

                        throwRb.useGravity = false;
                        throwRb.isKinematic = true;
                    }

                    SetColliderEnabled(throwCols, true);
                    counter.PlaceItem(throwObject);
                    return;
                }
            }

            // 어디에도 못 놓으면 바닥으로
            ReleaseObjectToFloor(throwObject, throwRb, throwCols, landingPos);
        }

        private ItemPlaceAndTake FindBestLandingCounter(Vector3 landingPos)
        {
            Collider[] hits = Physics.OverlapSphere(landingPos, _landingCheckRadius, _landingInteractionLayer);

            ItemPlaceAndTake bestCounter = null;
            float bestSqrDistance = float.MaxValue;

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i] == null)
                {
                    continue;
                }

                ItemPlaceAndTake counter = hits[i].GetComponentInParent<ItemPlaceAndTake>();
                if (counter == null)
                {
                    continue;
                }

                Vector3 closestPoint = hits[i].ClosestPoint(landingPos);
                float sqrDistance = (closestPoint - landingPos).sqrMagnitude;

                if (sqrDistance < bestSqrDistance)
                {
                    bestSqrDistance = sqrDistance;
                    bestCounter = counter;
                }
            }

            return bestCounter;
        }

        private bool CanThrowCurrentHeldObject()
        {
            if (_currentHeldObject == null)
            {
                return false;
            }

            return _currentHeldObject.GetComponent<Ingredient>() != null;
        }

        // 던지기 직전 물리 상태로 전환
        private void PrepareThrownObject(Rigidbody throwRb, Collider[] throwCols)
        {
            if (throwRb != null)
            {
                throwRb.isKinematic = false;
                throwRb.useGravity = true;
            }

            SetColliderEnabled(throwCols, true);
        }

        // 바닥에 떨굴 때 처리
        private void ReleaseObjectToFloor(GameObject throwObject, Rigidbody throwRb, Collider[] throwCols, Vector3 targetPos)
        {
            if (throwObject == null)
            {
                return;
            }

            throwObject.transform.SetParent(null);

            targetPos.y += 0.05f;
            throwObject.transform.position = targetPos;

            if (throwRb != null)
            {
                throwRb.isKinematic = false;
                throwRb.useGravity = true;
                throwRb.velocity = Vector3.zero;
                throwRb.angularVelocity = Vector3.zero;
            }

            SetColliderEnabled(throwCols, true);
        }

        private void TryPickUpIngredientFromSource(IngredientSource source)
        {
            GameObject newObject = source.SpawnIngredient();
            if (newObject == null)
            {
                return;
            }

            // 생성된 재료를 손에 들기
            SetCurrentHeldObject(newObject);
        }

        private void TryPickUpFromCounter(ItemPlaceAndTake counter)
        {
            GameObject takeObject = counter.TakeItem();
            if (takeObject == null)
            {
                return;
            }

            SetCurrentHeldObject(takeObject);
            Debug.Log($"{takeObject.name}을(를) 상자에서 다시 집었습니다.");
        }

        private void TryPickUpDirectObject(Transform target)
        {
            GameObject directObject = FindDirectPickableObject(target);
            if (directObject == null)
            {
                return;
            }

            SetCurrentHeldObject(directObject);
        }

        private GameObject FindDirectPickableObject(Transform target)
        {
            if (target == null)
            {
                return null;
            }

            Dish dish = target.GetComponentInParent<Dish>();
            if (dish != null)
            {
                return dish.gameObject;
            }

            Ingredient ingredient = target.GetComponentInParent<Ingredient>();
            if (ingredient != null)
            {
                return ingredient.gameObject;
            }

            Cookware cookware = target.GetComponentInParent<Cookware>();
            if (cookware != null)
            {
                return cookware.gameObject;
            }

            return null;
        }

        private void TryPlaceHeldObject(ItemPlaceAndTake counter)
        {
            if (_currentHeldObject == null)
            {
                return;
            }

            GameObject itemToPlace = _currentHeldObject;

            itemToPlace.transform.SetParent(null);

            if (counter.PlaceItem(_currentHeldObject))
            {
                _inGameSoundManager.PlaySFX(SFXType.ItemDrop);

                ClearCurrentHeldObject();
                Debug.Log($"{counter.name}에 아이템 자식 설정 성공");
            }
            else
            {
                // 실패했으면 다시 손에 복귀
                SetCurrentHeldObject(itemToPlace);
            }
        }

        private void TryDropHeldObject()
        {
            if (_currentHeldObject == null)
            {
                return;
            }

            Vector3 dropPos = transform.position + transform.forward * _dropDistance;
            dropPos.y = _holdPoint.position.y;

            _currentHeldObject.transform.SetParent(null);
            _currentHeldObject.transform.position = dropPos;

            if (_currentHeldRb != null)
            {
                _currentHeldRb.isKinematic = false;
                _currentHeldRb.useGravity = true;
                _currentHeldRb.velocity = Vector3.zero;
                _currentHeldRb.angularVelocity = Vector3.zero;
            }

            _inGameSoundManager.PlaySFX(SFXType.ItemDrop);

            SetHeldColliderEnabled(true);
            ClearCurrentHeldObject();
        }

        // 손에 아이템 들기
        private void SetCurrentHeldObject(GameObject heldObject)
        {
            if (heldObject == null)
            {
                return;
            }

            _currentHeldObject = heldObject;
            _currentIngredient = heldObject.GetComponent<Ingredient>();
            _currentHeldRb = heldObject.GetComponent<Rigidbody>();
            _currentHeldCols = heldObject.GetComponentsInChildren<Collider>();

            if (_currentHeldRb != null)
            {
                if (!_currentHeldRb.isKinematic)
                {
                    _currentHeldRb.velocity = Vector3.zero;
                    _currentHeldRb.angularVelocity = Vector3.zero;
                }

                _currentHeldRb.useGravity = false;
                _currentHeldRb.isKinematic = true;
            }

            // 손에 들고 있을 때는 콜라이더 끔
            SetHeldColliderEnabled(false);

            _inGameSoundManager.PlaySFX(SFXType.ItemPickUp);

            _currentHeldObject.transform.SetParent(_holdPoint);
            _currentHeldObject.transform.localPosition = Vector3.zero;
            _currentHeldObject.transform.localRotation = Quaternion.identity;
        }

        private void PrepareHeldObjectForPlace()
        {
            if (_currentHeldRb != null)
            {
                _currentHeldRb.isKinematic = true;
            }

            SetHeldColliderEnabled(false);
        }

        private void SetHeldColliderEnabled(bool isEnabled)
        {
            if (_currentHeldCols == null)
            {
                return;
            }

            for (int i = 0; i < _currentHeldCols.Length; i++)
            {
                _currentHeldCols[i].enabled = isEnabled;
            }
        }

        private void SetColliderEnabled(Collider[] cols, bool isEnabled)
        {
            if (cols == null)
            {
                return;
            }

            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i] == null)
                {
                    continue;
                }

                cols[i].enabled = isEnabled;
            }
        }

        // 던진 직후 자기 자신과의 충돌 잠깐 무시
        private void SetIgnorePlayerCollisions(Collider[] cols, bool ignore)
        {
            if (cols == null || _playerCols == null)
            {
                return;
            }

            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i] == null)
                {
                    continue;
                }

                for (int j = 0; j < _playerCols.Length; j++)
                {
                    if (_playerCols[j] == null)
                    {
                        continue;
                    }

                    Physics.IgnoreCollision(cols[i], _playerCols[j], ignore);
                }
            }
        }

        private void ClearCurrentHeldObject()
        {
            _currentHeldObject = null;
            _currentIngredient = null;
            _currentHeldRb = null;
            _currentHeldCols = null;
        }

        private void OnDrawGizmosSelected()
        {
            if (_rayPoint == null)
            {
                return;
            }

            Vector3 origin = _rayPoint.position;
            Vector3 forward = _rayPoint.forward;
            forward.y = 0f;

            if (forward.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            forward.Normalize();

            float halfAngle = _interactionAngle * 0.5f;
            Vector3 leftDir = Quaternion.Euler(0f, -halfAngle, 0f) * forward;
            Vector3 rightDir = Quaternion.Euler(0f, halfAngle, 0f) * forward;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(origin, _interactionDistance);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin, origin + forward * _interactionDistance);
            Gizmos.DrawLine(origin, origin + leftDir * _interactionDistance);
            Gizmos.DrawLine(origin, origin + rightDir * _interactionDistance);

            if (_holdPoint != null)
            {
                Gizmos.color = Color.cyan;
                Vector3 throwStart = _holdPoint.position + forward * _throwStartForwardOffset + Vector3.up * _throwStartUpOffset;
                Gizmos.DrawLine(_holdPoint.position, throwStart);
                Gizmos.DrawRay(throwStart, forward * 1.2f);
                Gizmos.DrawWireSphere(throwStart, 0.15f);
            }

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _catchRadius);
        }

        public GameObject GetCurrentHeldObject() => _currentHeldObject;

        public bool IsSelectedPlayer()
        {
            return _inputInjector != null && _inputInjector.IsSelected;
        }

        // 상자가 플레이어 레이 시작 위치를 참조할 때 사용
        public Transform GetRayPoint()
        {
            return _rayPoint;
        }
    }
}