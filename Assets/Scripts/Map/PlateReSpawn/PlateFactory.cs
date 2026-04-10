using Overcooked.Interfaces;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class PlateFactory
{
    [Inject] private IObjectResolver _container;

    private readonly GameObject _cleanPlatePrefab;
    private readonly GameObject _dirtyPlatePrefab;

    public PlateFactory(IObjectResolver container, GameObject cleanPlatePrefab, GameObject dirtyPlatePrefab)
    {
        _container = container;
        _cleanPlatePrefab = cleanPlatePrefab;
        _dirtyPlatePrefab = dirtyPlatePrefab;

        Debug.Log($"[PlateFactory] CleanPrefab = {(_cleanPlatePrefab != null ? _cleanPlatePrefab.name : "NULL")}, DirtyPrefab = {(_dirtyPlatePrefab != null ? _dirtyPlatePrefab.name : "NULL")}");
    }

    // 깨끗한 접시 생성
    public GameObject CreateClean(Vector3 pos)
    {
        Debug.Log($"[PlateFactory] CreateClean 호출 / prefab = {(_cleanPlatePrefab != null ? _cleanPlatePrefab.name : "NULL")}");
        return _container.Instantiate(_cleanPlatePrefab, pos, Quaternion.identity);
    }

    // 더러운 접시 생성
    public GameObject CreateDirty(Vector3 pos)
    {
        Debug.Log($"[PlateFactory] CreateDirty 호출 / prefab = {(_dirtyPlatePrefab != null ? _dirtyPlatePrefab.name : "NULL")}");
        return _container.Instantiate(_dirtyPlatePrefab, pos, Quaternion.identity);
    }

    // 타입에 따라 접시 생성
    public GameObject Create(Vector3 pos, PlateReturnType plateType)
    {
        Debug.Log($"[PlateFactory] Create 호출 / plateType = {plateType}");

        switch (plateType)
        {
            case PlateReturnType.Dirty:
                return CreateDirty(pos);

            case PlateReturnType.Clean:
            default:
                return CreateClean(pos);
        }
    }
}