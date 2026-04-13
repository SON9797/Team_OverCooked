using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform[] _waypoints;
    public float _moveSpeed = 2.0f; // 이동 속도
    public float _waitTime = 10f;  // 정지 지점에서의 대기 시간

    private int _currentIndex = 0;
    private bool _isMoving = true;

    void Start()
    {
        if (_waypoints.Length > 0)
        {
            transform.position = _waypoints[0].position;
            StartCoroutine(MoveRoutine());
        }
    }

    IEnumerator MoveRoutine()
    {
        while (true)
        {
            _currentIndex = (_currentIndex + 1) % _waypoints.Length;

            Debug.Log($"현재 목표 지점 인덱스: {_currentIndex} / 좌표: {_waypoints[_currentIndex].position}");

            Vector3 targetPosition = _waypoints[_currentIndex].position;

            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPosition,
                    _moveSpeed * Time.deltaTime
                );
                yield return null;
            }

            transform.position = targetPosition;

            yield return new WaitForSeconds(_waitTime);
        }
    }

}