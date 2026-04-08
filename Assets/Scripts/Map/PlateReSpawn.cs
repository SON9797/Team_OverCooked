using Overcooked;
using Overcooked.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class PlateReSpawn : ItemPlaceAndTake
{
    [SerializeField] private int _maxPlate = 4;
    [SerializeField] private float _respawnTime = 0.5f;
    [SerializeField] private float _heightInterval = 0.2f; //쌓이는 접시 높이
    [SerializeField] private Vector3[] _plates; //초기 접시들 위치값

    [Inject] PlateFactory _factory;

    // 현재 쌓여있는 아이템들을 관리할 리스트
    public List<GameObject> _spawnedPlate = new List<GameObject>();

    public List<GameObject> _checkedOutPlates = new List<GameObject>();

    private bool _isRespawning = false;

    private void Start()
    {
        StartItemSpawn();
    }

    private void Update()
    {
        _spawnedPlate.RemoveAll(item => item == null);

        _checkedOutPlates.RemoveAll(item => item == null);

        int totalPlates = _spawnedPlate.Count + _checkedOutPlates.Count;

        if (!_isRespawning && totalPlates < _maxPlate)
        {
            StartCoroutine(RespawnRoutine());
        }
    }
    private void StartItemSpawn()
    {
        for (int i = 0; i < _maxPlate; i++)
        {
            Vector3 spawnPosition = _plates[i];
            GameObject newItem = _factory.Create(spawnPosition);

            Collider[] colliders = Physics.OverlapSphere(spawnPosition, 0.5f);
            bool isPlacedElsewhere = false;

            foreach (var col in colliders)
            {
                if (col.gameObject == this.gameObject) continue;

                ItemPlaceAndTake counter = col.GetComponentInParent<ItemPlaceAndTake>();
                if (counter != null)
                {
                    counter.PlaceItem(newItem);

                    _checkedOutPlates.Add(newItem);
                    isPlacedElsewhere = true;
                    break;
                }
            }

            if (!isPlacedElsewhere)
            {
                _spawnedPlate.Add(newItem);
                _onCounterItem = newItem;
            }
        }
    }


    void SpawnStackedItem()
    {
        float currentYOffset = _spawnedPlate.Count * _heightInterval;
        Vector3 spawnPosition = _snapPoint.position + new Vector3(0, currentYOffset, 0);

        GameObject newItem = _factory.Create(spawnPosition);

        newItem.transform.SetParent(_snapPoint);
        newItem.transform.position = spawnPosition;
        newItem.transform.localRotation = Quaternion.identity;
        if (newItem.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true;
        }

        _spawnedPlate.Add(newItem);
        _onCounterItem = newItem;
    }

    IEnumerator RespawnRoutine()
    {
        _isRespawning = true;

        yield return new WaitForSeconds(_respawnTime);

        int totalPlates = _spawnedPlate.Count + _checkedOutPlates.Count;
        while (totalPlates < _maxPlate)
        {
            SpawnStackedItem();
            totalPlates = _spawnedPlate.Count + _checkedOutPlates.Count;

            yield return new WaitForSeconds(0.1f);
        }

        _isRespawning = false;
    }

    public void OnPlateDestroyed(GameObject plate)
    {
        _checkedOutPlates.Remove(plate);
    }

    public new bool HasItem => _spawnedPlate.Count > 0;

    public override GameObject TakeItem()
    {
        GameObject topPlate = GetTopPlate();

        if (topPlate != null)
        {
            if (topPlate.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.isKinematic = true;
                rb.velocity = Vector3.zero;
            }
            topPlate.transform.SetParent(null);

            if (!_checkedOutPlates.Contains(topPlate))
            {
                _checkedOutPlates.Add(topPlate);
            }

            Debug.Log($"[성공] {topPlate.name}을 손으로 넘겨줍니다.");
            return topPlate;
        }

        return null;
    }

    public GameObject GetTopPlate()
    {
        if (_spawnedPlate.Count == 0) return null;

        int lastIndex = _spawnedPlate.Count - 1;
        GameObject topPlate = _spawnedPlate[lastIndex];
        _spawnedPlate.RemoveAt(lastIndex);

        return topPlate;
    }

    public override bool CanPlaceItem() => false;
}
