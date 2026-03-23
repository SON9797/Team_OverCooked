using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPlaceAndTake : MonoBehaviour
{
    [SerializeField] private Transform _snapPoint;
    private GameObject _onCounterItem;

    public bool CanPlaceItem() => _onCounterItem == null;

    public void PlaceItem(GameObject item)
    {
        if (item == null) return;

        _onCounterItem = item;
        item.transform.SetParent(_snapPoint);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
    }

    public GameObject TakeItem()
    {
        if (_onCounterItem == null) return null;

        GameObject item = _onCounterItem;
        _onCounterItem = null;

        item.transform.SetParent(null);
        return item;
    }
}
