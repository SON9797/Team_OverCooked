using System;
using System.Collections.Generic;
using Overcooked.Interfaces;
using UnityEngine;

public class RecipeManager : IRecipeService
{
    // 1. 레시피 데이터들을 담아둘 리스트 (인스펙터 연동용 혹은 데이터 로드용)
    private readonly List<RecipeData> _allRecipes;
    private readonly List<RecipeData> _currentOrders = new List<RecipeData>();

    public IReadOnlyList<RecipeData> CurrentOrders => _currentOrders;

    public Action<RecipeData> OnOrderAdded { get; set; }
    public Action<int> OnOrderCompleted { get; set; }

    // 생성자를 통해 전체 레시피 리스트를 주입받을 수 있습니다 (VContainer 활용)
    public RecipeManager(List<RecipeData> allRecipes)
    {
        _allRecipes = allRecipes;
    }

    public void AddRandomOrder()
    {
        if (_allRecipes == null || _allRecipes.Count == 0)
        {
            Debug.LogWarning("등록된 레시피 데이터가 없습니다");
            return;
        }

        // 2. 랜덤하게 하나 뽑기
        int randomIndex = UnityEngine.Random.Range(0, _allRecipes.Count);
        RecipeData newOrder = _allRecipes[randomIndex];

        // 3. 현재 주문 리스트에 추가 및 이벤트 발생
        _currentOrders.Add(newOrder);
        OnOrderAdded?.Invoke(newOrder);
    }

    public void CompleteOrder(int orderIndex)
    {
        if (orderIndex >= 0 && orderIndex < _currentOrders.Count)
        {
            // 4. 주문 처리 완료
            _currentOrders.RemoveAt(orderIndex);
            OnOrderCompleted?.Invoke(orderIndex);
        }
    }
}