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

    public void StartConditionalWave(Vector3 centerPoint, float maxRadius)
    {
        Debug.Log($"[WorldMapManager] StartConditionalWave 호출됨. 타일 수: {allTiles.Count}");

        foreach (var tile in allTiles)
        {
            float distance = Vector3.Distance(centerPoint, tile.transform.position);

            if (distance <= maxRadius)
            {
                Debug.Log($"[WorldMapManager] 범위 내 타일: {tile.gameObject.name}, 거리: {distance}");

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