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

    public bool _stage1_1Open = false;
    private void Start()
    {
        StartCoroutine(IntroSequence());
    }

    private IEnumerator IntroSequence()
    {
        bool isAlreadyCleared = false;
        if (SaveLoad.instance != null && SaveLoad.instance.currentData.bestScores.ContainsKey("1-1"))
        {
            isAlreadyCleared = true;
        }

        if (_stage1_1Open)
        {
            yield break;
        }    
        _mapCamera.enabled = false;

        if (isAlreadyCleared)
        {
            // 1-1이 이미 클리어되었다면 연출 전체를 스킵하고 카메라만 바로 세팅
            _mapCamera.transform.position = _playerTransform.position + _introOffset;
            _mapCamera.SetTarget(_playerTransform, _introOffset);
            _stage1_1Open = true;
            yield break;
        }
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

        BusMove playerBus = _playerTransform.GetComponent<BusMove>();
        if (playerBus != null)
        {
            playerBus.CanMove = true;
        }

    }
}