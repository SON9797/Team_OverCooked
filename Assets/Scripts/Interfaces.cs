using System;
using System.Collections.Generic;
using UnityEngine;

namespace Overcooked.Interfaces
{
    // 점수 관련
    public interface IScoreService
    {
        int CurrentScore { get; }
        void AddScore(int amount);
        Action<int> OnScoreChanged { get; set; }
    }

    // 시간 관련
    public interface ITimerService
    {
        float RemainingTime { get; }
        bool IsTimeOver { get; }
        void Initialize(float duration);
        void StartTimer();
        void StopTimer();
        Action OnTimeOver { get; set; }
        Action<float> OnTimerTick { get; set; }
    }

    // 레시피 / 주문 관련
    public interface IRecipeService
    {
        IReadOnlyList<RecipeData> CurrentOrders { get; }
        void AddRandomOrder();
        void CompleteOrder(int orderIndex);
        Action<RecipeData> OnOrderAdded { get; set; }
        Action<int> OnOrderCompleted { get; set; }
    }

    // 플레이어
    public interface IPlayerInputService
    {
        // 이동 입력
        Vector2 GetMovementInput();
    
        // 상호작용 (잡기 / 놓기)
        bool IsInteractButtonDown();
    
        // 행동 입력 (썰기, 대쉬)
        bool IsActionButtonDown();
    }
    
    public interface IPlayerController
    {
        void SetMoveSpeed(float speed);
        void EnableControl(bool isEnabled);
    }

    // 맵
    public interface ILevelService
    {
        Transform GetPlayerSpawnPoint();
    }
}
