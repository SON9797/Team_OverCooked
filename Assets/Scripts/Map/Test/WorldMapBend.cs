using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapBend : MonoBehaviour
{
    [SerializeField] private float _bendAngle = 20f;
    [SerializeField] private float _bendDuration = 0.3f;
    [SerializeField] private float _holdDuration = 0.2f;
    [SerializeField] private float _returnDuration = 0.4f;
    [SerializeField] private float _triggerRadius = 0.8f;

    private Quaternion _originalRotation;
    private bool _isBending = false;
    private bool _isPlayerTouching = false;
    private void Awake()
    {
        _originalRotation = transform.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        _isPlayerTouching = true;

        BusMove bus = other.GetComponent<BusMove>();
        if (bus != null)
        {
            Vector3 dirToPlayer = other.transform.position - transform.position;
            dirToPlayer.y = 0f;
            bus.Knockback(dirToPlayer, 4f);
        }


        if (_isBending)
        {
            return;
        }
       
        Vector3 dir = other.transform.position - transform.position;
        dir.y = 0f;
        dir.Normalize();

        StartCoroutine(BendRoutine(-dir));
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _isPlayerTouching = false;
    }

    private IEnumerator BendRoutine(Vector3 bendDirection)
    {
        _isBending = true;

        // 휘어질 목표 회전값 계산 (버스 방향으로 기움)
        Quaternion bentRotation = _originalRotation * Quaternion.FromToRotation(
            Vector3.up,
            Vector3.Lerp(Vector3.up, bendDirection, Mathf.Sin(_bendAngle * Mathf.Deg2Rad))
        );

        // 휘어지기
        float elapsed = 0f;
        while (elapsed < _bendDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _bendDuration;
            float easedT = 1f - Mathf.Pow(1f - t, 3f); // EaseOutCubic
            transform.rotation = Quaternion.Slerp(_originalRotation, bentRotation, easedT);
            yield return null;
        }
        transform.rotation = bentRotation;

        // 휘어진 상태 유지
        yield return new WaitForSeconds(_holdDuration);
        while (_isPlayerTouching) // 플레이어가 떠날 때까지 대기
        {
            yield return null;
        }

        // 원래 상태로 복귀 (튕기는 느낌)
        elapsed = 0f;
        while (elapsed < _returnDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _returnDuration;
            float springT = 1f - Mathf.Exp(-6f * t) * Mathf.Cos(12f * t);
            transform.rotation = Quaternion.Slerp(bentRotation, _originalRotation, springT);
            yield return null;
        }
        transform.rotation = _originalRotation;

        _isBending = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _triggerRadius);
    }
}
