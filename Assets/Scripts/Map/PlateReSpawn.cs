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

    // 현재 아이템
    public List<GameObject> _spawnedPlates = new List<GameObject>();

    public List<GameObject> _checkedOutPlates = new List<GameObject>();

    private int _activeOutsideCount = 0;

    private bool _isRespawning = false;

    private void Start()
    {
        StartItemSpawn();
    }

    private void Update()
    {
        int currentTotal = _spawnedPlates.Count + _activeOutsideCount;

        if (!_isRespawning && currentTotal < _maxPlate)
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
            foreach (var col in colliders)
            {
                if (col.gameObject == this.gameObject) continue;

                ItemPlaceAndTake counter = col.GetComponentInParent<ItemPlaceAndTake>();
                if (counter != null)
                {
                    counter.PlaceItem(newItem);
                    Debug.Log($"{newItem.name}이(가) {counter.gameObject.name}에 자동으로 등록되었습니다.");
                    break;
                }
            }
            _activeOutsideCount++;
        }
    }
    IEnumerator RespawnRoutine()
    {
        _isRespawning = true;

        yield return new WaitForSeconds(_respawnTime);

        if ((_spawnedPlates.Count + _activeOutsideCount) < _maxPlate)
        {
            SpawnAtStack();
        }

        _isRespawning = false;
    }

    private void SpawnAtStack()
    {
        // 리스폰 지점에 차곡차곡 쌓기
        int stackIndex = _spawnedPlates.Count;
        float yOffset = stackIndex * _heightInterval;

        Vector3 spawnPos = (_snapPoint != null ? _snapPoint.position : transform.position) + new Vector3(0, yOffset, 0);
        GameObject newPlate = _factory.Create(spawnPos);

        newPlate.transform.SetParent(_snapPoint);
        newPlate.transform.localPosition = new Vector3(0, yOffset, 0);
        newPlate.transform.localRotation = Quaternion.identity;

        if (newPlate.TryGetComponent<Rigidbody>(out Rigidbody rb))
            rb.isKinematic = true;

        _spawnedPlates.Add(newPlate);

        _onCounterItem = newPlate;
    }

    public void OnPlateDestroyed(GameObject plate)
    {
        _activeOutsideCount--;
    }

    public override GameObject TakeItem()
    {
        if (_spawnedPlates.Count == 0)
        {
            Debug.LogWarning("스택에 접시가 없습니다!");
            return null;
        }

        // 가장 위에 있는(마지막 인덱스) 접시 가져오기
        int lastIndex = _spawnedPlates.Count - 1;
        GameObject topPlate = _spawnedPlates[lastIndex];

        _spawnedPlates.RemoveAt(lastIndex);
        _activeOutsideCount++;

        topPlate.transform.SetParent(null);
        if (topPlate.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = false; // 집어갈 때는 물리 연산 다시 활성화 (필요 시)
        }
        if (_spawnedPlates.Count > 0)
        {
            _onCounterItem = _spawnedPlates[_spawnedPlates.Count - 1];
        }
        else
        {
            _onCounterItem = null;
        }

        Debug.Log($"접시를 집어갔습니다. 남은 스택: {_spawnedPlates.Count}");
        return topPlate;
    }

    public override bool CanPlaceItem() => false;
}
