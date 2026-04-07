using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapCamera : MonoBehaviour
{
    [SerializeField] private Transform _target;

    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _smoothSpeed = 5f;

    private bool isControlledByManager;

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

    public void FocusTarget(Transform target, System.Action onComplete)
    {
        StartCoroutine(FocusRoutine(target, onComplete));
    }

    private IEnumerator FocusRoutine(Transform target, System.Action onComplete)
    {
        isControlledByManager = true; // 버스 추적 잠시 정지
        Vector3 startPos = transform.position;
        Vector3 targetPos = target.position + new Vector3(0, 10, -5); // 위에서 내려다보는 위치

        float duration = 1.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        yield return new WaitForSeconds(3.0f);
        onComplete?.Invoke();
    }

    public void ReturnToPlayer(Transform player)
    {
        StartCoroutine(ReturnRoutine(player));
    }

    private IEnumerator ReturnRoutine(Transform player)
    {
        Vector3 startPos = transform.position;
        float duration = 1.0f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);
            transform.position = Vector3.Lerp(startPos, player.position + new Vector3(0, 10, -5), t);
            yield return null;
        }

        isControlledByManager = false; 
    }
}