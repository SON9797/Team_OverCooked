using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateReSpawn : MonoBehaviour
{
    [SerializeField] private GameObject _platePrefab;
    [SerializeField] private int _maxPlate = 4;
    [SerializeField] private float _respawnTime = 0.5f;
    [SerializeField] private float _heightInterval = 0.2f; //쌓이는 접시 높이
    [SerializeField] private Vector3[] _plates; //초기 접시들 위치값

    // 현재 쌓여있는 아이템들을 관리할 리스트
    public List<GameObject> _spawnedPlate = new List<GameObject>();
    private bool _isRespawning = false;

    private void Start()
    {
            StartItemSpawn();
    }
    private void Update()
    {
        _spawnedPlate.RemoveAll(item => item == null);

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
       
        for (int i = 0; i < _maxPlate; i++)
        {
            Vector3 spawnPosition = _plates[i];
            GameObject newItem = Instantiate(_platePrefab, spawnPosition, Quaternion.identity);
            _spawnedPlate.Add(newItem);

            Collider[] colliders = Physics.OverlapSphere(spawnPosition, 0.5f);

            foreach (var col in colliders)
            {
                // 조리대 스크립트가 있는지 확인
                ItemPlaceAndTake counter = col.GetComponentInParent<ItemPlaceAndTake>();

                if (counter != null)
                {
                    // 조리대를 찾았다면, 조리대의 공식적인 PlaceItem 함수를 호출하여 등록
                    // 이렇게 하면 부모 설정, 위치 고정(Kinematic), 변수 등록이 한 번에 해결됩니다.
                    counter.PlaceItem(newItem);
                    Debug.Log($"{newItem.name}이(가) {counter.gameObject.name}에 자동으로 등록되었습니다.");
                    break; // 조리대 하나를 찾았으면 루프 종료
                }
            }
        }
       
    }
    void SpawnStackedItem()
    {
        float currentYOffset = _spawnedPlate.Count * _heightInterval;
        Vector3 spawnPosition = transform.position + new Vector3(0, currentYOffset, 0);
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
