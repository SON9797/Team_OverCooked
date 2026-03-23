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

    void Update()
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
        if (_inHandItem != null)
        {
            if (rayHit)
            {
                ItemPlaceAndTake counter = hit.transform.GetComponent<ItemPlaceAndTake>();
                if (counter != null && counter.CanPlaceItem())
                {
                    counter.PlaceItem(_inHandItem);
                    _inHandItem = null; // 손을 비움
                    return;
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

    void PickFromCounter(ItemPlaceAndTake counter)
    {
        _inHandItem = counter.TakeItem();
        _inHandItem.transform.SetParent(_holdPoint);
        _inHandItem.transform.localPosition = Vector3.zero;
        _inHandItem.transform.localRotation = Quaternion.identity;
    }

    void TakeFromBox(IngredientSource source)
    {
        GameObject newIng = source.SpawnIngredient();
        if (newIng != null)
        {
            // 플레이어 손으로 위치 이동 및 부모 설정
            _inHandItem = newIng;
            newIng.transform.position = _holdPoint.position;
            newIng.transform.SetParent(_holdPoint);


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
