using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapManager : MonoBehaviour
{
    public float waveSpeed = 5.0f; // 파동이 퍼지는 속도 (낮을수록 천천히 퍼짐)

    private List<WorldMapTileRotate> allTiles = new List<WorldMapTileRotate>();

    void Start()
    {
        // 맵에 배치된 모든 타일을 리스트에 담습니다.
        // 타일들이 특정 부모 오브젝트 아래에 있다면 그 부모를 참조해서 가져오는 것이 효율적입니다.
        allTiles.AddRange(FindObjectsOfType<WorldMapTileRotate>());
    }

    void Update()
    {
        // 마우스 클릭 시 해당 지점의 타일을 중심으로 뒤집기 시작 (테스트용)
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (RaycastHit(ray, out RaycastHit hit))
            {
                StartWave(hit.point);
            }
        }
    }

    // 특정 위치(center)에서 시작하는 파동 실행
    public void StartWave(Vector3 centerPoint)
    {
        foreach (var tile in allTiles)
        {
            // 중심점에서 타일까지의 거리 계산
            float distance = Vector3.Distance(centerPoint, tile.transform.position);

            // 거리에 비례한 지연 시간 계산 (지연 시간 = 거리 / 속도)
            float delay = distance / waveSpeed;

            StartCoroutine(DelayedFlip(tile, delay));
        }
    }

    private IEnumerator DelayedFlip(WorldMapTileRotate tile, float delay)
    {
        yield return new WaitForSeconds(delay);
        tile.Flip();
    }

    private bool RaycastHit(Ray ray, out RaycastHit hit)
    {
        return Physics.Raycast(ray, out hit);
    }
}