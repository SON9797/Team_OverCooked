using JetBrains.Annotations;
using Overcooked.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        private GameObject _currentHeldObject;
        private Ingredient _currentIngredient;
        private Rigidbody _currentHeldRb;
        private Collider[] _currentHeldCols;
        private InGameInputInjector _inputInjector;
        private PlayerAnimationController _animationController;

        private Collider[] _playerCols;

        private bool _isThrowing = false;
        private bool _isThrowAiming = false;

        // 현재 던져진 아이템 추적용
        private GameObject _activeThrownObject;
        private Rigidbody _activeThrownRb;
        private Collider[] _activeThrownCols;
        private int _currentWallBounceCount = 0;

        public bool HasIngredient => _currentHeldObject != null;
        public bool CanThrowHeldObject => _currentHeldObject != null && _currentHeldObject.GetComponent<Ingredient>() != null;
        public bool IsThrowAiming => _isThrowAiming;

        private void Awake()
        {
            _inputInjector = GetComponent<InGameInputInjector>();
            _animationController = GetComponent<PlayerAnimationController>();
            _playerCols = GetComponentsInChildren<Collider>();
        }


        public void TryInteractionIngredient()
        {
            if (_inputInjector != null && !_inputInjector.IsSelected)
            {
                return;
            }

            if (_isThrowing || _isThrowAiming)
            {
                return;
            }

            if (_rayPoint == null || _holdPoint == null)
            {
                return;
            }

            Transform target = FindClosestInteractTarget();

            if (_currentHeldObject == null)
            {
                if (target == null)
                {
                    return;
                }


                PlateReSpawn respawn = target.GetComponentInParent<PlateReSpawn>();
                if (respawn != null && respawn.HasItem)
                {
                    TryPickUpFromCounter(respawn); // 여기서 TakeItem()이 실행됨
                    return;
                }

                // 아이템박스테스트 - 조리대 위 아이템 집기
                ItemPlaceAndTake counter = target.GetComponentInParent<ItemPlaceAndTake>();
                if (counter != null && counter.HasItem)
                {
                    // 상자 위에 아이템이 있다면 무조건 '카운터에서 집기' 로직 실행
                    TryPickUpFromCounter(counter);
                    return;
                }

                // 아이템박스테스트 - 박스에서 아이템 꺼내기
                IngredientSource source = target.GetComponentInParent<IngredientSource>();
                if (source != null)
                {
                    TryPickUpIngredientFromSource(source);
                    return;
                }

                // 바닥이나 월드에 놓인 재료/접시 직접 줍기
                TryPickUpDirectObject(target);
            }
            else
            {
                if (target != null)
                {
                    ItemPlaceAndTake counter = target.GetComponentInParent<ItemPlaceAndTake>();
                    if (counter != null)
                    {
                        // 아이템박스테스트 - 조리대 위 접시에 재료 담기
                        if (counter.HasDish(out Dish dishOnCounter))
                        {
                            if (_currentIngredient != null && dishOnCounter.AddIngredient(_currentIngredient))
                            {
                                _currentHeldObject.transform.SetParent(null);
                                ClearCurrentHeldObject();
                                return;
                            }
                        }

                        // 아이템박스테스트 - 조리대 빈칸에 들고 있는 아이템 올려놓기
                        if (counter.CanPlaceItem())
                        {
                            TryPlaceHeldObject(counter);
                            return;
                        }

                        //추가
                        Debug.Log("조리대가 꽉 찼거나 상호작용 불가 상태입니다.");
                        return;
                    }
                }

                TryDropHeldObject();
            }
        }

        public void TryInteractionCook()
        {
            if (_inputInjector != null && !_inputInjector.IsSelected)
            {
                return;
            }

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

            // 아이템박스테스트 - 도마에 칼질 상호작용 전달
            ChopBoard chopBoard = target.GetComponentInParent<ChopBoard>();
            if (chopBoard != null)
            {
                //사운드 - 칼질
                bool isNowChopping = chopBoard.ToggleChop(this);
                _animationController?.SetChopping(isNowChopping);
                return;
            }

            _animationController?.SetChopping(false);
        }

        // 아이템던지기 - 컨트롤을 누르는 순간 조준 시작
        public void StartThrowAim()
        {
            if (_inputInjector != null && !_inputInjector.IsSelected)
            {
                return;
            }

            if (_isThrowing)
            {
                return;
            }

            // 아이템던지기 - Ingredient 스크립트가 붙은 것만 던질 수 있음
            if (!CanThrowCurrentHeldObject())
            {
                return;
            }

            _isThrowAiming = true;
        }

        // 아이템던지기 - 조준 중에는 이동 대신 바라보는 방향만 갱신
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

        // 아이템던지기 - 컨트롤을 떼는 순간 실제 물리 투척
        public void ReleaseThrowAimAndThrow()
        {
            if (!_isThrowAiming)
            {
                return;
            }

            _isThrowAiming = false;
            TryThrowHeldObject();
        }

        public void TryThrowHeldObject()
        {
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

            // 아이템던지기 - Ingredient 스크립트가 붙은 것만 던질 수 있음
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

            throwObject.transform.SetParent(null);
            ClearCurrentHeldObject();

            Vector3 forward = transform.forward;
            forward.y = 0f;

            if (forward.sqrMagnitude <= 0.0001f)
            {
                forward = Vector3.forward;
            }

            forward.Normalize();

            // 아이템던지기 - 손 위치에서 바로 겹치지 않게 약간 앞/위에서 시작
            Vector3 startPos = _holdPoint.position
                + forward * _throwStartForwardOffset
                + Vector3.up * _throwStartUpOffset;

            throwObject.transform.position = startPos;
            throwObject.transform.rotation = Quaternion.identity;

            PrepareThrownObject(throwRb, throwCols);

            _activeThrownObject = throwObject;
            _activeThrownRb = throwRb;
            _activeThrownCols = throwCols;

            EnsureThrownRelay(throwObject);

            //사운드 던지기
            if (throwRb != null)
            {
                throwRb.velocity = Vector3.zero;
                throwRb.angularVelocity = Vector3.zero;
                throwRb.AddForce(forward * _throwForce + Vector3.up * _throwUpForce, ForceMode.VelocityChange);
            }

            StartCoroutine(CoIgnorePlayerCollisionTemporarily(throwCols));
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

        // 아이템던지기 - 던져진 아이템이 충돌했을 때 릴레이가 이 함수 호출
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

            // 아이템던지기 - Walls 레이어에 닿았을 때만 살짝 튕김
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

                bool isPickable = t.GetComponentInParent<Ingredient>() != null ||
                                  t.GetComponentInParent<Dish>() != null;

                // 만약 아이템이 아니라 상자라면 기존 로직대로 진행
                bool isBox = t.GetComponentInParent<ItemPlaceAndTake>() != null ||
                             t.GetComponentInParent<IngredientSource>() != null;

                if (!isPickable && !isBox) continue;

                bool isInteractable =
                    t.GetComponentInParent<IngredientSource>() != null ||
                    t.GetComponentInParent<ItemPlaceAndTake>() != null ||
                    t.GetComponentInParent<PlateReSpawn>() != null ||
                    t.GetComponentInParent<ChopBoard>() != null ||
                    t.GetComponentInParent<Ingredient>() != null ||
                    t.GetComponentInParent<Dish>() != null;

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

        // 아이템던지기 - 던진 직후 플레이어 자신의 콜라이더와 잠깐 충돌 무시
        private IEnumerator CoIgnorePlayerCollisionTemporarily(Collider[] throwCols)
        {
            SetIgnorePlayerCollisions(throwCols, true);
            yield return new WaitForSeconds(_ignorePlayerCollisionTime);
            SetIgnorePlayerCollisions(throwCols, false);
        }

        // 아이템던지기 - 던져진 뒤 받기/착지/조리대 상호작용 감시
        private IEnumerator CoWatchThrownObject(GameObject throwObject, Rigidbody throwRb, Collider[] throwCols)
        {
            float elapsed = 0f;
            bool landingResolved = false;

            while (elapsed < _throwWatchDuration)
            {
                elapsed += Time.deltaTime;

                if (throwObject == null)
                {
                    ClearActiveThrownObject();
                    _isThrowing = false;
                    yield break;
                }

                // 던져진 아이템을 바라보는 조건은
                // 손에 들고있는 아이템이 없고, 던져지고있는 아이템이 일정 거리내로 들어올때
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

        // 아이템던지기 - 충돌 릴레이 스크립트 자동 부착
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

                // 손에 들고있는 아이템이 없고, 던져지고있는 아이템이 일정 거리내로 들어올때
                if (otherPlayer.HasIngredient)
                {
                    continue;
                }

                otherPlayer.FaceThrowTarget(throwPos);
            }
        }


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


        private void ResolveLandingInteraction(GameObject throwObject, Rigidbody throwRb, Collider[] throwCols, Vector3 landingPos)
        {
            Collider[] hits = Physics.OverlapSphere(landingPos, _landingCheckRadius, _landingInteractionLayer);

            // 쓰레기통에 던지게되면 그럼 버려짐.
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

            // 던져져서 재료가 멈춘위치가 테이블 위면 테이블과 상호작용.
            ItemPlaceAndTake counter = FindBestLandingCounter(landingPos);
            if (counter != null)
            {
                // 쓰레기통은 위에서 먼저 처리했으므로 제외
                if (counter is TrashCan)
                {
                    return;
                }

                // 만약 테이블에 이미 다른 재료가 올라가있다면 그냥 얹혀있음.
                if (counter.CanPlaceItem())
                {
                    if (throwRb != null)
                    {
                        throwRb.velocity = Vector3.zero;
                        throwRb.angularVelocity = Vector3.zero;
                        throwRb.isKinematic = true;
                    }

                    SetColliderEnabled(throwCols, true);
                    counter.PlaceItem(throwObject);
                    return;
                }
            }

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

        private void PrepareThrownObject(Rigidbody throwRb, Collider[] throwCols)
        {
            if (throwRb != null)
            {
                throwRb.isKinematic = false;
                throwRb.useGravity = true;
            }

            SetColliderEnabled(throwCols, true);
        }

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

            //사운드 - 아이템 들기
            SetCurrentHeldObject(newObject);
        }


        private void TryPickUpFromCounter(ItemPlaceAndTake counter)
        {
            // 아이템박스테스트 - 조리대에서 아이템 가져오기
            GameObject takeObject = counter.TakeItem();
            if (takeObject == null)
            {
                return;
            }

            //사운드 - 아이템 들기
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

            //사운드 - 아이템 들기
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

            // 상자에게 아이템을 놓으라고 명령.
            if (counter.PlaceItem(_currentHeldObject))
            {
                //사운드 - 아이템 놓기

                // 성공시 플레이어 변수 초기화
                ClearCurrentHeldObject();
                Debug.Log($"{counter.name}에 아이템 자식 설정 성공");
            }
            else
            {
                // 실패시 다시 집어듬
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

            SetHeldColliderEnabled(true);

            //사운드 - 아이템 놓기
            ClearCurrentHeldObject();
        }

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

            SetHeldColliderEnabled(false);

            // 아이템박스테스트 - 집은 아이템을 손 위치로 붙이기
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

        // 2. 상자가 플레이어의 레이 시작 지점을 알 수 있게 전달
        public Transform GetRayPoint()
        {
            return _rayPoint;
        }
    }
}