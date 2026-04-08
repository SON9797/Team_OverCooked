using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StageController : MonoBehaviour
{
    public WorldMapManager _tileManager;
    public Transform[] _stageFlagTransform; // 현재 스테이지 깃발 위치

    public WorldMapCamera _mapCamera;
    private void Start()
    {
        PlayerPrefs.DeleteAll();
        CheckNewStageUnlock();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            MarkStageAsCleared(1);
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

                // 연출을 봤으니 다시는 안 나오게 0으로 변경
                PlayerPrefs.SetInt(key, 0);
                PlayerPrefs.Save();
                break;
            }
        }
    }
    public void OnStageUnlockAnimation(int stageIndex)
    {
        Debug.Log($"{stageIndex} 스테이지 길 열기 연출 시작!");

        Transform target = _stageFlagTransform[stageIndex];

        _mapCamera.FocusTarget(target, () =>
        {
            _tileManager.StartConditionalWave(target.position, 5.0f);

            StartCoroutine(WaitAndReturn());
        });

        _tileManager.StartConditionalWave(_stageFlagTransform[stageIndex].position, 5.0f);
    }

    private IEnumerator WaitAndReturn()
    {
        yield return new WaitForSeconds(3.0f); // 타일이 뒤집히는 애니메이션 시간 확보
        _mapCamera.ReturnToPlayer(BusMove._instance.transform);
    }

    // 게임 클리어시 호출
    public void MarkStageAsCleared(int clearedStageIndex)
    {
        PlayerPrefs.SetInt("Stage_Clear_" + clearedStageIndex, 1);

        int nextStage = clearedStageIndex + 1;
        if (nextStage < _stageFlagTransform.Length)
        {
            if (PlayerPrefs.GetInt("Stage_" + nextStage + "_UnlockAnimation", 0) == 0)
            {
                PlayerPrefs.SetInt("Stage_" + nextStage + "_UnlockAnimation", 1);
            }
        }

        PlayerPrefs.Save();
    }

    
}
