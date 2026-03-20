using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableInteraction : MonoBehaviour
{
    public ItemType _acceptableTypes;

    private bool _isPlayerNearby = false;
    public Transform anchorPoint;
    [SerializeField] private bool _isOccupied = false;

    [SerializeField] private ItemType _currentItemType = ItemType.None;
    private GameObject _storedItem;

    public bool IsOccupied => _isOccupied;
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    public bool TryPlaceItem(GameObject _item, ItemType _incomingType)
    {
        // 자리확인
        if (_isOccupied)
        {
            return false;
        }

        if ((_acceptableTypes & _incomingType) != 0)
        {
            _storedItem = _item;
            _currentItemType = _incomingType;
            _isOccupied = true;

            _item.transform.SetParent(this.transform);

            return true;
        }

        return false;
    }

    public GameObject TakeItem(out ItemType _releasedType)
    {
        _releasedType = _currentItemType;

        if (!_isOccupied) return null;

        GameObject item = _storedItem;
        _storedItem = null;
        _currentItemType = ItemType.None;
        _isOccupied = false;

        return item;
    }

}
