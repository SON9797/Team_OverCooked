using JetBrains.Annotations;
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

        [Header("던지기 거리")]
        [SerializeField] private float _throwDistance = 4f;

        [Header("던지기 판정 레이어")]
        [SerializeField] private LayerMask _throwLayer = ~0;

        [Header("바닥 판정 레이어")]
        [SerializeField] private LayerMask _groundLayer = ~0;

        [Header("벽 판정 레이어")]
        [SerializeField] private LayerMask _wallLayer = 0;

        [Header("벽 앞 여유 거리")]
        [SerializeField] private float _wallStopOffset = 0.1f;

        [Header("던지기 연출 시간")]
        [SerializeField] private float _throwDuration = 0.25f;

        [Header("던지기 포물선 높이")]
        [SerializeField] private float _throwArcHeight = 1.2f;

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

        [Header("착지 보정 시작 높이")]
        [SerializeField] private float _landingRayStartHeight = 2f;

        [Header("착지 보정 레이 길이")]
        [SerializeField] private float _landingRayDistance = 10f;

        private GameObject _currentHeldObject;
        private Ingredient _currentIngredient;
        private Rigidbody _currentHeldRb;
        private Collider[] _currentHeldCols;
        private InGameInputInjector _inputInjector;
        private PlayerAnimationController _animationController;

        private bool _isThrowing = false;

        public bool HasIngredient => _currentHeldObject != null;
        public bool CanThrowHeldObject => _currentHeldObject != null && _currentHeldObject.GetComponent<Ingredient>() != null;

        private void Awake()
        {
            _inputInjector = GetComponent<InGameInputInjector>();
            _animationController = GetComponent<PlayerAnimationController>();
        }


        public void TryInteractionIngredient()
        {
            if (_inputInjector != null && !_inputInjector.IsSelected)
            {
                return;
            }

            if (_isThrowing)
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

            if (_isThrowing)
            {
                return;
            }

            // 손에 아이템 들고 있으면 칼질 막기
            if (HasIngredient)
            {
                return;
            }

            if (_rayPoint == null)
            {
                return;
            }

            Transform target = FindClosestInteractTarget();
            if (target == null)
            {
                return;
            }

            // 아이템박스테스트 - 도마에 칼질 상호작용 전달
            ChopBoard chopBoard = target.GetComponentInParent<ChopBoard>();
            if (chopBoard != null)
            {
                _animationController?.SetChopping(true);
                chopBoard.ToggleChop();
            }
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

            throwObject.transform.SetParent(null);
            ClearCurrentHeldObject();

            // 아이템던지기 - 던질 때 미리 목표를 확정하지 않고 실제 비행 후 처리
            StartCoroutine(CoThrowObject(throwObject, throwRb, throwCols));
        }

        public bool CanReceiveThrownItem()
        {
            return !HasIngredient && !_isThrowing;
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


        private IEnumerator CoThrowObject(GameObject throwObject, Rigidbody throwRb, Collider[] throwCols)
        {
            if (throwObject == null || _holdPoint == null)
            {
                yield break;
            }

            _isThrowing = true;

            PrepareThrownObject(throwRb, throwCols);

            Vector3 startPos = _holdPoint.position;
            Vector3 forward = transform.forward;
            forward.y = 0f;

            if (forward.sqrMagnitude <= 0.0001f)
            {
                forward = Vector3.forward;
            }

            forward.Normalize();

            Vector3 endPos = GetThrowFloorPosition(startPos, forward);

            float duration = Mathf.Max(0.01f, _throwDuration);
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                Vector3 pos = Vector3.Lerp(startPos, endPos, t);
                pos.y += Mathf.Sin(t * Mathf.PI) * _throwArcHeight;

                throwObject.transform.position = pos;

                // 던져진 아이템을 바라보는 조건은
                // 손에 들고있는 아이템이 없고, 던져지고있는 아이템이 일정 거리내로 들어올때
                if (elapsed <= _catchWindow)
                {
                    TryNotifyPlayersToFaceThrow(pos);

                    PlayerItemController catchPlayer = FindCatchPlayer(pos);
                    if (catchPlayer != null)
                    {
                        catchPlayer.SetCurrentHeldObject(throwObject);
                        _isThrowing = false;
                        yield break;
                    }
                }

                yield return null;
            }

            throwObject.transform.position = endPos;

            ResolveLandingInteraction(throwObject, throwRb, throwCols, endPos);

            _isThrowing = false;
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

        private Vector3 GetThrowFloorPosition(Vector3 origin, Vector3 forward)
        {
            float finalDistance = _throwDistance;

            // 던지는 방향 앞에 벽이 있으면 벽 바로 앞까지만 던짐
            Ray wallRay = new Ray(origin, forward);
            if (Physics.Raycast(wallRay, out RaycastHit wallHit, _throwDistance, _wallLayer))
            {
                finalDistance = Mathf.Max(0f, wallHit.distance - _wallStopOffset);
            }

            Vector3 target = origin + forward * finalDistance;

            // 바닥은 _groundLayer만 맞게 해서 테이블 상판을 바닥으로 잘못 잡지 않게 함
            Ray ray = new Ray(target + Vector3.up * _landingRayStartHeight, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, _landingRayDistance, _groundLayer))
            {
                return hit.point;
            }

            target.y = transform.position.y;
            return target;
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
                throwRb.isKinematic = true;
            }

            SetColliderEnabled(throwCols, false);
        }

        private void ReleaseObjectToFloor(GameObject throwObject, Rigidbody throwRb, Collider[] throwCols, Vector3 targetPos)
        {
            if (throwObject == null)
            {
                return;
            }

            throwObject.transform.SetParent(null);
            throwObject.transform.position = targetPos;

            if (throwRb != null)
            {
                throwRb.isKinematic = false;
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
                _currentHeldRb.velocity = Vector3.zero;
                _currentHeldRb.angularVelocity = Vector3.zero;
            }

            SetHeldColliderEnabled(true);
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
                Gizmos.DrawLine(_holdPoint.position, _holdPoint.position + forward * _throwDistance);
                Gizmos.DrawWireSphere(_holdPoint.position + forward * _throwDistance, 0.15f);
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