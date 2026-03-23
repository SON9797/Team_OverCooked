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

        [Header("상호작용 레이 시작점")]
        [SerializeField] private Transform _rayPoint;

        [Header("레이 상호작용 거리")]
        [SerializeField] private float _interactionDistance = 3f;

        [Header("주변 줍기 반경")]
        [SerializeField] private float _interactRadius = 1.5f;

        [Header("정면 판정 기준")]
        [SerializeField] private float _forwardDot = 0.3f;

        [Header("재료 레이어")]
        [SerializeField] private LayerMask _ingredientLayer;

        [Header("바닥 드랍 거리")]
        [SerializeField] private float _dropDistance = 1f;

        private Ingredient _currentIngredient;
        private Rigidbody _currentIngredientRb;
        private Collider[] _currentIngredientCols;







        public bool HasIngredient => _currentIngredient != null;

        public void TryInteractionIngredient()
        {
            RaycastHit hit;
            bool isHit = false;

            if (_rayPoint != null)
            {
                isHit = Physics.Raycast(
                    _rayPoint.position,
                    _rayPoint.forward,
                    out hit,
                    _interactionDistance
                );
            }
            else
            {
                hit = default;
            }

            if (HasIngredient)
            {
                TryPlaceOrDrop(hit, isHit);
            }
            else
            {
                TryPickOrTake(hit, isHit);
            }
        }

        private void TryPlaceOrDrop(RaycastHit hit, bool isHit)
        {
            if (_currentIngredient == null) return;

            if (isHit)
            {
                ItemPlaceAndTake counter = hit.transform.GetComponentInParent<ItemPlaceAndTake>();
                if (counter != null && counter.CanPlaceItem())
                {
                    PlaceToCounter(counter);
                    return;
                }
            }

            DropToGround();
        }

        private void TryPickOrTake(RaycastHit hit, bool isHit)
        {
            if (isHit)
            {
                IngredientSource source = hit.transform.GetComponentInParent<IngredientSource>();
                if (source != null)
                {
                    TakeFromSource(source);
                    return;
                }

                ItemPlaceAndTake counter = hit.transform.GetComponentInParent<ItemPlaceAndTake>();
                if (counter != null && !counter.CanPlaceItem())
                {
                    TakeFromCounter(counter);
                    return;
                }
            }

            PickNearbyIngredient();
        }

        private void PickNearbyIngredient()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, _interactRadius, _ingredientLayer);

            Ingredient nearestIngredient = null;
            float nearestDistance = float.MaxValue;

            for (int i = 0; i < hits.Length; i++)
            {
                Ingredient ingredient = hits[i].GetComponentInParent<Ingredient>();
                if (ingredient == null) continue;

                Vector3 toIngredient = ingredient.transform.position - transform.position;
                toIngredient.y = 0f;

                if (toIngredient.sqrMagnitude < 0.0001f) continue;

                float dot = Vector3.Dot(transform.forward, toIngredient.normalized);
                if (dot < _forwardDot) continue;

                float sqrDistance = toIngredient.sqrMagnitude;
                if (sqrDistance < nearestDistance)
                {
                    nearestDistance = sqrDistance;
                    nearestIngredient = ingredient;
                }
            }

            if (nearestIngredient == null) return;

            HoldIngredient(nearestIngredient);
        }

        private void TakeFromSource(IngredientSource source)
        {
            GameObject spawnedObject = source.SpawnIngredient();
            if (spawnedObject == null) return;

            Ingredient ingredient = spawnedObject.GetComponent<Ingredient>();
            if (ingredient == null)
            {
                ingredient = spawnedObject.GetComponentInParent<Ingredient>();
            }

            if (ingredient == null) return;

            HoldIngredient(ingredient);
        }

        private void TakeFromCounter(ItemPlaceAndTake counter)
        {
            GameObject itemObject = counter.TakeItem();
            if (itemObject == null) return;

            Ingredient ingredient = itemObject.GetComponent<Ingredient>();
            if (ingredient == null)
            {
                ingredient = itemObject.GetComponentInParent<Ingredient>();
            }

            if (ingredient == null) return;

            HoldIngredient(ingredient);
        }

        private void PlaceToCounter(ItemPlaceAndTake counter)
        {
            if (_currentIngredient == null) return;

            ReleaseIngredientPhysics();
            _currentIngredient.transform.SetParent(null);

            counter.PlaceItem(_currentIngredient.gameObject);

            ClearCurrentIngredient();
        }

        private void DropToGround()
        {
            if (_currentIngredient == null) return;

            _currentIngredient.transform.SetParent(null);
            _currentIngredient.transform.position = transform.position + transform.forward * _dropDistance;

            ReleaseIngredientPhysics();
            ClearCurrentIngredient();
        }

        private void HoldIngredient(Ingredient ingredient)
        {
            _currentIngredient = ingredient;
            _currentIngredientRb = _currentIngredient.GetComponent<Rigidbody>();
            _currentIngredientCols = _currentIngredient.GetComponentsInChildren<Collider>();

            if (_currentIngredientRb != null)
            {
                _currentIngredientRb.velocity = Vector3.zero;
                _currentIngredientRb.angularVelocity = Vector3.zero;
                _currentIngredientRb.isKinematic = true;
            }

            if (_currentIngredientCols != null)
            {
                for (int i = 0; i < _currentIngredientCols.Length; i++)
                {
                    _currentIngredientCols[i].enabled = false;
                }
            }

            _currentIngredient.transform.SetParent(_holdPoint);
            _currentIngredient.transform.localPosition = Vector3.zero;
            _currentIngredient.transform.localRotation = Quaternion.identity;
        }

        private void ReleaseIngredientPhysics()
        {
            if (_currentIngredientRb != null)
            {
                _currentIngredientRb.isKinematic = false;
            }

            if (_currentIngredientCols != null)
            {
                for (int i = 0; i < _currentIngredientCols.Length; i++)
                {
                    _currentIngredientCols[i].enabled = true;
                }
            }
        }

        private void ClearCurrentIngredient()
        {
            _currentIngredient = null;
            _currentIngredientRb = null;
            _currentIngredientCols = null;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _interactRadius);

            if (_rayPoint != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(_rayPoint.position, _rayPoint.position + _rayPoint.forward * _interactionDistance);
            }
        }
    }
}