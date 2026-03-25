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

        private ILevelService _levelService;

        [Header("¼¼ÆÃ")]
        [SerializeField] private float _moveSpeed = 8f;
        [SerializeField] private float _rotationSpeed = 15f;

        [Inject]
        public void Construct(IPlayerInputService inputService, ILevelService levelService)
        {
            _inputService = inputService;
            _levelService = levelService;
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();

            _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        private void Start()
        {
            if (_levelService != null)
            {
                Transform spawnPoint = _levelService.GetPlayerSpawnPoint();

                if (spawnPoint != null)
                {
                    _rb.position = spawnPoint.position;
                    _rb.rotation = spawnPoint.rotation;
                }
            }
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