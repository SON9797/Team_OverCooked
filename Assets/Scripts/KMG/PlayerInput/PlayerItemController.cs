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

        [Header("던지기 연출 시간")]
        [SerializeField] private float _throwDuration = 0.25f;

        [Header("던지기 포물선 높이")]
        [SerializeField] private float _throwArcHeight = 1.2f;

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

        //수정
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

                //추가
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

            // 아이템던지기 - 플레이어 우선, 그다음 조리대/선반, 마지막은 바닥
            ResolveThrow();
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

        //수정
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

        private void ResolveThrow()
        {
            Vector3 origin = _holdPoint.position;
            Vector3 forward = transform.forward;
            forward.y = 0f;

            if (forward.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            forward.Normalize();

            RaycastHit[] hits = Physics.RaycastAll(origin, forward, _throwDistance, _throwLayer);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            // 아이템던지기 - 경로 내 다른 플레이어가 있으면 자동으로 회전해서 받기
            for (int i = 0; i < hits.Length; i++)
            {
                Transform hitTransform = hits[i].transform;
                if (hitTransform == null)
                {
                    continue;
                }

                PlayerItemController otherPlayer = hitTransform.GetComponentInParent<PlayerItemController>();
                if (otherPlayer != null && otherPlayer != this && otherPlayer.CanReceiveThrownItem())
                {
                    ThrowToPlayer(otherPlayer);
                    return;
                }
            }

            // 아이템던지기 - 경로 내 조리대/선반이 있으면 그곳에 올려두기
            for (int i = 0; i < hits.Length; i++)
            {
                Transform hitTransform = hits[i].transform;
                if (hitTransform == null)
                {
                    continue;
                }

                ItemPlaceAndTake counter = hitTransform.GetComponentInParent<ItemPlaceAndTake>();
                if (counter != null && counter.CanPlaceItem())
                {
                    ThrowToCounter(counter);
                    return;
                }
            }

            // 아이템던지기 - 경로 내 아무것도 없으면 바닥으로 던지기
            ThrowToFloor(GetThrowFloorPosition(origin, forward));
        }

        private Vector3 GetThrowFloorPosition(Vector3 origin, Vector3 forward)
        {
            Vector3 target = origin + forward * _throwDistance;

            Ray ray = new Ray(target + Vector3.up * 2f, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, 10f, _groundLayer))
            {
                return hit.point;
            }

            target.y = transform.position.y;
            return target;
        }

        private bool CanThrowCurrentHeldObject()
        {
            if (_currentHeldObject == null)
            {
                return false;
            }

            return _currentHeldObject.GetComponent<Ingredient>() != null;
        }

        private void ThrowToPlayer(PlayerItemController targetPlayer)
        {
            if (_currentHeldObject == null || targetPlayer == null)
            {
                return;
            }

            GameObject throwObject = _currentHeldObject;

            throwObject.transform.SetParent(null);

            // 아이템던지기 - 받는 플레이어가 날아오는 아이템 방향으로 자동 회전
            targetPlayer.FaceThrowOrigin(transform.position);

            Rigidbody throwRb = _currentHeldRb;
            Collider[] throwCols = _currentHeldCols;

            ClearCurrentHeldObject();
            StartCoroutine(CoThrowToPlayer(throwObject, throwRb, throwCols, targetPlayer));
        }

        private void ThrowToCounter(ItemPlaceAndTake counter)
        {
            if (_currentHeldObject == null || counter == null)
            {
                return;
            }

            GameObject throwObject = _currentHeldObject;
            Rigidbody throwRb = _currentHeldRb;
            Collider[] throwCols = _currentHeldCols;

            throwObject.transform.SetParent(null);
            ClearCurrentHeldObject();

            StartCoroutine(CoThrowToCounter(throwObject, throwRb, throwCols, counter));
        }

        private void ThrowToFloor(Vector3 targetPos)
        {
            if (_currentHeldObject == null)
            {
                return;
            }

            GameObject throwObject = _currentHeldObject;
            Rigidbody throwRb = _currentHeldRb;
            Collider[] throwCols = _currentHeldCols;

            throwObject.transform.SetParent(null);
            ClearCurrentHeldObject();

            StartCoroutine(CoThrowToFloor(throwObject, throwRb, throwCols, targetPos));
        }

        private IEnumerator CoThrowToPlayer(GameObject throwObject, Rigidbody throwRb, Collider[] throwCols, PlayerItemController targetPlayer)
        {
            if (throwObject == null || targetPlayer == null || targetPlayer._holdPoint == null)
            {
                yield break;
            }

            _isThrowing = true;

            PrepareThrownObject(throwRb, throwCols);

            Vector3 startPos = throwObject.transform.position;
            Vector3 endPos = targetPlayer._holdPoint.position;

            yield return StartCoroutine(CoMoveAlongArc(throwObject.transform, startPos, endPos));

            if (targetPlayer != null && targetPlayer.CanReceiveThrownItem())
            {
                targetPlayer.SetCurrentHeldObject(throwObject);
            }
            else
            {
                ReleaseObjectToFloor(throwObject, throwRb, throwCols, endPos);
            }

            _isThrowing = false;
        }

        private IEnumerator CoThrowToCounter(GameObject throwObject, Rigidbody throwRb, Collider[] throwCols, ItemPlaceAndTake counter)
        {
            if (throwObject == null || counter == null)
            {
                yield break;
            }

            _isThrowing = true;

            PrepareThrownObject(throwRb, throwCols);

            Vector3 startPos = throwObject.transform.position;
            Vector3 endPos = counter.transform.position;

            yield return StartCoroutine(CoMoveAlongArc(throwObject.transform, startPos, endPos));

            if (counter != null && counter.CanPlaceItem())
            {
                if (throwRb != null)
                {
                    throwRb.isKinematic = true;
                }

                SetColliderEnabled(throwCols, false);
                counter.PlaceItem(throwObject);
            }
            else
            {
                ReleaseObjectToFloor(throwObject, throwRb, throwCols, endPos);
            }

            _isThrowing = false;
        }

        private IEnumerator CoThrowToFloor(GameObject throwObject, Rigidbody throwRb, Collider[] throwCols, Vector3 targetPos)
        {
            if (throwObject == null)
            {
                yield break;
            }

            _isThrowing = true;

            PrepareThrownObject(throwRb, throwCols);

            Vector3 startPos = throwObject.transform.position;
            Vector3 endPos = targetPos;

            yield return StartCoroutine(CoMoveAlongArc(throwObject.transform, startPos, endPos));

            ReleaseObjectToFloor(throwObject, throwRb, throwCols, endPos);

            _isThrowing = false;
        }

        private IEnumerator CoMoveAlongArc(Transform target, Vector3 startPos, Vector3 endPos)
        {
            if (target == null)
            {
                yield break;
            }

            float duration = Mathf.Max(0.01f, _throwDuration);
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                Vector3 pos = Vector3.Lerp(startPos, endPos, t);
                pos.y += Mathf.Sin(t * Mathf.PI) * _throwArcHeight;

                target.position = pos;
                yield return null;
            }

            target.position = endPos;
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

        //수정
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

        //수정
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
        }

        //추가
        public GameObject GetCurrentHeldObject() => _currentHeldObject;

        public bool IsSelectedPlayer()
        {
            return _inputInjector != null && _inputInjector.IsSelected;
        }

        // 상자가 플레이어의 레이 시작 지점을 알 수 있게 전달
        public Transform GetRayPoint()
        {
            return _rayPoint;
        }
    }
}