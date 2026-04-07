using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Cookware : MonoBehaviour
{
    [Header("세팅")]
    [SerializeField] protected float maxCookTime = 5f;

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

    public void CookTick(float deltaTime)
    {
        if (!_isOnStove)
        {
            return;
        }

        currentCookTime += deltaTime;

        if (currentCookTime >= maxCookTime)
        {
            currentCookTime = maxCookTime;
            FinishCooking();
        }
    }

    public abstract bool TryAddIngredient(Ingredient ingredient);

    protected virtual void FinishCooking()
    {
        // 요리 완성 로직
        Debug.Log("요리가 완성되었습니다!");
    }
}
