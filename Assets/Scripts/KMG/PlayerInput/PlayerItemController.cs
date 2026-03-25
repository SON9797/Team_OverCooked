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

        [Header("내려놓기 거리")]
        [SerializeField] private float _dropDistance = 1f;

        private GameObject _currentHeldObject;
        private Ingredient _currentIngredient;
        private Rigidbody _currentHeldRb;
        private Collider[] _currentHeldCols;
        private InGameInputInjector _inputInjector;

        public bool HasIngredient => _currentHeldObject != null;

        private void Awake()
        {
            _inputInjector = GetComponent<InGameInputInjector>();
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

            RaycastHit hit;
            // 아이템박스테스트 - 레이캐스트로 정면 상호작용 대상 찾기
            bool rayHit = Physics.Raycast(_rayPoint.position, _rayPoint.forward, out hit, _interactionDistance);

            if (_currentHeldObject == null)
            {
                if (!rayHit)
                {
                    return;
                }

                // 아이템박스테스트 - 박스에서 아이템 꺼내기
                IngredientSource source = hit.transform.GetComponent<IngredientSource>();
                if (source != null)
                {
                    TryPickUpIngredientFromSource(source);
                    return;
                }

                // 아이템박스테스트 - 조리대 위 아이템 집기
                ItemPlaceAndTake counter = hit.transform.GetComponentInParent<ItemPlaceAndTake>();
                if (counter != null && !counter.CanPlaceItem())
                {
                    TryPickUpFromCounter(counter);
                }
            }
            else
            {
                if (rayHit)
                {
                    // 아이템박스테스트 - 조리대 상호작용 대상 찾기
                    ItemPlaceAndTake counter = hit.transform.GetComponentInParent<ItemPlaceAndTake>();
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

            if (_rayPoint == null)
            {
                return;
            }

            RaycastHit hit;
            // 아이템박스테스트 - 정면 레이로 도마 찾기
            bool rayHit = Physics.Raycast(_rayPoint.position, _rayPoint.forward, out hit, _interactionDistance);

            if (!rayHit)
            {
                return;
            }

            // 아이템박스테스트 - 도마에 칼질 상호작용 전달
            ChopBoard chopBoard = hit.transform.GetComponentInParent<ChopBoard>();
            if (chopBoard != null)
            {
                chopBoard.ToggleChop();
            }
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
    }
}