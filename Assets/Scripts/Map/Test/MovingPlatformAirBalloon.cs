using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformAirBalloon : MonoBehaviour
{
    [SerializeField] private Transform[] _waypoints;
    [SerializeField] private Transform _startPoints;

    [SerializeField] private float _moveSpeed = 2.0f;
    [SerializeField] private float _waitTime = 1.5f;
    [SerializeField] private float _startDelay = 20.0f;

    private int _currentIndex = 0;

    void Start()
    {
        if (_waypoints.Length > 0)
        {
            StartCoroutine(MoveRoutine());
        }
    }
    private IEnumerator MoveRoutine()
    {
        transform.position = _startPoints.position;
        yield return new WaitForSeconds(_startDelay);

        transform.position = _waypoints[0].position;

        while (true)
        {
            _currentIndex = (_currentIndex + 1) % _waypoints.Length;

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