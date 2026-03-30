using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OverCooked
{
    [CreateAssetMenu(fileName = "Recipe_", menuName = "Overcooked/Recipe Data", order = 2)]
    public class RecipeData : ScriptableObject
    {
        public string DishName;
        public Sprite FinishedDishImage;
        public List<IngreDientData> Ingredients;
        public int BaseScore;
        public GameObject model;
    }
}
