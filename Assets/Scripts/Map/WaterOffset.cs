using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterOffset : MonoBehaviour
{
    private Renderer _renderer;

    private float _currentOffset;

    private float _offsetY = 0f;
    void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    void Update()
    {
        Material[] mats = _renderer.materials;

        _currentOffset -= 0.05f * Time.deltaTime;

        mats[0].SetTextureOffset("_MainTex", new Vector2(_currentOffset, _offsetY));
    }
}