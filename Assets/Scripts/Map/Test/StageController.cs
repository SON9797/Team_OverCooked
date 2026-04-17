using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    [SerializeField] private float _busOffset = 2.0f;

    private Coroutine _waitRoutine;
    private string _testStageName = "1-1";
    private void Start()
    {
        _stageTransformDict.Clear();
        for (int i = 0; i < _stageFlagTransformInput.Count; i++)
        {
            _stageTransformDict[_stageFlagTransformInput[i].name] = _stageFlagTransformInput[i].transform;
        }

        
    }
    
    public void CheckNewStageUnlockPublic()
    {
        if (PlayerPrefs.HasKey("PendingUnlockStage"))
        {
            string nextStageKey = PlayerPrefs.GetString("PendingUnlockStage");
            foreach (var key in _stageTransformDict.Keys)
                Debug.Log($"  - {key}");


            if (_stageTransformDict.ContainsKey(nextStageKey))
            {
                if (SaveLoad.instance != null)
                {
                    SaveLoad.instance.currentData.UnlockStage(nextStageKey);
                    SaveLoad.instance.AutoSave();
                }
                OnStageUnlockAnimation(nextStageKey);
            }
            PlayerPrefs.DeleteKey("PendingUnlockStage");
            PlayerPrefs.Save();
        }
        else
        {
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
        Debug.Log($"[StageController] OnStageUnlockAnimation 호출됨: {stageIndex}");


        if (_waitRoutine != null)
        {
            StopCoroutine(_waitRoutine);
            _waitRoutine = null;
        }


        if (!_stageTransformDict.ContainsKey(stageIndex))
        {
            Debug.LogError($"[StageController] {stageIndex} 키가 딕셔너리에 없음!");
            return;
        }

        Transform target = _stageTransformDict[stageIndex];

        Debug.Log($"[StageController] 타겟 Transform: {target.name}, 위치: {target.position}");


        _mapCamera.FocusTarget(target, () =>
        {
            Debug.Log($"[StageController] FocusTarget 콜백 실행됨");

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
    public void MarkStageAsCleared(int mainChapter, int subChapter, int score, int stars)
    {

        if (SaveLoad.instance != null)
        {
            SaveLoad.instance.CurrentDataUpdate(mainChapter, subChapter, score, stars);
            SaveLoad.instance.AutoSave();

            Debug.Log($"[MarkStageAsCleared] 저장 후 currentChapter={SaveLoad.instance.currentData.currentChapter}, currentSubChapter={SaveLoad.instance.currentData.currentSubChapter}");
            Debug.Log($"[MarkStageAsCleared] bestScores 목록:");
            foreach (var key in SaveLoad.instance.currentData.bestScores.Keys)
                Debug.Log($"  key: {key}");
        }
        else
        {
            Debug.LogError("[MarkStageAsCleared] SaveLoad.instance가 null!");
        }

        string nextStageKey = InputStagePlus($"{mainChapter}-{subChapter}");
        // 게임씬엔 월드맵 타일이 없으므로 딕셔너리 검증 없이 바로 저장
        PlayerPrefs.SetString("PendingUnlockStage", nextStageKey);
        PlayerPrefs.Save();
    }

    private string InputStagePlus(string inputstage)
    {
        var split = inputstage.Split('-');
        int mainChapter = int.Parse(split[0]);
        int subChapter = int.Parse(split[1]) + 1;

        return $"{mainChapter}-{subChapter}";
    }
    
    public void SetBusToStageFlag(string stageKey, Transform busTransform)
    {
        if (!_stageTransformDict.ContainsKey(stageKey))
        {
            Debug.LogWarning($"[StageController] {stageKey} 깃발을 찾을 수 없음");
            return;
        }

        Transform flag = _stageTransformDict[stageKey];

        Vector3 busPos = flag.position + flag.forward * _busOffset;
        busPos.y = busTransform.position.y;

        busTransform.position = busPos;
    }
}
