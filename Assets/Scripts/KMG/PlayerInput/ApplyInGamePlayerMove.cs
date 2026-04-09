using OverCooked;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Overcooked.Interfaces;
using VContainer;

/// <summary>
/// 플레이어이동 적용, 인터페이스로 구현된 플레이어 인폿을
/// InGamePlayerInput에 구현하고 그것을 받아와 실제 움직임만 담당.
/// </summary>
namespace Overcooked
{
    [RequireComponent(typeof(Rigidbody))]
    public class ApplyInGamePlayerMove : MonoBehaviour
    {
        #region 인스펙터
        [Header("걷기 속도")]
        [SerializeField] private float _moveSpeed = 5f;

        [Header("회전 속도(도/초)")]
        [SerializeField] private float _turnSpeed = 720f;

        [Header("대쉬 속도")]
        [SerializeField] private float _dashPower = 10f;

        [Header("대쉬 지속시간")]
        [SerializeField] private float _dashDuration = 0.3f;

        [Header("대쉬 쿨타임")]
        [SerializeField] private float _dashCoolTime = 0.5f;

        private IInGameSoundManager _inGameSoundManager;
        #endregion

        #region 내부 변수
        private Rigidbody _rb;
        private Vector3 _moveDir;
        private bool _canDash = true;
        private bool _isDashing;

        private Vector3 _conveyorVelocity;
        #endregion

        [Inject]
        public void Construct(IInGameSoundManager inGameSoundManager)
        {
            _inGameSoundManager = inGameSoundManager;
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.freezeRotation = true;
            _rb.constraints = RigidbodyConstraints.FreezeRotation
                            | RigidbodyConstraints.FreezePositionY;
        }

        public void SetMoveInput(Vector2 input)
        {
            _moveDir = new Vector3(input.x, 0f, input.y);
        }

        public void TryDash()
        {
            if (!_canDash || _isDashing)
            {
                return;
            }

            _inGameSoundManager.PlaySFX(SFXType.Dash);

            Vector3 dashDir;

            if (_moveDir.sqrMagnitude > 0.001f)
                dashDir = _moveDir.normalized;
            else
                dashDir = transform.forward.normalized;

            StartCoroutine(DashCoroutine(dashDir));
        }

        private IEnumerator DashCoroutine(Vector3 dashDir)
        {
            _canDash = false;
            _isDashing = true;

            float elapsed = 0f;

            while (elapsed < _dashDuration)
            {
                RotateToward(dashDir);

                _rb.velocity = new Vector3(dashDir.x * _dashPower, _rb.velocity.y, dashDir.z * _dashPower);

                elapsed += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            _isDashing = false;

            yield return new WaitForSeconds(_dashCoolTime);
            _canDash = true;
        }

        private void FixedUpdate()
        {
            if (_isDashing)
            {
                return;
            }

            Vector3 move = _moveDir * _moveSpeed;

            if (_moveDir.sqrMagnitude < 0.001f && _conveyorVelocity.sqrMagnitude > 0.001f)
            {
                float dot = Vector3.Dot(_moveDir.normalized, _conveyorVelocity.normalized);
                move *= (1f + (dot * 0.2f));
            }

            Vector3 finalVel = move + _conveyorVelocity;

            _rb.velocity = new Vector3(finalVel.x, _rb.velocity.y, finalVel.z);

            if (_moveDir.sqrMagnitude > 0.001f)
            {
                RotateToward(_moveDir);
            }
        }

        private void RotateToward(Vector3 dir)
        {
            dir.y = 0f;

            if (dir.sqrMagnitude <= 0.001f)
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(dir.normalized);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                _turnSpeed * Time.fixedDeltaTime
            );
        }

        public void SetConveyorVelocity(Vector3 velocity)
        {
            _conveyorVelocity = velocity;
        }
    }
}
