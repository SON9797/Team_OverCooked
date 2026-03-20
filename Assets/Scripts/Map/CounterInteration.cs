using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum ItemType//임시
{
    None = 0,
    Plate = 1 << 0,
    Food = 1 << 1,
    Utensil = 1 << 2
}

public class CounterInteration : MonoBehaviour
{
    public ItemType _acceptableTypes; // 이 테이블(음식나가는 카운터)이 허용하는 아이템 타입을 인스펙터에서 체크(접시+생선/접시+ 새우)

    public Transform anchorPoint;

    [SerializeField] private bool _isOccupied = false; // 현재 테이블에 물건 있없?

    [SerializeField] private ItemType _currentItemType = ItemType.None; // 현재 놓인 아이템의 비트 타입
    private GameObject _storedItem;

    public bool IsOccupied => _isOccupied;

    //_item : 배치할 아이템 오브젝트
    //_incomingType : 배치할 아이템의 비트 타입
    public bool TryPlaceItem(GameObject _item, ItemType _incomingType)
    {
        // 자리확인
        if (_isOccupied)
        {
            return false;
        }

        //테이블에 올라갈 수 있는 타입과 플레이어가 들고있는 아이템 비교
        if ((_acceptableTypes & _incomingType) != 0)
        {
            _storedItem = _item; 
            _currentItemType = _incomingType;
            _isOccupied = true;

            _item.transform.SetParent(this.transform);
            _item.transform.position = anchorPoint.position;

            Destroy(_item, 2.0f);

            return true;
        }

        return false;
    }
    //_releasedType : 꺼내가는 아이템의 타입을 외부로 전달(out)
    public GameObject TakeItem(out ItemType _releasedType)
    {
        _releasedType = _currentItemType;

        if (!_isOccupied) return null;

        GameObject _item = _storedItem;
        _storedItem = null;
        _currentItemType = ItemType.None;
        _isOccupied = false;

        return _item;
    }

}
