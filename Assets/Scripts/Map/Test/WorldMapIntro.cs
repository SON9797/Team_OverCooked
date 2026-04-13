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
        Debug.Log("[Intro] 루틴 시작");

        if (SaveLoad.instance == null)
        {
            Debug.LogError("[Intro] SaveLoad 인스턴스가 없습니다!");
        }
        else
        {
            Debug.Log($"[Intro] 현재 데이터의 스테이지 개수: {SaveLoad.instance.currentData.bestScores.Count}");
            foreach (var key in SaveLoad.instance.currentData.bestScores.Keys)
            {
                Debug.Log($"[Intro] 저장된 키 목록: {key}");
            }
        }


        bool is1_1Cleared = SaveLoad.instance != null
         && SaveLoad.instance.currentData != null
         && SaveLoad.instance.currentData.bestScores.ContainsKey("1-1");


        if (SaveLoad.instance != null && SaveLoad.instance.currentData != null)
        {
            is1_1Cleared = SaveLoad.instance.currentData.bestScores.ContainsKey("1-1");
        }

        if (is1_1Cleared)
        {
            Debug.Log("[Intro] 1-1 이미 클리어됨 로그 확인!");

            // 1-1이 클리어되었다면 연출 스킵: 즉시 버스 위치로
            _mapCamera.enabled = true; // 카메라 기능 활성화
            _mapCamera.transform.position = _playerTransform.position + _introOffset;
            _mapCamera.SetTarget(_playerTransform, _introOffset);

            BusMove player = _playerTransform.GetComponent<BusMove>();
            if (player != null)
            {
                player.CanMove = true;
            }
            _stage1_1Open = true;

            yield return new WaitForSeconds(0.6f);

            if (_stageController != null)
            {
                Debug.Log("[Intro] StageController.CheckNewStageUnlockPublic() 호출");
                _stageController.CheckNewStageUnlockPublic();
            }
            else
            {
                Debug.LogError("[Intro] _stageController가 null입니다! 인스펙터에서 연결해주세요.");
            }

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