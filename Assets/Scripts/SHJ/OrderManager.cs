using Overcooked;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OverCooked
{
    public struct SubmittedDish
    {
        public string DishName;
    }

    public class OrderManager : MonoBehaviour
    {
        public LevelData _CurrentLevelData;

        private List<RecipeData> _activeOrders = new List<RecipeData>();

        public bool TrySubmitDish(SubmittedDish submittedDish, out int score)
        {
            score = 0;

            for (int i = 0; i < _activeOrders.Count; i++)
            {
                if (_activeOrders[i].DishName == submittedDish.DishName)
                {
                    score = _activeOrders[i].BaseScore;

                    CompleteOrder(i);
                    return true;
                }
            }

            return false;
        }

        private void CompleteOrder(int orderIndex)
        {
            _activeOrders.RemoveAt(orderIndex);
        }
    }
}
