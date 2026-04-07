using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapCamera : MonoBehaviour
{
    [SerializeField] private Transform _target;

    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _smoothSpeed = 5f;


    private void FixedUpdate()
    {
        if (_target == null)
        {
            return;
        }

        Vector3 targetPosition = _target.position + _offset;

        transform.position = Vector3.Lerp(transform.position, targetPosition, _smoothSpeed * Time.deltaTime);
    }

    public void SetTarget(Transform newTarget, Vector3 customOffset)
    {
        _target = newTarget;
        _offset = customOffset;
        this.enabled = true;
    }
}