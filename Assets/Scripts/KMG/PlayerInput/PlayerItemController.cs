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

        private GameObject _currentHeldObject;
        private Ingredient _currentIngredient;
        private Rigidbody _currentHeldRb;
        private Collider[] _currentHeldCols;
        private InGameInputInjector _inputInjector;
        private PlayerAnimationController _animationController;

        public bool HasIngredient => _currentHeldObject != null;

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

                // 아이템박스테스트 - 박스에서 아이템 꺼내기
                IngredientSource source = target.GetComponentInParent<IngredientSource>();
                if (source != null)
                {
                    TryPickUpIngredientFromSource(source);
                    return;
                }

                // 아이템박스테스트 - 조리대 위 아이템 집기
                ItemPlaceAndTake counter = target.GetComponentInParent<ItemPlaceAndTake>();
                if (counter != null && !counter.CanPlaceItem())
                {
                    TryPickUpFromCounter(counter);
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

                bool isInteractable =
                    t.GetComponentInParent<IngredientSource>() != null ||
                    t.GetComponentInParent<ItemPlaceAndTake>() != null ||
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

            PrepareHeldObjectForPlace();
            counter.PlaceItem(_currentHeldObject);
            ClearCurrentHeldObject();
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
                _currentHeldRb.velocity = Vector3.zero;
                _currentHeldRb.angularVelocity = Vector3.zero;
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
                _currentHeldRb.velocity = Vector3.zero;
                _currentHeldRb.angularVelocity = Vector3.zero;
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
        }
    }
}