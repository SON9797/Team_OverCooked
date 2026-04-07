using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController : MonoBehaviour
{
    public WorldMapManager _tileManager;
    public Transform[] _stageFlagTransform; // 현재 스테이지 깃발 위치
    private int flag = 0;

    private void Update()
    {
        if( Input.GetKeyDown(KeyCode.Space) )
        {
            OnStageClear();
        }
    }
    //스테이지 클리어시 호출
    public void OnStageClear()
    {
        _tileManager.StartConditionalWave(_stageFlagTransform[flag].position, 5.0f);
        flag++;

        Debug.Log("새로운 길이 열렸습니다!");
    }
}
