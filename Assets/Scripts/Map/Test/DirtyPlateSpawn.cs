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
    [SerializeField] private float _heightInterval = 0.2f; 
    [SerializeField] private Vector3[] _plates;
    [SerializeField] private PlateReSpawn _plateReSpawn;

    [Inject] PlateFactory _factory;
    [Inject] DirtyPlateFactory _dirtyFactory;

    public List<GameObject> _spawnedPlate = new List<GameObject>();
    public List<GameObject> _spawnedDirtyPlates = new List<GameObject>();
    public List<GameObject> _checkedOutDirtyPlates = new List<GameObject>();

    private bool _isRespawning = false;

    protected override void Start()
    {
        StartItemSpawn();


    }
    protected new void Update()
    {
        _spawnedPlate.RemoveAll(item => item == null);

        _spawnedDirtyPlates.RemoveAll(item => item == null);

        int totalCount = _spawnedPlate.Count + _spawnedDirtyPlates.Count + _checkedOutDirtyPlates.Count;


        if (totalCount <= _maxPlate && !_isRespawning)
        {
            _isRespawning = true;
            StartCoroutine(RespawnRoutine());
        }

    }

    public void StartItemSpawn()
    {
        for (int i = 0; i < _maxPlate; i++)
        {
            Vector3 spawnPosition = _plates[i];

            GameObject newItem = _factory.Create(spawnPosition);

            _spawnedPlate.Add(newItem);

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
                _onCounterItem = newItem;
            }
        }
    }

    IEnumerator RespawnRoutine()
    {
        _isRespawning = true;

        yield return new WaitForSeconds(_respawnTime);

        _spawnedDirtyPlates.RemoveAll(item => item == null);
        _spawnedPlate.RemoveAll(item => item == null);

        int totalCount = _spawnedPlate.Count + _spawnedDirtyPlates.Count + _checkedOutDirtyPlates.Count;

        if (totalCount < _maxPlate)
        {
            SpawnStackedItem();
        }

        _isRespawning = false;
    }

    private void SpawnStackedItem()
    {
        float currentYOffset = _spawnedDirtyPlates.Count * _heightInterval;
        Vector3 spawnPosition = _snapPoint.position + new Vector3(0, currentYOffset, 0);

        GameObject newItem = _dirtyFactory.Create(spawnPosition);

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

        _checkedOutDirtyPlates.Add(topPlate);


        topPlate.transform.SetParent(null);

        if (_spawnedDirtyPlates.Count > 0)
            _onCounterItem = _spawnedDirtyPlates[_spawnedDirtyPlates.Count - 1];
        else
            _onCounterItem = null;

        return topPlate;
    }

    public override bool CanPlaceItem() => false;

}