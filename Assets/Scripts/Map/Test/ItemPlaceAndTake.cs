using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPlaceAndTake : MonoBehaviour
{
    //조리대에 적용하는 코드
    public Transform _snapPoint; // 재료 위치
    protected GameObject _onCounterItem; // 현재 조리대에 놓인 아이템


    public virtual bool CanPlaceItem() => _onCounterItem == null;

    private void Start()
    {
        // 이미 에디터에서 할당했다면 통과, 비어있다면 주변 탐색
        if (_onCounterItem == null)
        {
            CheckExistingItem();
        }
    }

    public virtual void PlaceItem(GameObject item)
    {
        _onCounterItem = item;
        item.transform.SetParent(_snapPoint);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        if (item.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true;
        }
    }

    public bool HasDish(out Dish dish)
    {
        dish = null;
        if (_onCounterItem == null)
        {
            return false;
        }
        // 위에 놓인 아이템에서 Dish 컴포넌트를 찾음
        dish = _onCounterItem.GetComponent<Dish>();

        // 찾았다면 true, 없으면 false 반환
        return dish != null;
    }

    //다시 집어갈 때
    public virtual GameObject TakeItem()
    {
        GameObject item = _onCounterItem;
        _onCounterItem = null;
        return item;
    }

    //처음 접시 체크
    private void CheckExistingItem()
    {
        Collider[] colliders = Physics.OverlapSphere(_snapPoint.position, 0.1f);

        foreach (var col in colliders)
        {
            if (col.gameObject == this.gameObject)
            {
                continue;
            }
            // 아이템(재료나 접시)인 경우 변수에 등록
            if (col.GetComponent<Ingredient>() != null || col.GetComponent<Dish>() != null)
            {
                _onCounterItem = col.gameObject;

                // 물리 설정 고정
                if (_onCounterItem.TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    rb.isKinematic = true;
                }

                // 부모 설정 (에디터에서 안 되어 있을 경우를 대비)
                _onCounterItem.transform.SetParent(_snapPoint);
                _onCounterItem.transform.localPosition = Vector3.zero;
                _onCounterItem.transform.localRotation = Quaternion.identity;

                Debug.Log($"{gameObject.name} 위에 이미 {col.name}이(가) 있습니다.");
                break;
            }
        }
    }
}
