using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;
using Overcooked.Interfaces;

public class GameLoopManager : IStartable, ITickable
{
    private readonly ITimerService _timer;
    private readonly IScoreService _score;

    public GameLoopManager(ITimerService timer, IScoreService score)
    {
        _timer = timer;
        _score = score;
    }

    public void Start()
    {
        _timer.Initialize(180f);        // 1. 스테이지 시간 설정 (예: 180초)
        
        _timer.StartTimer();            // 2. 타이머 시작

        // 3. (나중에 추가) 카운트다운 연출 시작 등
    }


    public void Tick()
    {
        if (_timer.IsTimeOver)
        {
            EndGame();
        }

        // 매 프레임 체크해야 할 게임 승리/패배 조건
    }

    private void EndGame()
    {
        // 게임 종료 로직 (예: 결과창 띄우기 요청 등)

        _timer.StopTimer();         // 중복 실행 방지를 위해 타이머 정지 등 후속 처리
    }
}
