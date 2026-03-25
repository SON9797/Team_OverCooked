using Overcooked.Interfaces;
using OverCooked;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;


public class ScoreManager : MonoBehaviour, IScoreService
{
    private OrderManager _orderManager;
    public int CurrentScore { get; private set; }

    public Action<int> OnScoreChanged { get; set; }

    [Inject]
    public void Construct(OrderManager orderManager)
    {
        _orderManager = orderManager;
    }

    public void OnPlaySubmitItem(SubmittedDish item)
    {
        if (_orderManager.TrySubmitDish(item, out int earnedScore))
        {
            AddScore(earnedScore);
        }

        else
        {
            Debug.Log("잘못된 주문");
        }
    }

    public void AddScore(int amount)
    {
        CurrentScore += amount;

        // 점수가 변했다고 UI 에게 알림
        OnScoreChanged?.Invoke(CurrentScore);
    }
}

