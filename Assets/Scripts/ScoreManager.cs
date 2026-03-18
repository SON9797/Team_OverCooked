using Overcooked.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : IScoreService
{
    public int CurrentScore { get; private set; }
    public Action<int> OnScoreChanged { get; set; }

    public void AddScore(int amount)
    {
        CurrentScore += amount;

        if (CurrentScore < 0)
        {
            CurrentScore = 0;
        }

        OnScoreChanged?.Invoke(CurrentScore);           // 점수가 변했다고 구독자(UI 등)에게 알림
    }

    public void ResetScore()
    {
        CurrentScore = 0;
        OnScoreChanged?.Invoke(CurrentScore);
    }
}
