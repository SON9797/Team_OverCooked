using Overcooked.Interfaces;
using OverCooked;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VContainer;

public class BusMove : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 5f;
    [SerializeField] private float _turnSpeed = 5f;

    [SerializeField] private float _dashMultiplier = 0.5f;
    [SerializeField] private float _dashDuration = 1.0f;  
    [SerializeField] private float _dashCooldown = 1.0f;  

    private bool _canMove = false;
    private bool _isDashing = false;
    private bool _canDash = true; 
    private float _currentSpeed;

    public static BusMove _instance;

    private Transform _camTransform;
    private Rigidbody _rb;

    private IWorldMapSoundManager _worldMapSoundManager;

    [Inject]
    public void Construct(IWorldMapSoundManager worldMapSoundManager)
    {
        _worldMapSoundManager = worldMapSoundManager;
    }

    public bool CanMove
    {
        get => _canMove;
        set => _canMove = value;
    }

    void Start()
    {
        if (Camera.main != null)
        {
            _camTransform = Camera.main.transform;
        }

        _rb = GetComponent<Rigidbody>();

        _currentSpeed = _moveSpeed;

        PlayEngineSound();
    }

    private void Update()
    {
        if (!_canMove)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.LeftShift) && _canDash && !_isDashing)
        {
            Debug.Log("대쉬 시작!");
            StartCoroutine(DashRoutine());
        }
    }

    private void FixedUpdate()
    {
        if (!_canMove)
        {
            return;
        }

        MoveAndRotate();
    }

    private void MoveAndRotate()
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
            _rb.MovePosition(_rb.position + moveDir * _currentSpeed * Time.fixedDeltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            targetRotation *= Quaternion.Euler(0, -90, 0);
            _rb.rotation = Quaternion.Slerp(_rb.rotation, targetRotation, _turnSpeed * Time.fixedDeltaTime);            
        }        
    }

    private IEnumerator DashRoutine()
    {
        _isDashing = true;
        _canDash = false;

        _currentSpeed = _moveSpeed * _dashMultiplier;

        yield return new WaitForSeconds(_dashDuration);

        _currentSpeed = _moveSpeed;
        _isDashing = false;

        yield return new WaitForSeconds(_dashCooldown);
        _canDash = true;
    }

    public void Knockback(Vector3 hitDirection, float force = 3f)
    {
        Vector3 knockbackDir = -hitDirection.normalized;
        knockbackDir.y = 0f;
        StartCoroutine(KnockbackRoutine(knockbackDir, force));
    }

    private IEnumerator KnockbackRoutine(Vector3 dir, float force)
    {
        float duration = 0.15f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = 1f - (elapsed / duration);
            _rb.MovePosition(_rb.position + dir * force * t * Time.deltaTime);
            yield return null;
        }
    }

    private void PlayEngineSound()
    {
        if (_worldMapSoundManager != null)
        {
            _worldMapSoundManager.PlayLoopSFX(SFXType.Van_Engine);
        }
    }
}
