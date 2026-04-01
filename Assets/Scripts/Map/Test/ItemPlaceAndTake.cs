using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPlaceAndTake : MonoBehaviour
{
    //조리대에 적용하는 코드
    public Transform _snapPoint; // 재료 위치
    protected GameObject _onCounterItem; // 현재 조리대에 놓인 아이템

    public bool HasItem => _onCounterItem != null;

    protected virtual void Start()
    {
        // 이미 에디터에서 할당했다면 통과, 비어있다면 주변 탐색
        if (_onCounterItem == null)
        {
            CheckExistingItem();
        }
    }

    protected virtual void Update()
    {
        // 만약 테이블에 이미 다른 재료가 올라가있다면 그냥 얹혀있음.
        // 그 테이블에서 재료를 빼면 위에 얹혀있던 재료가 자동으로 테이블에 들어감.
        if (_onCounterItem == null)
        {
            TryAbsorbFloatingItem();
        }
    }

    public virtual bool CanPlaceItem()
    {
        // 상자가 열려있어도 위에 아이템이 없으면 올려놓을 수 있도록 변경
        return _onCounterItem == null;
    }

    public virtual bool PlaceItem(GameObject item)
    {
        if (HasItem || item == null || _snapPoint == null)
        {
            return false;
        }

        _onCounterItem = item;

        if (item.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        //  부모 설정 (Hierarchy에서 상자 밑으로 들어감)
        item.transform.SetParent(_snapPoint);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        // 레이어 분리 (매우 중요)
        // 상자 본체: 플레이어 레이를 통과시킴
        this.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        // 아이템: 플레이어 레이에 맞도록 설정
        item.gameObject.layer = LayerMask.NameToLayer("Default");

        foreach (var col in item.GetComponentsInChildren<Collider>())
        {
            col.enabled = true;
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

        // 찾았다면 true, 없으면 false 
        return dish != null;
    }

    //다시 집어갈 때
    public virtual GameObject TakeItem()
    {
        if (_onCounterItem == null)
        {
            return null;
        }

        GameObject item = _onCounterItem;
        _onCounterItem = null;

        // 1. 부모 관계 해제 (이제 플레이어가 가져갈 것이므로)
        item.transform.SetParent(null);

        if (item.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // 2. 상자의 레이어를 다시 Default로 복구 (그래야 나중에 다시 아이템을 놓을 수 있음)
        this.gameObject.layer = LayerMask.NameToLayer("Default");

        return item;
    }

    private void CheckExistingItem()
    {
        if (_snapPoint == null)
        {
            return;
        }

        Collider[] colliders = Physics.OverlapSphere(_snapPoint.position, 0.3f);

        foreach (var col in colliders)
        {
            if (col.gameObject == this.gameObject)
            {
                continue;
            }

            Dish dish = col.GetComponentInParent<Dish>();
            Ingredient ing = col.GetComponentInParent<Ingredient>();

            // 물리 및 부모 설정 강제 동기화
            if (dish != null || ing != null)
            {
                GameObject target = dish != null ? dish.gameObject : ing.gameObject;

                // 이미 다른 곳의 자식이라면 무시하거나 새로 설정
                PlaceItem(target);
                break;
            }
        }
    }

    
    protected virtual void TryAbsorbFloatingItem()
    {
        if (_snapPoint == null || _onCounterItem != null)
        {
            return;
        }

        Collider[] colliders = Physics.OverlapSphere(_snapPoint.position, 0.45f);

        GameObject closestItem = null;
        float closestSqrDistance = float.MaxValue;

        foreach (var col in colliders)
        {
            if (col == null)
            {
                continue;
            }

            if (col.gameObject == this.gameObject)
            {
                continue;
            }

            Dish dish = col.GetComponentInParent<Dish>();
            Ingredient ing = col.GetComponentInParent<Ingredient>();

            if (dish == null && ing == null)
            {
                continue;
            }

            GameObject target = dish != null ? dish.gameObject : ing.gameObject;

            if (target == _onCounterItem)
            {
                continue;
            }

            float sqrDistance = (target.transform.position - _snapPoint.position).sqrMagnitude;
            if (sqrDistance < closestSqrDistance)
            {
                closestSqrDistance = sqrDistance;
                closestItem = target;
            }
        }

        if (closestItem != null)
        {
            PlaceItem(closestItem);
        }
    }
}
