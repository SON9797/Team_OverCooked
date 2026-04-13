using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

[Serializable]
public class StageTransform
{
    public string name;
    public Transform transform;
}

public class StageController : MonoBehaviour
{
    public WorldMapManager _tileManager;
    [SerializeField] List<StageTransform> _stageFlagTransformInput;// 현재 스테이지 깃발 위치
    Dictionary<string, Transform> _stageTransformDict = new Dictionary<string, Transform>();

    public BusMove _busMove;

    public WorldMapCamera _mapCamera;

    private Coroutine _waitRoutine;
    private string _testStageName = "1-1";
    private void Start()
    {
        //PlayerPrefs.DeleteAll();
        _stageTransformDict.Clear();
        for (int i = 0; i < _stageFlagTransformInput.Count; i++)
        {
            _stageTransformDict[_stageFlagTransformInput[i].name] = _stageFlagTransformInput[i].transform;
        }

        
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //   TestUnlockNextStage();
        //}
    }

    private void TestUnlockNextStage()
    {
        // 배열 범위를 넘지 않는지 체크
        if (_stageTransformDict.ContainsKey(_testStageName))
        {
            Debug.Log($"{_testStageName} 스테이지 연출 테스트 시작!");

            // 즉시 연출 호출
            OnStageUnlockAnimation(_testStageName);

            // 다음번엔 그다음 스테이지가 열리도록 인덱스 증가
            StagePlus();
        }
        else
        {
            Debug.Log("모든 스테이지 연출을 확인했습니다.");
        }
    }

    public void CheckNewStageUnlockPublic()
    {
        Debug.Log("[StageController] CheckNewStageUnlockPublic 호출됨");
        Debug.Log($"[StageController] PendingUnlockStage 존재 여부: {PlayerPrefs.HasKey("PendingUnlockStage")}");


        if (PlayerPrefs.HasKey("PendingUnlockStage"))
        {
            string nextStageKey = PlayerPrefs.GetString("PendingUnlockStage");
            Debug.Log($"[StageController] 해금 시도 키: {nextStageKey}");
            Debug.Log($"[StageController] _stageTransformDict 키 목록:");
            foreach (var key in _stageTransformDict.Keys)
                Debug.Log($"  - {key}");


            if (_stageTransformDict.ContainsKey(nextStageKey))
            {
                OnStageUnlockAnimation(nextStageKey);
            }
            else
            {
                Debug.LogWarning($"[StageController] '{nextStageKey}' 키가 딕셔너리에 없음!");
            }
            PlayerPrefs.DeleteKey("PendingUnlockStage");
            PlayerPrefs.Save();
        }
        else
        {
            Debug.Log("[StageController] PendingUnlockStage가 PlayerPrefs에 없음!");
            Debug.Log("[StageController] 현재 저장된 모든 베스트스코어 키:");
            if (SaveLoad.instance != null)
            {
                foreach (var key in SaveLoad.instance.currentData.bestScores.Keys)
                    Debug.Log($"  - {key}");
            }
        }
    }


    private void CheckNewStageUnlock() => CheckNewStageUnlockPublic();
        /*
        OnStageUnlockAnimation(_testStageName);
        // 다음번엔 그다음 스테이지가 열리도록 인덱스 증가
        StagePlus();
        /*
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
        */
    public void OnStageUnlockAnimation(string stageIndex)
    {
        if (_waitRoutine != null)
        {
            StopCoroutine(_waitRoutine);
            _waitRoutine = null;
        }

        Debug.Log($"{stageIndex} 스테이지 길 열기 연출 시작!");

        Transform target = _stageTransformDict[stageIndex];

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
    public void 
        
        MarkStageAsCleared(int mainChapter, int subChapter, int score, int stars)
    {
        Debug.Log($"[StageController] MarkStageAsCleared 호출됨: {mainChapter}-{subChapter}");

        if (SaveLoad.instance != null)
        {
            SaveLoad.instance.CurrentDataUpdate(mainChapter, subChapter, score, stars);
            SaveLoad.instance.AutoSave();
            Debug.Log($"[StageController] {mainChapter}-{subChapter} 클리어 및 저장 완료");
        }
        else
        {
            Debug.LogError("[StageController] SaveLoad.instance가 null!");
        }

        string nextStageKey = InputStagePlus($"{mainChapter}-{subChapter}");
        Debug.Log($"[StageController] 다음 스테이지 키: {nextStageKey}");

        // 게임씬엔 월드맵 타일이 없으므로 딕셔너리 검증 없이 바로 저장
        PlayerPrefs.SetString("PendingUnlockStage", nextStageKey);
        PlayerPrefs.Save();
        Debug.Log($"[StageController] PlayerPrefs 저장 완료: {nextStageKey}");
    }

    private string InputStagePlus(string inputstage)
    {
        var split = inputstage.Split('-');
        int mainChapter = int.Parse(split[0]);
        int subChapter = int.Parse(split[1]) + 1;

        return $"{mainChapter}-{subChapter}";
    }
    private void StagePlus()
    {
        var split = _testStageName.Split('-');

        int mainChapter = int.Parse(split[0]);
        int subChapter = int.Parse(split[1])+1;

        string connect = $"{mainChapter}-{subChapter}";
        if (!_stageTransformDict.ContainsKey(connect))
        {
            connect = $"{mainChapter + 1}-1";
        }
        _testStageName = connect;
    }


}
