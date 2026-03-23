using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPlaceAndTake : MonoBehaviour
{
    //조리대에 적용하는 코드
    [SerializeField] private Transform _snapPoint; // 재료 위치
    protected GameObject _onCounterItem; // 현재 조리대에 놓인 아이템

    public bool CanPlaceItem() => _onCounterItem == null;

    public void PlaceItem(GameObject item)
    {
        _onCounterItem = item;
        item.transform.SetParent(_snapPoint);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

    }

    //다시 집어갈 때
    public virtual GameObject TakeItem()
    {
        GameObject item = _onCounterItem;
        _onCounterItem = null;
        return item;
    }
}
