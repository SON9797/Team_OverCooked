using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerMove : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 5f;
    [SerializeField] private float _jumpImpulse = 6.0f;
    [SerializeField] private float _groundCheakDistance = 0.6f;
    [SerializeField] private float _turnSpeed = 10f;

    [SerializeField] private LayerMask _groundMask = ~0;

    [SerializeField] private BoxCollider _endPoint;

    public static TestPlayerMove _instance;

    private Transform _camTransform;
    private Rigidbody _rb;
    void Start()
    {
        if (Camera.main != null)
        {
            _camTransform = Camera.main.transform;
        }

        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        MoveAndRotate();
    }

    void MoveAndRotate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 forward = _camTransform.forward;
        Vector3 right = _camTransform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = (forward * v + right * h).normalized;

        if (moveDir.magnitude >= 0.1f)
        {
            _rb.MovePosition(_rb.position + moveDir * _moveSpeed * Time.fixedDeltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            _rb.rotation = Quaternion.Slerp(_rb.rotation, targetRotation, _turnSpeed * Time.fixedDeltaTime);
        }
    }

    

}
