using Overcooked;
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
    private IInGameSoundManager _inGameSoundManager;

    public int DeliveryOrderCount {  get; private set; }
    public int DeliveryOrderScore {  get; private set; }
    public int TotalTips {  get; private set; }
    public int FailedOrderCount {  get; private set; }
    public int FailedOrderPenalty {  get; private set; }
    public int CurrentCombo {  get; private set; }

    public int CurrentScore => DeliveryOrderScore + TotalTips - FailedOrderPenalty;
    public Action<int> OnScoreChanged { get; set; }

    [Inject]
    public void Construct(OrderManager orderManager, IUIManager uIManager, IInGameSoundManager inGameSoundManager)
    {
        _orderManager = orderManager;
        _uiManager = uIManager;
        _inGameSoundManager = inGameSoundManager;
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
                _inGameSoundManager.PlaySFX(SFXType.SuccessDelivery);
                _uiManager.ShowTipEffect(tip);
            }
        }

        else
        {        
            if (item.OriginalRecipe != null)
            {
                HandleFailedOrder(item.OriginalRecipe.BaseScore);
            }
        }
    }

    public void HandleFailedOrder(int penaltyScore)
    {
        FailedOrderCount++;
        FailedOrderPenalty += penaltyScore;

        UpdateScoreUI();
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

    public void SaveBestScore(LevelData levelData)
    {
        SaveLoad saveload = SaveLoad.instance;
        
        string levelIdentifier = $"{levelData.Chapter}-{levelData.Stage}";
        string scoreKey = $"BestScore_{levelIdentifier}";
        string starKey = $"BestStar_{levelIdentifier}";

        int previousBest = PlayerPrefs.GetInt(scoreKey, 0);

        bool isFirstClear = previousBest == 0;

        if (CurrentScore > previousBest)
        {
            PlayerPrefs.SetInt(scoreKey, CurrentScore);

            int starCount = 0;
            if (CurrentScore >= levelData.ThreeStar)
            {
                starCount = 3;
            }

            else if (CurrentScore >= levelData.TwoStar)
            {
                starCount = 2;
            }

            else if (CurrentScore >= levelData.OneStar)
            {
                starCount = 1;
            }

            PlayerPrefs.SetInt(starKey, starCount);
            //수정
            string nextStageKey = GetNextStageKey(levelData.Chapter, levelData.Stage);
            PlayerPrefs.SetString("PendingUnlockStage", nextStageKey);
            Debug.Log($"[ScoreManager] 다음 스테이지 해금 예약: {nextStageKey}");
            //수정

            PlayerPrefs.Save();
            saveload.CurrentDataUpdate(levelData.Chapter, levelData.Stage, CurrentScore, starCount);
            saveload.AutoSave();
        }

        if (isFirstClear && CurrentScore > 0)
        {
            string nextStageKey = $"{levelData.Chapter}-{levelData.Stage + 1}";
            PlayerPrefs.SetString("PendingUnlockStage", nextStageKey);
            PlayerPrefs.Save();
        }
        

    }
    private string GetNextStageKey(int mainChapter, int subChapter)
    {
        return $"{mainChapter}-{subChapter + 1}";
    }
}

