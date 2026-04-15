using Overcooked.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class WorldMapManager : MonoBehaviour
{
    public float _waveSpeed = 5.0f;
    private List<WorldMapTileRotate> allTiles = new List<WorldMapTileRotate>();

    private IWorldMapSoundManager _soundManager;

    [Inject]
    public void Construct(IWorldMapSoundManager soundManager)
    {
        _soundManager = soundManager;
    }

    void Start()
    {
        allTiles.AddRange(FindObjectsOfType<WorldMapTileRotate>());
    }

    public void StartConditionalWave(Vector3 centerPoint, float maxRadius)
    {
        Debug.Log($"[WorldMapManager] StartConditionalWave 호출됨. 타일 수: {allTiles.Count}");

        if (_soundManager != null)
        {
            _soundManager.PlaySFX(OverCooked.SFXType.World_Tile);
        }

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