using Overcooked.Interfaces;
using OverCooked;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IngreDientKind
{
    lettuce,
    fish,
    carrot,
    prawn,
    rice,
    seaweed,
    cucumber
}
[Serializable]
public struct IngreDientData
{
    public IngreDientKind kind;
    public CookBehaivior stat;
    public Sprite icon;
    public override bool Equals(object obj)
    {
        if (obj is IngreDientData other)
        {
            return kind == other.kind && stat == other.stat;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(kind, stat);
    }

}
namespace Overcooked
{
    public class RecipeManager : IRecipeService
    {
        // 1. 레시피 데이터들을 담아둘 리스트 (인스펙터 연동용 혹은 데이터 로드용)
        private readonly List<RecipeData> _allRecipes;
        private readonly List<RecipeData> _currentOrders = new List<RecipeData>();

        private readonly LevelData _levelData;
        private bool _isGeneration = false;

        public IReadOnlyList<RecipeData> CurrentOrders => _currentOrders;

        public Action<RecipeData> OnOrderAdded { get; set; }
        public Action<int> OnOrderCompleted { get; set; }

        // 생성자를 통해 전체 레시피 리스트를 주입받을 수 있습니다 (VContainer 활용)
        public RecipeManager(LevelData levelData)
        {
            _levelData = levelData;
            _allRecipes = levelData.Recipes;
        }

        public void StartGeneration(MonoBehaviour runner)
        {
            if (_isGeneration)
            {
                return;
            }

            _isGeneration = true;
            runner.StartCoroutine(OrderGenerationRoutine());
        }

        private IEnumerator OrderGenerationRoutine()
        {
            while (_isGeneration)
            {
                if (_currentOrders.Count < _levelData.MaxOrderCount)
                {
                    AddRandomOrder();
                }

                float waitTime = UnityEngine.Random.Range(5f, 10f);
                yield return new WaitForSeconds(waitTime);
            }
        }

        public void StopGeneration()
        {
            if (!_isGeneration)
            {
                return;
            }

            _isGeneration = false;

            for (int i = _currentOrders.Count - 1; i >= 0; i--)
            {
                OnOrderCompleted?.Invoke(i);
            }

            _currentOrders.Clear();
            Debug.Log("UI 제거");
        }

        public void AddRandomOrder()
        {
            if (_allRecipes == null || _allRecipes.Count == 0)
            {
                return;
            }

            if (_currentOrders.Count >= _levelData.MaxOrderCount)
            {
                return;
            }

            // 2. 랜덤하게 하나 뽑기
            var randomRecipe = _allRecipes[UnityEngine.Random.Range(0, _allRecipes.Count)];
            _currentOrders.Add(randomRecipe);

            OnOrderAdded?.Invoke(randomRecipe);
        }

        public void CompleteOrder(int orderIndex)
        {
            if (orderIndex >= 0 && orderIndex < _currentOrders.Count)
            {
                OnOrderCompleted?.Invoke(orderIndex);
            }
        }

        public void RemoveCompletedOrder(RecipeData recipeData)
        {
            if (_currentOrders.Contains(recipeData))
            {
                _currentOrders.Remove(recipeData);

                AddRandomOrder();
            }
        }

        public GameObject GetRecipyModel(HashSet<IngreDientData> mix)
        {
            Debug.Log("mix-----");
            foreach (IngreDientData a in mix)
            {

                Debug.Log($"{a.kind}, {a.stat}");
            }
            foreach (RecipeData r in _allRecipes)
            {

                Debug.Log("recipy-------");
                foreach (IngreDientData a in r.Ingredients)
                {
                    Debug.Log($"{a.kind},{a.stat}");

                }

                if (mix.SetEquals(r.Ingredients) && mix.Count == r.Ingredients.Count)
                {
                    return r.model;
                }
            }
            return null;
        }

        public string GetDishNameByIngredients(HashSet<IngreDientData> submittedIngredients)
        {
            foreach (RecipeData recipe in _allRecipes)
            {
                if (recipe.Ingredients.Count != submittedIngredients.Count)
                {
                    continue;
                }

                HashSet<IngreDientData> recipeIngredients = new HashSet<IngreDientData>(recipe.Ingredients);

                if (recipeIngredients.SetEquals(submittedIngredients))
                {
                    return recipe.DishName;
                }
            }

            return null;
        }

        public void RemoveFailedOrder(RecipeData failedData)
        {
            if (_currentOrders.Contains(failedData))
            {
                _currentOrders.Remove(failedData);
            }
        }
    }
}