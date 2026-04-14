using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusScale : MonoBehaviour
{
    [Header("¼³Á¤")]
    [SerializeField] private float _scaleSpeed = 20f;
    [SerializeField] private float _amount = 0.01f;

    private Vector3 _originScale;


    void Start()
    {
        _originScale = transform.localScale;
    }


    void Update()
    {
        float wave = Mathf.Sin(Time.time * _scaleSpeed) * _amount;

        transform.localScale = _originScale + new Vector3(wave, wave, wave);
    }
}
