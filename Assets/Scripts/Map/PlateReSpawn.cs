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

    private List<GameObject> _checkedOutPlates = new List<GameObject>();

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
            _spawnedPlate.Add(newItem);

            Collider[] colliders = Physics.OverlapSphere(spawnPosition, 0.5f);
            foreach (var col in colliders)
            {
                ItemPlaceAndTake counter = col.GetComponentInParent<ItemPlaceAndTake>();
                if (counter != null)
                {
                    counter.PlaceItem(newItem);
                    Debug.Log($"{newItem.name}이(가) {counter.gameObject.name}에 자동으로 등록되었습니다.");
                    break;
                }
            }
        }
    }

    void SpawnStackedItem()
    {
        float currentYOffset = _spawnedPlate.Count * _heightInterval;
        Vector3 spawnPosition = transform.position + new Vector3(0, currentYOffset, 0);

        GameObject newItem = _factory.Create(spawnPosition);

        newItem.transform.SetParent(_snapPoint);
        newItem.transform.position = spawnPosition;
        newItem.transform.localRotation = Quaternion.identity;
        if (newItem.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true;
        }

        _spawnedPlate.Add(newItem);
    }

    IEnumerator RespawnRoutine()
    {
        _isRespawning = true;

        // [추가] 대기 시간 동안 Update가 중복 실행되지 않도록 
        // 즉시 루프를 돌거나 대기 시간을 조금 조절하는 것이 좋습니다.
        yield return new WaitForSeconds(_respawnTime);

        int totalPlates = _spawnedPlate.Count + _checkedOutPlates.Count;
        while (totalPlates < _maxPlate)
        {
            SpawnStackedItem();
            // SpawnStackedItem 안에서 _spawnedPlate.Add를 하므로 개수를 갱신해줘야 합니다.
            totalPlates = _spawnedPlate.Count + _checkedOutPlates.Count;

            // 한꺼번에 생기지 않고 하나씩 생기게 하고 싶다면 여기도 yield를 넣으세요.
            yield return new WaitForSeconds(0.1f);
        }

        _isRespawning = false;
    }

    public void OnPlateDestroyed(GameObject plate)
    {
        _checkedOutPlates.Remove(plate);
        // plate가 제거되면 totalPlates가 줄어들어 Update에서 자동으로 리스폰 트리거
    }

    public override GameObject TakeItem()
    {
        GameObject topPlate = GetTopPlate();
        if (topPlate != null)
        {
            topPlate.transform.SetParent(null);

            _checkedOutPlates.Add(topPlate);

        }
        return topPlate;
    }

    public GameObject GetTopPlate()
    {
        if (_spawnedPlate.Count == 0)
        {
            Debug.Log("가져갈 접시가 없습니다!");
            return null;
        }

        int lastIndex = _spawnedPlate.Count - 1;
        GameObject topPlate = _spawnedPlate[lastIndex];
        _spawnedPlate.RemoveAt(lastIndex);
        return topPlate;
    }

    public override bool CanPlaceItem() => false;
}
