using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StageController : MonoBehaviour
{
    public WorldMapManager _tileManager;
    public Transform[] _stageFlagTransform; // 현재 스테이지 깃발 위치

    public BusMove _busMove;

    public WorldMapCamera _mapCamera;

    private Coroutine _waitRoutine;
    private int _testStageIndex = 1;
    private void Start()
    {
        PlayerPrefs.DeleteAll();
        CheckNewStageUnlock();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TestUnlockNextStage();
        }
    }

    private void TestUnlockNextStage()
    {
        // 배열 범위를 넘지 않는지 체크
        if (_testStageIndex < _stageFlagTransform.Length)
        {
            Debug.Log($"{_testStageIndex} 스테이지 연출 테스트 시작!");

            // 즉시 연출 호출
            OnStageUnlockAnimation(_testStageIndex);

            // 다음번엔 그다음 스테이지가 열리도록 인덱스 증가
            _testStageIndex++;
        }
        else
        {
            Debug.Log("모든 스테이지 연출을 확인했습니다.");
        }
    }

    private void CheckNewStageUnlock()
    {
        for (int i = 1; i < _stageFlagTransform.Length; i++)
        {
            string key = "Stage_" + i + "_UnlockAnimation";

            if (PlayerPrefs.GetInt(key, 0) == 1)
            {
                OnStageUnlockAnimation(i);

                PlayerPrefs.SetInt(key, 0);
                PlayerPrefs.Save();
                break;
            }
        }
    }
    public void OnStageUnlockAnimation(int stageIndex)
    {
        if (_waitRoutine != null)
        {
            StopCoroutine(_waitRoutine);
            _waitRoutine = null;
        }

        Debug.Log($"{stageIndex} 스테이지 길 열기 연출 시작!");

        Transform target = _stageFlagTransform[stageIndex];

        _mapCamera.FocusTarget(target, () =>
        {
            _tileManager.StartConditionalWave(target.position, 5.0f);
            if (_waitRoutine != null)
            {
                StopCoroutine(_waitRoutine);
            }
            _waitRoutine = StartCoroutine(WaitAndReturn());
        });
    }

    private IEnumerator WaitAndReturn()
    {
        yield return new WaitForSecondsRealtime(3.5f);

        _mapCamera.ReturnToPlayer(_busMove.transform);

        _waitRoutine = null;
    }

    // 게임 클리어시 호출
    public void MarkStageAsCleared(int clearedStageIndex)
    {
        int nextStage = clearedStageIndex + 1;

        if (nextStage < _stageFlagTransform.Length)
        {
            OnStageUnlockAnimation(nextStage);
        }
        else
        {
            Debug.Log("마지막 스테이지입니다! 더 이상 열 길 이 없습니다.");
        }
    }


}
