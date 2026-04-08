using Overcooked;
using Overcooked.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public abstract class Cookware : MonoBehaviour
{
    [Header("세팅")]
    [SerializeField] protected float maxCookTime = 5f;
    [SerializeField] protected GameObject _contentObject;

    protected List<Ingredient> currentIngredients = new List<Ingredient>();
    protected float currentCookTime = 0f;
    protected bool _isOnStove = false;

    public bool HasIngredients => currentIngredients.Count > 0;

    public float CurrentCookTime => currentCookTime;
    public float MaxCookTime => maxCookTime;
    public bool IsCooked => currentCookTime >= maxCookTime;

    public void SetOnStove(bool state)
    {
        _isOnStove = state;
    }

    public void CookTick(float deltaTime, RecipeManager recipeManager)
    {
        if (!_isOnStove)
        {
            return;
        }

        currentCookTime += deltaTime;

        if (currentCookTime >= maxCookTime)
        {
            currentCookTime = maxCookTime;
            FinishCooking(recipeManager);
        }
    }

    public abstract bool TryAddIngredient(Ingredient ingredient);

    protected virtual void FinishCooking(RecipeManager recipeManager)
    {
        if (recipeManager == null)
        {
            return;
        }

        HashSet<IngreDientData> mixSet = new HashSet<IngreDientData>();

        foreach (var ing in currentIngredients)
        {
            mixSet.Add(ing.GetIngredientData());
        }

        GameObject cookedModel = recipeManager.GetRecipeModel(mixSet);

        if (cookedModel != null)
        {
            Debug.Log("boil 완성");
        }
    }

    public void GiveFoodToPlate(Dish plate)
    {
        if (!IsCooked)
        {
            return;
        }

        List<IngreDientData> dataList = new List<IngreDientData>();

        foreach (var ing in currentIngredients)
        {
            dataList.Add(ing.GetIngredientData());
        }

        if (plate.AddCookedRecipe(dataList))
        {
            Debug.Log("음식 접시에 전달");
            ClearCookware();
        }

        else
        {
            Debug.Log("접시에 음식 전달 실패");
        }
    }

    public void ClearCookware()
    {
        foreach(var ing in currentIngredients)
        {
            if (ing != null)
            {
                Destroy(ing.gameObject);
            }
        }

        currentIngredients.Clear();
        currentCookTime = 0f;

        if (_contentObject != null)
        {
            _contentObject.SetActive(false);
        }
    }

    public List<IngreDientData> GetIngredientDataList()
    {
        List<IngreDientData> dataList = new List<IngreDientData>();

        foreach (var ing in currentIngredients)
        {
            dataList.Add(ing.GetIngredientData());
        }

        return dataList;
    }

    protected void ApllyCookingStatus(CookBehaivior status)
    {
        foreach (var ingredient in currentIngredients)
        {
            if (ingredient != null)
            {
                ingredient.AddStatus(status);
            }
        }
    }
}
