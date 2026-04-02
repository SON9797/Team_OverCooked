using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapCamera : MonoBehaviour
{
    [SerializeField] private Transform _target;

    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _smoothSpeed = 5f;

    private void Start()
    {
        if (_offset == Vector3.zero && _target != null)
        {
            _offset = transform.position - _target.position;
        }
    }

    private void FixedUpdate()
    {
        if (_target == null) return;

        Vector3 targetPosition = _target.position + _offset;

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, _smoothSpeed * Time.deltaTime);

        transform.position = smoothedPosition;
    }
}