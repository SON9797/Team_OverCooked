using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectLoopRandomizer : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] private Animator _animator;
    [SerializeField] private string _stateName = "Effect_Loop";

    [Header("속도 랜덤 범위")]
    [SerializeField] private float _minSpeed = 0.9f;
    [SerializeField] private float _maxSpeed = 1.1f;

    [Header("옵션")]
    [SerializeField] private bool _randomStartTime = true;
    [SerializeField] private bool _applyOnEnable = false;

    private void Awake()
    {
        if (_animator == null)
            _animator = GetComponent<Animator>();

        ApplyRandom();
    }

    private void OnEnable()
    {
        if (_applyOnEnable)
            ApplyRandom();
    }

    private void ApplyRandom()
    {
        if (_animator == null)
            return;

        float speed = Random.Range(_minSpeed, _maxSpeed);
        _animator.speed = speed;

        float normalizedTime = _randomStartTime ? Random.value : 0f;
        _animator.Play(_stateName, 0, normalizedTime);
    }
}
