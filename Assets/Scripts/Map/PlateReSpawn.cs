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
        for (int i = 0; i < _maxPlate; i++)
        {
            Vector3 spawnPos = (_plates != null && i < _plates.Length) ? _plates[i] : transform.position + (Vector3.up * i * _heightInterval);

            GameObject plate = Instantiate(_platePrefab, spawnPos, Quaternion.identity);
            _spawnedPlate.Add(plate);
        }

    }
    private void Update()
    {
        // 리스트에서 파괴된 아이템 제거
        _spawnedPlate.RemoveAll(item => item == null);

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

        SpawnStackedItem();
        _isRespawning = false;
    }

    void SpawnStackedItem()
    {
        float currentYOffset = _spawnedPlate.Count * _heightInterval;
        Vector3 spawnPosition = transform.position + new Vector3(0, currentYOffset, 0);
        _heightInterval += 0.05f;
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
