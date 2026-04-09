using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DishWash : ItemPlaceAndTake
{
    [SerializeField] private float _washTime = 3f;
    [SerializeField] private GameObject _cleanPlatePrefab;
    [SerializeField] private GameObject[] _inDishWasher;
    private DirtyPlateSpawn _dirtyPlateSpawn;
    private bool _isWashing = false;

    [SerializeField] private GameObject _canvasObj;
    protected override void Start()
    {
        if (_canvasObj != null)
        {
            _canvasObj.SetActive(false);
        }

        for (int i = 0;  i < _inDishWasher.Length; i++)
        {
            _inDishWasher[i].SetActive(false);
        }
        _dirtyPlateSpawn = FindObjectOfType<DirtyPlateSpawn>();
    }

    public override bool PlaceItem(GameObject item)
    {
        if (item.CompareTag("Dirty"))
        {
            if (!_isWashing)
            {
                StartCoroutine(WashRoutine(item));
                return true;
            }
            return false;
        }
        return false;
    }

    IEnumerator WashRoutine(GameObject dirtyPlate)
    {
        _isWashing = true;

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

        GameObject newPlate = Instantiate(_cleanPlatePrefab, _snapPoint.position, Quaternion.identity);

        for (int i = 0; i < _inDishWasher.Length; i++)
        {
            _inDishWasher[i].SetActive(false);
        }

        _canvasObj.SetActive(false);

        if (_dirtyPlateSpawn != null)
        {
            _dirtyPlateSpawn._spawnedPlate.Add(newPlate);
        }

        newPlate.transform.SetParent(_snapPoint);
        newPlate.transform.localPosition = Vector3.zero;
        newPlate.transform.localRotation = Quaternion.identity;

        _onCounterItem = newPlate;
        _isWashing = false;
    }

    public override bool CanPlaceItem() => !_isWashing;
}
