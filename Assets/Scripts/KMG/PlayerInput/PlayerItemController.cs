using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Overcooked
{
    public class PlayerItemController : MonoBehaviour
    {
        [Header("손 위치")]
        [SerializeField] private Transform _holdPoint;

        [Header("상호작용 반경")]
        [SerializeField] private float _interactRadius = 1.5f;

        [Header("정면 판정 기준")]
        [SerializeField] private float _forwardDot = 0.3f;

        [Header("재료 레이어")]
        [SerializeField] private LayerMask _ingredientLayer;

        [Header("내려놓기 거리")]
        [SerializeField] private float _dropDistance = 1f;

        private Ingredient _currentIngredient;
        private Rigidbody _currentIngredientRb;
        private Collider[] _currentIngredientCols;

        public bool HasIngredient => _currentIngredient != null;

        public void TryInteractionIngredient()
        {
            RaycastHit hit;
            bool rayHit = Physics.Raycast(_holdPoint.position, transform.forward, out hit, _interactRadius);

            if (_currentIngredient == null)
            {
                if (!rayHit)
                {
                    return;
                }

                // 상자에서 꺼내기
                IngredientSource source = hit.transform.GetComponent<IngredientSource>();
                if (source != null)
                {
                    TryPickUpIngredientFromSource(source);
                    return;
                }

                // 조리대에서 집기
                ItemPlaceAndTake counter = hit.transform.GetComponentInParent<ItemPlaceAndTake>();
                if (counter != null && !counter.CanPlaceItem())
                {
                    TryPickUpIngredientFromCounter(counter);
                }
            }
            else
            {
                if (rayHit)
                {
                    ItemPlaceAndTake counter = hit.transform.GetComponentInParent<ItemPlaceAndTake>();
                    if (counter != null && counter.CanPlaceItem())
                    {
                        TryPlaceIngredient(counter);
                        return;
                    }
                }

                TryDropIngredient();
            }
        }

        public void TryInteractionCook()
        {
            RaycastHit hit;
            bool rayHit = Physics.Raycast(_holdPoint.position, transform.forward, out hit, _interactRadius);

            if (!rayHit)
            {
                return;
            }

            ChopBoard chopBoard = hit.transform.GetComponentInParent<ChopBoard>();
            if (chopBoard != null)
            {
                chopBoard.ToggleChop();
            }
        }

        private void TryPickUpIngredientFromSource(IngredientSource source)
        {
            GameObject newIngredientObject = source.SpawnIngredient();
            if (newIngredientObject == null)
            {
                return;
            }

            SetCurrentIngredient(newIngredientObject);
        }

        private void TryPickUpIngredientFromCounter(ItemPlaceAndTake counter)
        {
            GameObject takeIngredientObject = counter.TakeItem();
            if (takeIngredientObject == null)
            {
                return;
            }

            SetCurrentIngredient(takeIngredientObject);
        }

        private void TryPlaceIngredient(ItemPlaceAndTake counter)
        {
            if (_currentIngredient == null)
            {
                return;
            }

            PrepareIngredientForPlace();

            counter.PlaceItem(_currentIngredient.gameObject);
            ClearCurrentIngredient();
        }

        private void TryDropIngredient()
        {
            if (_currentIngredient == null)
            {
                return;
            }

            Vector3 dropPos = transform.position + transform.forward * _dropDistance;
            dropPos.y = _holdPoint.position.y;

            _currentIngredient.transform.SetParent(null);
            _currentIngredient.transform.position = dropPos;

            if (_currentIngredientRb != null)
            {
                _currentIngredientRb.isKinematic = false;
                _currentIngredientRb.velocity = Vector3.zero;
                _currentIngredientRb.angularVelocity = Vector3.zero;
            }

            SetIngredientColliderEnabled(true);

            ClearCurrentIngredient();
        }

        private void SetCurrentIngredient(GameObject ingredientObject)
        {
            Ingredient ingredient = ingredientObject.GetComponent<Ingredient>();
            if (ingredient == null)
            {
                return;
            }

            _currentIngredient = ingredient;
            _currentIngredientRb = ingredientObject.GetComponent<Rigidbody>();
            _currentIngredientCols = ingredientObject.GetComponentsInChildren<Collider>();

            if (_currentIngredientRb != null)
            {
                _currentIngredientRb.isKinematic = true;
                _currentIngredientRb.velocity = Vector3.zero;
                _currentIngredientRb.angularVelocity = Vector3.zero;
            }

            SetIngredientColliderEnabled(false);

            _currentIngredient.transform.SetParent(_holdPoint);
            _currentIngredient.transform.localPosition = Vector3.zero;
            _currentIngredient.transform.localRotation = Quaternion.identity;
        }

        private void PrepareIngredientForPlace()
        {
            if (_currentIngredientRb != null)
            {
                _currentIngredientRb.isKinematic = true;
                _currentIngredientRb.velocity = Vector3.zero;
                _currentIngredientRb.angularVelocity = Vector3.zero;
            }

            SetIngredientColliderEnabled(false);
        }

        private void SetIngredientColliderEnabled(bool isEnabled)
        {
            if (_currentIngredientCols == null)
            {
                return;
            }

            for (int i = 0; i < _currentIngredientCols.Length; i++)
            {
                _currentIngredientCols[i].enabled = isEnabled;
            }
        }

        private void ClearCurrentIngredient()
        {
            _currentIngredient = null;
            _currentIngredientRb = null;
            _currentIngredientCols = null;
        }
    }
}