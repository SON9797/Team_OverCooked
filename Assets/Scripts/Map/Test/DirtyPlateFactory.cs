using Overcooked.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class DirtyPlateFactory
{
    [Inject] IObjectResolver _container;

    private readonly GameObject _prefab;

    public DirtyPlateFactory(IObjectResolver container, GameObject prefab)
    {
        _container = container;
        _prefab = prefab;

        Debug.Log("µî·Ï");
    }

    public GameObject Create(Vector3 pos)
    {
        return _container.Instantiate(_prefab, pos, Quaternion.identity);
    }
}

