using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class PlateFactory
{
    private readonly IObjectResolver _container;
    private readonly GameObject _prefab;

    public PlateFactory(IObjectResolver container, GameObject prefab)
    {
        _container = container;
        _prefab = prefab;
    }

    public GameObject Create(Vector3 pos)
    {
        return _container.Instantiate(_prefab, pos, Quaternion.identity);
    }
}
