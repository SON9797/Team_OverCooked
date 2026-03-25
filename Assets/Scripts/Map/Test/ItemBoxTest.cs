using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ItemBoxTest : MonoBehaviour
{
    // 플레이어에 적용하는 코드
    [SerializeField] float _interactionDistance = 3f;
    [SerializeField] Transform _rayPoint;
    [SerializeField] Transform _holdPoint;

    private GameObject _inHandItem;

    private void OnDrawGizmos()
    {
        // 레이가 시작되는 지점을 눈으로 확인하기 위해 작은 구체를 그림
        Gizmos.color = Color.blue;
        Vector3 rayStart = _rayPoint.position;
        Gizmos.DrawWireSphere(rayStart, 0.1f);

        // 실제 레이가 나가는 경로를 그림
        Gizmos.color = Color.red;
        Gizmos.DrawLine(rayStart, rayStart + transform.forward * _interactionDistance);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HandleInteraction();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            HandleChopping();
        }
    }



    private void HandleInteraction()
    {
        RaycastHit hit;
        bool rayHit = Physics.Raycast(_rayPoint.position, _rayPoint.forward, out hit, _interactionDistance);

        if (Physics.Raycast(_rayPoint.position, _rayPoint.forward, out hit, _interactionDistance))
        {
            Debug.Log($"레이에 맞은 물체 이름: {hit.transform.name}");

            var target = hit.transform.GetComponentInParent<ItemPlaceAndTake>();
            if (target == null)
            {
                Debug.Log($"{hit.transform.name}에서 스크립트를 찾지 못했습니다!");
            }
            else
            {
                Debug.Log($"스크립트 찾음! 현재 조리대 빈 공간: {target.CanPlaceItem()}");
            }
        }

        if (Physics.Raycast(_rayPoint.position, _rayPoint.forward, out hit, _interactionDistance))
        {
            // 조리대를 먼저 찾음
            ItemPlaceAndTake counter = hit.transform.GetComponentInParent<ItemPlaceAndTake>();

            if (counter != null)
            {
                // 조리대 위에 접시(Dish)가 있는지 확인
                if (counter.HasDish(out Dish dishOnCounter))
                {
                    // 손에 재료가 있다면 접시에 담기 시도
                    if (_inHandItem != null)
                    {
                        Ingredient ing = _inHandItem.GetComponent<Ingredient>();
                        if (ing != null)
                        {
                            // 성공하면 true 반환 및 재료 파괴됨
                            if (dishOnCounter.AddIngredient(ing))
                            {
                                _inHandItem = null; // 손 비우기
                                Debug.Log("접시에 재료를 추가했습니다!");
                                return;
                            }
                        }
                    }
                }
            }
        }

        if (_inHandItem != null)
        {
            if (rayHit)
            {
                ItemPlaceAndTake counter = hit.transform.GetComponent<ItemPlaceAndTake>();
                if (counter != null && _inHandItem != null)
                {
                    //조리대에게 "너 접시 들고 있니?"라고 물어봄
                    if (counter.HasDish(out Dish dishOnCounter))
                    {
                        Ingredient ing = _inHandItem.GetComponent<Ingredient>();
                        // 접시에 담기 시도
                        if (ing != null && dishOnCounter.AddIngredient(ing))
                        {
                            _inHandItem = null;
                            return; // 성공했으면 종료
                        }
                    }

                    // 접시가 없거나 담을 수 없는 재료라면 조리대에 그냥 놓기
                    if (counter.CanPlaceItem())
                    {
                        if (counter.PlaceItem(_inHandItem))
                        {
                            _inHandItem = null;
                        }
                    }
                }
            }
            
        }
        else
        {
            if (rayHit)
            {
                // 상자에서 꺼내기
                IngredientSource source = hit.transform.GetComponent<IngredientSource>();
                if (source != null)
                {
                    TakeFromBox(source);
                    return;
                }

                // 조리대에서 집기
                ItemPlaceAndTake counter = hit.transform.GetComponentInParent<ItemPlaceAndTake>();
                if (counter != null && !counter.CanPlaceItem())
                {
                    PickFromCounter(counter);
                }
            }
        }
    }

    private void PickFromCounter(ItemPlaceAndTake counter)
    {
        if (_inHandItem == null && !counter.CanPlaceItem())
        {
            // 조리대에 있는 걸 집어옴
            _inHandItem = counter.TakeItem();
            _inHandItem.transform.SetParent(_holdPoint);
            _inHandItem.transform.localPosition = Vector3.zero;

            // 이때 Rigidbody가 다시 켜지는지 확인!
            if (_inHandItem.TryGetComponent<Rigidbody>(out Rigidbody rb)) rb.isKinematic = true;
        }
    }

    private void TakeFromBox(IngredientSource source)
    {
        GameObject newIng = source.SpawnIngredient();
        if (newIng != null)
        {
            // 플레이어 손으로 위치 이동 및 부모 설정
            _inHandItem = newIng;
            newIng.transform.SetParent(_holdPoint);
            newIng.transform.position = _holdPoint.position;
            


        }
    }

    private void HandleChopping()
    {
        RaycastHit hit;
        if (Physics.Raycast(_rayPoint.position, _rayPoint.forward, out hit, _interactionDistance))
        {
            ChopBoard chopBoard = hit.transform.GetComponentInParent<ChopBoard>();
            if (chopBoard != null)
            {
                chopBoard.ToggleChop();
            }
        }
    }
}
