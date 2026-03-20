using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StuffOffset : MonoBehaviour
{
    private Renderer _renderer;
    [SerializeField] private float _offsetX = 0.2f;
    [SerializeField] private float _offsetY = 0.2f;
    void Start()
    {
        _renderer = GetComponent<Renderer>();

        Material[] mats = _renderer.materials;

        if (mats.Length > 1)
        {
            mats[1].SetTextureOffset("_MainTex", new Vector2(_offsetX, _offsetY));
        }
    }
}
