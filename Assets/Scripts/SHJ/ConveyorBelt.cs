using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Overcooked;

public class ConveyorBelt : MonoBehaviour
{
    [Header("¼³Á¤")]
    [SerializeField] private float _beltSpeed = 2f;
    [SerializeField] private Vector3 _direction = Vector3.left;

    private int _playerLayer;

    private void Awake()
    {
        _playerLayer = LayerMask.NameToLayer("Player");
    }

    private Vector3 WorldVelocity => transform.TransformDirection(_direction) * _beltSpeed;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == _playerLayer)
        {
            if (other.TryGetComponent<ApplyInGamePlayerMove>(out var player))
            {
                player.SetConveyorVelocity(WorldVelocity);
            }
        }

        else if (other.attachedRigidbody != null)
        {
            other.attachedRigidbody.position = WorldVelocity * Time.deltaTime;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == _playerLayer)
        {
            if (other.TryGetComponent<ApplyInGamePlayerMove>(out var player))
            {
                player.SetConveyorVelocity(Vector3.zero);
            }
        }
    }
}
