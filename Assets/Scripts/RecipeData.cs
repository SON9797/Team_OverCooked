using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Overcooked/Recipe")]
public class RecipeData : ScriptableObject
{
    public string _recipeName;                          
    public Sprite _recipeIcon;
    public HashSet<IngreDientData> _requiredIngredients;       // 재료 리스트
    public GameObject resultPrefab;
    public int scorePoint;
    public float timeLimit;
}