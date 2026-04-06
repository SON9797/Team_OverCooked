using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPlaceAndTake : MonoBehaviour
{
    //조리대에 적용하는 코드
    public Transform _snapPoint; // 재료 위치
    protected GameObject _onCounterItem; // 현재 조리대에 놓인 아이템

    public bool HasItem => _onCounterItem != null;

    public virtual bool CanPlaceItem()
    {
        // 상자가 열려있어도 위에 아이템이 없으면 올려놓을 수 있도록 변경
        return true;
    }
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
        if (_onCounterItem == null)
        {
            TryAbsorbFloatingItem();
        }
    }

    public virtual bool PlaceItem(GameObject item)
    {
        if (HasItem)
        {
            // 조리대에 접시(Dish)가 있고, 플레이어가 재료(Ingredient)를 들고 올 때
            if (HasDish(out Dish dishOnCounter))
            {
                if (item.TryGetComponent<Ingredient>(out Ingredient incomingIng))
                {
                    if (dishOnCounter.AddIngredient(incomingIng))
                    {
                        // 합치기 성공: 들어온 아이템은 Dish 내부에서 Destroy됨
                        return true;
                    }
                }
            }
            // 조리대에 재료(Ingredient)가 있고, 플레이어가 접시(Dish)를 들고 올 때
            else if (_onCounterItem.TryGetComponent<Ingredient>(out Ingredient ingOnCounter))
            {
                if (item.TryGetComponent<Dish>(out Dish incomingDish))
                {
                    if (incomingDish.AddIngredient(ingOnCounter))
                    {
                        // 합치기 성공: 조리대에 있던 재료를 접시에 담음
                        // 조리대 아이템 정보를 새로 들어온 접시로 교체
                        _onCounterItem = item;
                        SetupItemTransform(item); // 위치 고정 로직
                        return true;
                    }
                }
            }

            // 합치기가 불가능하면(둘 다 접시거나, 레시피가 아니거나 등) 놓기 실패
            return false;
        }

        // 2. 아이템이 없는 경우 (기존 로직 수행)
        if (item == null || _snapPoint == null) return false;

        _onCounterItem = item;
        SetupItemTransform(item);
        return true;
    }

    // 중복되는 트랜스폼 설정을 별도 함수로 분리
    private void SetupItemTransform(GameObject item)
    {
        item.transform.SetParent(_snapPoint);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            if (!rb.isKinematic)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            rb.useGravity = false;
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

        item.transform.SetParent(null);

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
