using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class DishWash : ItemPlaceAndTake
{
    [SerializeField] private float _washTime = 3f;
    [SerializeField] private GameObject[] _inDishWasher;
    [SerializeField] private GameObject _canvasObj;

    [Inject] private PlateFactory _plateFactory;

    private bool _isWashing = false;

    // 시작 시 설거지 연출 오브젝트를 꺼둠
    protected new void Start()
    {
        if (_canvasObj != null)
        {
            _canvasObj.SetActive(false);
        }

        for (int i = 0; i < _inDishWasher.Length; i++)
        {
            _inDishWasher[i].SetActive(false);
        }
    }

    // 더러운 접시를 올리면 설거지를 시작
    public override bool PlaceItem(GameObject item)
    {
        if (_isWashing)
        {
            return false;
        }

        if (item == null || !item.CompareTag("Dirty"))
        {
            return false;
        }

        if (_onCounterItem != null)
        {
            return false;
        }

        StartCoroutine(WashRoutine(item));
        return true;
    }

    // 일정 시간 후 더러운 접시를 깨끗한 접시로 바꿈
    private IEnumerator WashRoutine(GameObject dirtyPlate)
    {
        _isWashing = true;
        _onCounterItem = dirtyPlate;

        dirtyPlate.SetActive(false);

        if (_canvasObj != null)
        {
            _canvasObj.SetActive(true);
        }

        for (int i = 0; i < _inDishWasher.Length; i++)
        {
            _inDishWasher[i].SetActive(true);
        }

        yield return new WaitForSeconds(_washTime);

        Destroy(dirtyPlate);

        GameObject newPlate = _plateFactory.CreateClean(_snapPoint.position);

        newPlate.transform.SetParent(_snapPoint);
        newPlate.transform.localPosition = Vector3.zero;
        newPlate.transform.localRotation = Quaternion.identity;

        if (newPlate.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        for (int i = 0; i < _inDishWasher.Length; i++)
        {
            _inDishWasher[i].SetActive(false);
        }

        if (_canvasObj != null)
        {
            _canvasObj.SetActive(false);
        }

        _onCounterItem = newPlate;
        _isWashing = false;
    }

    // 설거지 중이 아닐 때만 아이템을 올릴 수 있음
    public override bool CanPlaceItem()
    {
        return !_isWashing && _onCounterItem == null;
    }

    // 설거지 완료된 접시를 가져감
    public override GameObject TakeItem()
    {
        if (_isWashing || _onCounterItem == null)
        {
            return null;
        }

        GameObject item = _onCounterItem;
        _onCounterItem = null;

        item.transform.SetParent(null);

        if (item.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        return item;
    }
}