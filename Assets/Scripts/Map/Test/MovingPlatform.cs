using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform[] waypoints; // 중앙, 오른쪽, 중앙, 왼쪽 순서로 지점을 넣어주세요.
    public float moveSpeed = 2.0f; // 이동 속도
    public float waitTime = 1.5f;  // 정지 지점에서의 대기 시간

    private int _currentIndex = 0;
    private bool _isMoving = true;

    void Start()
    {
        if (waypoints.Length > 0)
        {
            // 시작 시 첫 번째 포인트로 위치 고정
            transform.position = waypoints[0].position;
            StartCoroutine(MoveRoutine());
        }
    }

    IEnumerator MoveRoutine()
    {
        while (true)
        {
            // 1. 다음 목표 지점 설정
            _currentIndex = (_currentIndex + 1) % waypoints.Length;

            Debug.Log($"현재 목표 지점 인덱스: {_currentIndex} / 좌표: {waypoints[_currentIndex].position}");

            Vector3 targetPosition = waypoints[_currentIndex].position;

            // 2. 목표 지점까지 이동
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPosition,
                    moveSpeed * Time.deltaTime
                );
                yield return null;
            }

            // 위치 보정
            transform.position = targetPosition;

            // 3. 도착 후 대기
            yield return new WaitForSeconds(waitTime);
        }
    }

    // 통나무 위의 플레이어가 같이 움직이게 하기 위한 처리
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}