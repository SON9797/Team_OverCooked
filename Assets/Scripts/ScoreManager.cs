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
    private IUIManager _uiManager;

    public int DeliveryOrderCount {  get; private set; }
    public int DeliveryOrderScore {  get; private set; }
    public int TotalTips {  get; private set; }
    public int FailedOrderCount {  get; private set; }
    public int FailedOrderPenalty {  get; private set; }

    public int CurrentScore => DeliveryOrderScore + TotalTips - FailedOrderPenalty;
    public Action<int> OnScoreChanged { get; set; }

    [Inject]
    public void Construct(OrderManager orderManager, IUIManager uIManager)
    {
        _orderManager = orderManager;
        _uiManager = uIManager;
    }

    public void OnPlaySubmitItem(SubmittedDish item)
    {
        if (_orderManager.TrySubmitDish(item, out int earnedScore, out int tip))
        {
            DeliveryOrderCount++;
            DeliveryOrderScore += earnedScore;
            TotalTips += tip;

            UpdateScoreUI();

            if (tip > 0)
            {
                _uiManager.ShowTipEffect(tip);
            }
        }

        else
        {
            Debug.Log("잘못된 주문");
            // 추후 패널티 추가
            // HandleFailedOrder();
        }
    }

    private void HandleFailedOrder(int penalty)
    {

    }

    private void UpdateScoreUI()
    {
        _uiManager.UpdateScoreText(CurrentScore);
        OnScoreChanged?.Invoke(CurrentScore);
    }

    public void ResetScore()
    {
        DeliveryOrderCount = 0;
        DeliveryOrderScore = 0;
        TotalTips = 0;
        FailedOrderCount = 0;
        FailedOrderPenalty = 0;

        UpdateScoreUI();
    }
}

