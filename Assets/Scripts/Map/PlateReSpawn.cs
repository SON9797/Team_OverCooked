using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateReSpawn : MonoBehaviour
{
    [SerializeField] private GameObject _platePrefab;
    [SerializeField] private int _maxPlate = 4;
    [SerializeField] private float _respawnTime = 0.5f;
    [SerializeField] private float _heightInterval = 0.5f; //쌓이는 접시 높이
    [SerializeField] private Vector3[] _plates; //초기 접시들 위치값

    // 현재 쌓여있는 아이템들을 관리할 리스트
    public List<GameObject> _spawnedPlate = new List<GameObject>();
    private bool _isRespawning = false;

    private void Start()
    {
        _spawnedPlate.Clear();

        for (int i = 0; i < _maxPlate; i++)
        {
            StartItemSpawn();
        }

    }
    private void Update()
    {
        if (_spawnedPlate.Contains(null))
        {
            _spawnedPlate.RemoveAll(item => item == null);
        }

        // 아이템이 최대치보다 적고, 리스폰 루틴이 실행 중이지 않을 때
        if (_spawnedPlate.Count < _maxPlate && !_isRespawning)
        {
            StartCoroutine(RespawnRoutine());
        }
    }

    IEnumerator RespawnRoutine()
    {
        _isRespawning = true;
        yield return new WaitForSeconds(_respawnTime);

        if (_spawnedPlate.Count < _maxPlate)
        {
            SpawnStackedItem();
        }

        _isRespawning = false;
    }

    private void StartItemSpawn()
    {
       
        for (int i = 0; i <= 4; i++)
        {
            Vector3 spawnPosition = _plates[i];
            GameObject newItem = Instantiate(_platePrefab, spawnPosition, Quaternion.identity);
            _spawnedPlate.Add(newItem);
        }
       
    }
    void SpawnStackedItem()
    {
        float currentYOffset = _heightInterval;
        Vector3 spawnPosition = transform.position + new Vector3(0, currentYOffset, 0);
        _heightInterval += 0.2f;
        GameObject newItem = Instantiate(_platePrefab, spawnPosition, Quaternion.identity);
        _spawnedPlate.Add(newItem);
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
}
