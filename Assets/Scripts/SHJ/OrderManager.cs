using Overcooked;
using Overcooked.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace OverCooked
{
    public struct SubmittedDish
    {
        public string DishName;
    }

    public class OrderManager : MonoBehaviour
    {
        public LevelData _currentLevelData;
        private RecipeManager _recipeManager;
        private IUIManager _uiManager;

        private int _comboCount = 0;

        [Inject]
        public void Construct(IRecipeService recipeService, IUIManager uiManager)
        {
            _recipeManager = (RecipeManager)recipeService;
            _uiManager = uiManager;
        }

        public bool TrySubmitDish(SubmittedDish submittedDish, out int score)
        {
            score = 0;

            var currentOrders = _recipeManager.CurrentOrders;

            for (int i = 0; i < currentOrders.Count; i++)
            {
                if (currentOrders[i].DishName == submittedDish.DishName)
                {
                    int baseScore = currentOrders[i].BaseScore;
                    int baseTip = _currentLevelData.BaseTipAmount;

                    if (i == 0)
                    {
                        _comboCount = Mathf.Min(_comboCount + 1, 4);

                        int totalTip = baseTip * _comboCount;
                        score = baseScore + totalTip;

                        Debug.Log($"{_comboCount} / {totalTip}");

                        // UI ÄÞº¸
                    }

                    else
                    {
                        _comboCount = 0;
                        score = baseScore + _comboCount;

                        Debug.Log("ÄÞº¸ ÃÊ±âÈ­");
                    }

                    _uiManager.UpdateComboUI(_comboCount);

                    _recipeManager.CompleteOrder(i);
                    return true;
                }                              
            }

            _comboCount = 0;
            _uiManager.UpdateComboUI(_comboCount);
            return false;
        }
    }
}
