using Overcooked.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace Overcooked
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        private IPlayerInputService _inputService;
        private Rigidbody _rb;

        [Header("¼¼ÆÃ")]
        [SerializeField] private float _moveSpeed = 8f;
        [SerializeField] private float _rotationSpeed = 15f;

        [Inject]
        public void Construct(IPlayerInputService inputService)
        {
            _inputService = inputService;
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();

            _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        private void FixedUpdate()
        {
            if (_inputService == null)
            {
                return;
            }

            Vector2 input = _inputService.GetMovementInput();

            if (input.sqrMagnitude < 0.01f)
            {
                return;
            }

            Vector3 moveDir = new Vector3(input.x, 0f, input.y);

            Vector3 targetPosition = _rb.position + moveDir * _moveSpeed * Time.deltaTime;
            _rb.MovePosition(targetPosition);

            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRot, _rotationSpeed * Time.deltaTime));
        }
    }
}