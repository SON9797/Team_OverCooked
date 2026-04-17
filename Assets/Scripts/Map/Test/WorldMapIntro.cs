using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapIntro : MonoBehaviour
{
    [SerializeField] private WorldMapCamera _mapCamera;      // 내 카메라 스크립트
    [SerializeField] private WorldMapManager _tileManager;       // 타일 매니저
    [SerializeField] private Transform _firstStageTransform; // 첫 스테이지 타일 위치
    [SerializeField] private Transform _playerTransform;     // 플레이어(버스)

    [SerializeField] private Vector3 _introOffset = new Vector3(0, 8, -4);
    [SerializeField] private float _cameraMoveDuration = 2.0f; // 카메라 이동 시간

    [SerializeField] private StageController _stageController;

    public bool _stage1_1Open = false;
    private void Start()
    {
        StartCoroutine(IntroSequence());
    }

    private IEnumerator IntroSequence()
    {

        yield return new WaitUntil(() => SaveLoad.instance != null);

        bool hasPlayedIntro = SaveLoad.instance != null
        && SaveLoad.instance.currentData.hasPlayedIntro;

        if (hasPlayedIntro)
        {
            _mapCamera.enabled = true;

            if (_stageController != null)
            {
                //string targetStageKey;

                int ch = SaveLoad.instance.currentData.currentChapter;
                int sub = SaveLoad.instance.currentData.currentSubChapter;
                Debug.Log($"[WorldMapIntro] currentChapter={ch}, currentSubChapter={sub}");
                Debug.Log($"[WorldMapIntro] bestScores 키 목록:");
                foreach (var key in SaveLoad.instance.currentData.bestScores.Keys)
                    Debug.Log($"  bestScore key: {key}");
                Debug.Log($"[WorldMapIntro] unlockedStages 목록:");
                foreach (var key in SaveLoad.instance.currentData.unlockedStages)
                    Debug.Log($"  unlocked: {key}");

                string targetStageKey = (ch > 0 && sub > 0) ? $"{ch}-{sub}" : "1-1";

                Debug.Log($"[WorldMapIntro] targetStageKey={targetStageKey}");

                _stageController.SetBusToStageFlag(targetStageKey, _playerTransform);
            }


            _mapCamera.transform.position = _playerTransform.position + _introOffset;
            _mapCamera.SetTarget(_playerTransform, _introOffset);

            BusMove player = _playerTransform.GetComponent<BusMove>();
            if (player != null) player.CanMove = true;

            _stage1_1Open = true;

            yield return new WaitForSeconds(0.6f);

            if (_stageController != null)
                _stageController.CheckNewStageUnlockPublic();

            yield break;
        }


        _mapCamera.enabled = false;
        _mapCamera.transform.position = _firstStageTransform.position + _introOffset;
        _mapCamera.transform.LookAt(_firstStageTransform);

        yield return new WaitForSeconds(1.0f);

        _tileManager.StartConditionalWave(_firstStageTransform.position, 5.0f);

        yield return new WaitForSeconds(4f);

        Vector3 startPos = _mapCamera.transform.position;
        Quaternion startRot = _mapCamera.transform.rotation;

        Vector3 finalCameraPos = _playerTransform.position + _introOffset;

        float elapsed = 0f;

        while (elapsed < _cameraMoveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / _cameraMoveDuration);

            _mapCamera.transform.position = Vector3.Lerp(startPos, finalCameraPos, t);

            Quaternion targetRot = Quaternion.LookRotation(_playerTransform.position - _mapCamera.transform.position);
            _mapCamera.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);

            yield return null;
        }

        SaveLoad.instance.currentData.hasPlayedIntro = true;

        SaveLoad.instance.currentData.UnlockStage("1-1");

        SaveLoad.instance.AutoSave();

        _stage1_1Open = true;
        _mapCamera.SetTarget(_playerTransform, _introOffset);
        _mapCamera.enabled = true;

        BusMove playerBus = _playerTransform.GetComponent<BusMove>();
        if (playerBus != null)
        {
            playerBus.CanMove = true;
        }

        if (_stageController != null)
        {
            _stageController.CheckNewStageUnlockPublic();
        }

    }
}