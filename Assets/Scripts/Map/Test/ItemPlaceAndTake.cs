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

    public virtual bool PlaceItem(GameObject item)
    {
        _onCounterItem = item;
        item.transform.SetParent(_snapPoint);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        if (item.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true;
        }

        return true;
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
        if (_onCounterItem == null)
        {
            CheckExistingItem();
        }
        if (_onCounterItem == null)
        {
            return null;
        }
            GameObject item = _onCounterItem;
        _onCounterItem = null;
        return item;
    }

    //처음 접시 체크
    private void CheckExistingItem()
    {
        Collider[] colliders = Physics.OverlapSphere(_snapPoint.position, 0.3f);

        foreach (var col in colliders)
        {
            // 자기 자신(조리대)은 무시
            if (col.gameObject == this.gameObject) continue;

            // 접시(Dish)나 재료(Ingredient)를 '자신 또는 부모'에서 찾음
            Dish dish = col.GetComponentInParent<Dish>();
            Ingredient ing = col.GetComponentInParent<Ingredient>();

            if (dish != null || ing != null)
            {
                // 실제 스크립트가 붙은 최상단 오브젝트를 타겟으로 잡음
                _onCounterItem = dish != null ? dish.gameObject : ing.gameObject;

                // 물리 및 부모 설정 강제 동기화
                if (_onCounterItem.TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    rb.isKinematic = true;
                }

                _onCounterItem.transform.SetParent(_snapPoint);
                _onCounterItem.transform.localPosition = Vector3.zero;
                _onCounterItem.transform.localRotation = Quaternion.identity;

                Debug.Log($"{gameObject.name}가 주변에서 {_onCounterItem.name}을 찾아 등록했습니다.");
                break;
            }
        }
    }
}
