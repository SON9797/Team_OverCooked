using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
{
    public WorldMapManager _tileManager;
    public Transform[] _stageFlagTransform; // 현재 스테이지 깃발 위치

    // 스테이지 클리어 시 호출될 함수
    private void Update()
    {
        if( Input.GetKeyDown(KeyCode.Space) )
        {
            OnStageClear();
        }
    }
    public void OnStageClear()
    {
        int flag = 0;
        // 깃발 위치를 중심으로 반경 10유닛 이내의 타일만 뒤집기
        _tileManager.StartConditionalWave(_stageFlagTransform[flag].position, 5.0f);
        flag++;

        Debug.Log("새로운 길이 열렸습니다!");
    }
}
