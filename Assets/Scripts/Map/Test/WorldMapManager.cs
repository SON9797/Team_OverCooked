using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapManager : MonoBehaviour
{
    public float _waveSpeed = 5.0f;
    private List<WorldMapTileRotate> allTiles = new List<WorldMapTileRotate>();

    void Start()
    {
        allTiles.AddRange(FindObjectsOfType<WorldMapTileRotate>());
    }

    /// <summary>
    /// 특정 지점을 중심으로 특정 반경 내의 타일만 뒤집습니다.
    /// </summary>
    /// <param name="centerPoint">중심 위치</param>
    /// <param name="maxRadius">뒤집힐 범위 (이 거리보다 멀면 무시)</param>
    public void StartConditionalWave(Vector3 centerPoint, float maxRadius)
    {
        foreach (var tile in allTiles)
        {
            float distance = Vector3.Distance(centerPoint, tile.transform.position);

            // 설정한 반경 이내에 있는 타일만 실행
            if (distance <= maxRadius)
            {
                float delay = distance / _waveSpeed;
                StartCoroutine(DelayedFlip(tile, delay));
            }
        }
    }

    private IEnumerator DelayedFlip(WorldMapTileRotate tile, float delay)
    {
        yield return new WaitForSeconds(delay);
        tile.Flip();
    }
}