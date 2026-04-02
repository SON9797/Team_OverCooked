using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusMove : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 5f;
    [SerializeField] private float _turnSpeed = 5f;

    public static BusMove _instance;

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
            targetRotation *= Quaternion.Euler(0, -90, 0);
            _rb.rotation = Quaternion.Slerp(_rb.rotation, targetRotation, _turnSpeed * Time.fixedDeltaTime);
        }
    }


}
