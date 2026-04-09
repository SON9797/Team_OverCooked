using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform[] waypoints; // 중앙, 오른쪽, 중앙, 왼쪽 순서로 지점을 넣어주세요.
    public float moveSpeed = 2.0f;
    public float waitTime = 10f; 

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
            _currentIndex = (_currentIndex + 1) % waypoints.Length;
            Vector3 targetPosition = waypoints[_currentIndex].position;

            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPosition,
                    moveSpeed * Time.deltaTime
                );
                yield return null;
            }

            transform.position = targetPosition;

            yield return new WaitForSeconds(waitTime);
        }
    }

    // 통나무 위의 플레이어가 같이 움직이게
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
