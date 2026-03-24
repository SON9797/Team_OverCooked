using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;
using Overcooked.Interfaces;

public class TimerManager : ITimerService, ITickable
{
    public float RemainingTime { get; private set; }
    public bool IsTimeOver { get; private set; }
    public bool IsRunning { get; private set; }
    public Action OnTimeOver { get; set; }
    public Action<float> OnTimerTick { get; set; }

    public void Initialize(float duration)
    {
        RemainingTime = duration;
        IsTimeOver = false;
        IsRunning = false;
    }

    public void StartTimer() => IsRunning = true;
    public void StopTimer() => IsRunning = false;


    // VContainer가 매 프레임 호출해줌
    public void Tick()
    {
        if (!IsRunning || IsTimeOver) 
        { 
            return;
        }

        RemainingTime -= Time.deltaTime;

        OnTimerTick?.Invoke(RemainingTime);     // 매 프레임 남은 시간을 UI 등에 알려주기 위해 호출

        if (RemainingTime <= 0)
        {
            RemainingTime = 0;
            IsTimeOver = true;
            IsRunning = false;
            OnTimeOver?.Invoke(); // 시간 종료 이벤트 발생
        }
    }
}
