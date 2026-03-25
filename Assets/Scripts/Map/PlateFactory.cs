using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class PlateFactory
{
    [Inject] IObjectResolver _container;
    private readonly GameObject _prefab;

    public PlateFactory(IObjectResolver container, GameObject prefab)
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
