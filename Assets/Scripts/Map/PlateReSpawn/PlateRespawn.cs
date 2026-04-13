using Overcooked.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class PlateRespawn : ItemPlaceAndTake
{
    [Header("공통 설정")]
    [SerializeField] private int _maxPlate = 4;
    [SerializeField] private float _respawnTime = 0.5f;
    [SerializeField] private float _heightInterval = 0.2f;

    [Header("초기 접시 위치들 (항상 깨끗한 접시)")]
    [SerializeField] private Vector3[] _plates;

    [Header("제출 후 반환 타입 (씬별 설정)")]
    [SerializeField] private PlateReturnType _returnPlateType = PlateReturnType.Clean;

    [Inject] private PlateFactory _factory;

    public List<GameObject> _spawnedCleanPlates = new List<GameObject>();
    public List<GameObject> _spawnedDirtyPlates = new List<GameObject>();
    public List<GameObject> _checkedOutPlates = new List<GameObject>();

    private bool _isRespawning = false;

    private IInGameSoundManager _inGameSoundManager;

    [Inject]
    public void Construct(IInGameSoundManager inGameSoundManager)
    {
        _inGameSoundManager = inGameSoundManager;
    }

    // 시작 시 초기 위치들에 깨끗한 접시를 생성
    private new void Start()
    {
        Debug.Log($"[PlateRespawn:{name}] Start / ReturnType = {_returnPlateType}");
        StartInitialSpawn();
    }

    // 파괴된 오브젝트를 리스트에서 정리
    private void Update()
    {
        _spawnedCleanPlates.RemoveAll(item => item == null);
        _spawnedDirtyPlates.RemoveAll(item => item == null);
        _checkedOutPlates.RemoveAll(item => item == null);
    }

    // 시작할 때 초기 위치들에 깨끗한 접시 배치
    private void StartInitialSpawn()
    {
        if (_plates == null || _plates.Length == 0)
        {
            return;
        }

        int spawnCount = Mathf.Min(_maxPlate, _plates.Length);

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 spawnPosition = _plates[i];
            GameObject newItem = _factory.CreateClean(spawnPosition);

            Collider[] colliders = Physics.OverlapSphere(spawnPosition, 0.5f);
            bool isPlacedElsewhere = false;

            foreach (var col in colliders)
            {
                if (col.gameObject == gameObject) continue;
                if (col.gameObject == newItem) continue;

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
                SetupInitialPlate(newItem, spawnPosition);
                _spawnedCleanPlates.Add(newItem);
                _onCounterItem = newItem;
            }
        }
    }

    // 제출 후 일정 시간 뒤 반환 타입에 맞는 접시 1장 생성
    private IEnumerator RespawnRoutine()
    {
        _isRespawning = true;
        Debug.Log($"[PlateRespawn:{name}] RespawnRoutine 시작 / ReturnType = {_returnPlateType}");
        yield return new WaitForSeconds(_respawnTime);

        int totalPlates = _spawnedCleanPlates.Count + _spawnedDirtyPlates.Count + _checkedOutPlates.Count;
        Debug.Log($"[PlateRespawn:{name}] Respawn 직전 / totalPlates = {totalPlates}, maxPlate = {_maxPlate}, ReturnType = {_returnPlateType}");

        if (totalPlates < _maxPlate)
        {
            SpawnStackedItem(_returnPlateType);
        }

        _isRespawning = false;
    }

    // 반환 타입에 맞는 접시를 스택 형태로 생성
    private void SpawnStackedItem(PlateReturnType plateType)
    {
        int stackedCount = _spawnedCleanPlates.Count + _spawnedDirtyPlates.Count;
        float currentYOffset = stackedCount * _heightInterval;
        Vector3 spawnPosition = _snapPoint.position + new Vector3(0f, currentYOffset, 0f);

        Debug.Log($"[PlateRespawn:{name}] SpawnStackedItem / plateType = {plateType} / spawnPosition = {spawnPosition}");

        GameObject newItem = _factory.Create(spawnPosition, plateType);

        Debug.Log($"[PlateRespawn:{name}] 실제 생성 결과 = {(newItem != null ? newItem.name : "NULL")}");

        SetupStackedPlate(newItem, spawnPosition);

        if (plateType == PlateReturnType.Clean)
        {
            _inGameSoundManager.PlaySFX(OverCooked.SFXType.FinishWashing);
            _spawnedCleanPlates.Add(newItem);
        }
        else
        {
            _inGameSoundManager.PlaySFX(OverCooked.SFXType.FinishWashing);
            _spawnedDirtyPlates.Add(newItem);
        }

        _onCounterItem = newItem;
    }

    // 초기 배치 접시 상태 설정
    private void SetupInitialPlate(GameObject item, Vector3 position)
    {
        item.transform.position = position;
        item.transform.rotation = Quaternion.identity;

        if (item.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    // 리스폰된 스택 접시 상태 설정
    private void SetupStackedPlate(GameObject item, Vector3 position)
    {
        item.transform.SetParent(_snapPoint);
        item.transform.position = position;
        item.transform.localRotation = Quaternion.identity;

        if (item.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    // 제출되어 사라진 접시를 체크아웃 목록에서 제거하고 리스폰 시작
    public void OnPlateDestroyed(GameObject plate)
    {
        Debug.Log($"[PlateRespawn:{name}] OnPlateDestroyed 호출 / ReturnType = {_returnPlateType} / plate = {(plate != null ? plate.name : "NULL")}");

        _checkedOutPlates.Remove(plate);

        if (!_isRespawning)
        {
            StartCoroutine(RespawnRoutine());
        }
    }

    // 현재 가져갈 수 있는 접시가 있는지 확인
    public new bool HasItem => (_spawnedCleanPlates.Count + _spawnedDirtyPlates.Count) > 0;

    // 가장 위에 있는 접시를 꺼내줌
    public override GameObject TakeItem()
    {
        GameObject topPlate = GetTopPlate();

        if (topPlate == null)
        {
            return null;
        }

        if (topPlate.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        topPlate.transform.SetParent(null);

        if (!_checkedOutPlates.Contains(topPlate))
        {
            _checkedOutPlates.Add(topPlate);
        }

        if (_spawnedDirtyPlates.Count > 0)
        {
            _onCounterItem = _spawnedDirtyPlates[_spawnedDirtyPlates.Count - 1];
        }
        else if (_spawnedCleanPlates.Count > 0)
        {
            _onCounterItem = _spawnedCleanPlates[_spawnedCleanPlates.Count - 1];
        }
        else
        {
            _onCounterItem = null;
        }

        return topPlate;
    }

    // 가장 위에 있는 접시를 찾아 리스트에서 제거
    private GameObject GetTopPlate()
    {
        GameObject topClean = null;
        GameObject topDirty = null;

        if (_spawnedCleanPlates.Count > 0)
        {
            topClean = _spawnedCleanPlates[_spawnedCleanPlates.Count - 1];
        }

        if (_spawnedDirtyPlates.Count > 0)
        {
            topDirty = _spawnedDirtyPlates[_spawnedDirtyPlates.Count - 1];
        }

        if (topClean == null && topDirty == null)
        {
            return null;
        }

        if (topClean != null && topDirty == null)
        {
            _spawnedCleanPlates.RemoveAt(_spawnedCleanPlates.Count - 1);
            return topClean;
        }

        if (topClean == null && topDirty != null)
        {
            _spawnedDirtyPlates.RemoveAt(_spawnedDirtyPlates.Count - 1);
            return topDirty;
        }

        if (topDirty.transform.position.y >= topClean.transform.position.y)
        {
            _spawnedDirtyPlates.RemoveAt(_spawnedDirtyPlates.Count - 1);
            return topDirty;
        }
        else
        {
            _spawnedCleanPlates.RemoveAt(_spawnedCleanPlates.Count - 1);
            return topClean;
        }
    }

    // 이 오브젝트에는 직접 아이템을 올려놓지 못하게 막음
    public override bool CanPlaceItem()
    {
        return false;
    }
}