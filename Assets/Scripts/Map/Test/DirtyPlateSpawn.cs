using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class DirtyPlateSpawn : ItemPlaceAndTake
{
    [SerializeField] private GameObject _platePrefab;
    [SerializeField] private GameObject _dirtyPlatePrefab;

    [SerializeField] private int _maxPlate = 4;
    [SerializeField] private float _respawnTime = 3f;
    [SerializeField] private float _heightInterval = 0.2f; //쌓이는 접시 높이
    [SerializeField] private Vector3[] _plates; //초기 접시들 위치값

    //public PlateReSpawn _plateReSpawn;

    [Inject] private PlateFactory _factory;

    public List<GameObject> _spawnedPlate = new List<GameObject>();
    public List<GameObject> _spawnedDirtyPlates = new List<GameObject>();

    public List<GameObject> _allActivePlates = new List<GameObject>();

    private int _pendingDirtyPlates = 0;
    private bool _isRespawning = false;


    protected override void Start()
    {
        StartItemSpawn();

    }
    protected new void Update()
    {

        _spawnedDirtyPlates.RemoveAll(item => item == null);

        int totalPlates = _spawnedDirtyPlates.Count;

        if (totalPlates < _maxPlate && !_isRespawning)
        {
            Debug.Log("RespawnRoutine 시작!");

            StartCoroutine(RespawnRoutine());
        }
    }
    public  void RegisterPlate(GameObject plate)
    {
        if (plate != null && !_allActivePlates.Contains(plate))
        {
            _allActivePlates.Add(plate);
        }
    }

    public void StartItemSpawn()
    {
        for (int i = 0; i < _maxPlate; i++)
        {
            Vector3 spawnPosition = _plates[i];
            GameObject newItem = Instantiate(_platePrefab, spawnPosition, Quaternion.identity);

            RegisterPlate(newItem);

            Collider[] colliders = Physics.OverlapSphere(spawnPosition, 0.5f);
            bool isPlacedElsewhere = false;

            foreach (var col in colliders)
            {
                if (col.gameObject == this.gameObject)
                {
                    continue;
                }
                ItemPlaceAndTake counter = col.GetComponentInParent<ItemPlaceAndTake>();
                if (counter != null)
                {
                    counter.PlaceItem(newItem);
                    isPlacedElsewhere = true;
                    break;
                }
            }

            if (!isPlacedElsewhere)
            {
                if (_spawnedPlate != null)
                {
                    _spawnedPlate.Add(newItem);
                }
                _onCounterItem = newItem;
            }
        }
    }

    IEnumerator RespawnRoutine()
    {
        _isRespawning = true;

        Debug.Log("RespawnRoutine 진입");

        yield return new WaitForSeconds(_respawnTime);

        Debug.Log($"대기 완료. _spawnedDirtyPlates.Count: {_spawnedDirtyPlates.Count}");


        if (_spawnedDirtyPlates.Count < _maxPlate)
        {
            Debug.Log("SpawnStackedItem 호출");
            SpawnStackedItem();
        }
        else
        {
            Debug.Log("조건 불충족 - 스폰 안함");
        }

        _isRespawning = false;
    }

    private void SpawnStackedItem()
    {
        Debug.Log($"_snapPoint: {_snapPoint}, _dirtyPlatePrefab: {_dirtyPlatePrefab}");

        float currentYOffset = _spawnedDirtyPlates.Count * _heightInterval;
        Vector3 spawnPosition = _snapPoint.position + new Vector3(0, currentYOffset, 0);

        GameObject newItem = Instantiate(_dirtyPlatePrefab, spawnPosition, Quaternion.identity);
        Debug.Log($"더러운 접시 스폰됨: {newItem.name} at {spawnPosition}");

        SetupDirtyPlate(newItem, spawnPosition);

        _spawnedDirtyPlates.Add(newItem);
        _onCounterItem = newItem;
    }

    private void SetupDirtyPlate(GameObject item, Vector3 pos)
    {
        item.transform.SetParent(_snapPoint);
        item.transform.position = pos;
        item.transform.localRotation = Quaternion.identity;

        if (item.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true;
        }
    }


    public new bool HasItem => _spawnedPlate.Count > 0;

    public override GameObject TakeItem()
    {
        if (_spawnedDirtyPlates.Count == 0) return null;

        int lastIndex = _spawnedDirtyPlates.Count - 1;
        GameObject topPlate = _spawnedDirtyPlates[lastIndex];

        _spawnedDirtyPlates.RemoveAt(lastIndex);

        topPlate.transform.SetParent(null);

        // 다음 번에 잡힐 아이템 갱신
        if (_spawnedDirtyPlates.Count > 0)
            _onCounterItem = _spawnedDirtyPlates[_spawnedDirtyPlates.Count - 1];
        else
            _onCounterItem = null;

        return topPlate;
    }


    public override bool CanPlaceItem() => false;
}