using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBeltOffset : MonoBehaviour
{
    private Renderer _renderer;

    private float _currentOffset;
    
    private float _offsetX = 0f;
    private float _offsetY = 0f;
    void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    void Update()
    {
        Material[] mats = _renderer.materials;

        _currentOffset += 0.5f * Time.deltaTime;

        if (mats.Length > 1)
        {
            mats[1].SetTextureOffset("_MainTex", new Vector2(_currentOffset, _offsetY));
        }
    }
}
