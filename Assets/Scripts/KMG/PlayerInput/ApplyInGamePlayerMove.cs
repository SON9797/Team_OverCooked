using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        [Header("대쉬 속도")]
        [SerializeField] private float _dashPower = 10f;
        [Header("대쉬 지속시간")]
        [SerializeField] private float _dashDuration = 0.3f;
        [Header("대쉬 쿨타임")]
        [SerializeField] private float _dashCoolTime = 0.5f;
        
        #endregion
    
    
        #region 내부 변수
        Rigidbody _rb;
        Vector3 _moveDir;
        private bool _canDash = true;
        private bool _isDashing;
    
    
        #endregion
    
    
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.freezeRotation = true;
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

            Vector3 DashDir;
    
            if (_moveDir.sqrMagnitude > 0.001f) DashDir = _moveDir.normalized;
            else DashDir = transform.forward.normalized;
    
            
            StartCoroutine(DashCoroutine(DashDir));
        }
    
    
        /// <summary>
        /// 대쉬 지속시간동안 움직이지 못하도록 하기 위해 코루틴 사용.
        /// </summary>
        /// <param name="dashDir"></param>
        /// <returns></returns>
        private IEnumerator DashCoroutine(Vector3 dashDir)
        {
            _canDash = false;
            _isDashing = true;
    
            transform.forward = dashDir;
            float elapsed = 0f;
    
            //대쉬지속시간동안 가속.
            while (elapsed < _dashDuration)
            {
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

            Vector3 v = _moveDir * _moveSpeed;
    
            _rb.velocity = new Vector3(v.x,_rb.velocity.y, v.z);
    
    
            if(_moveDir.sqrMagnitude > 0.001f)
            {
                transform.forward = _moveDir;
            }
    
        }
    
    }
}
