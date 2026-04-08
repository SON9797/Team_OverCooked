using Overcooked;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingPot : Cookware
{
    [Header("ºñÁê¾ó ¼³Á¤")]
    [SerializeField] private ParticleSystem _cookingParticle;

    private void Start()
    {
        if (_contentObject != null)
        {
            _contentObject.SetActive(false);
        }

        if (_cookingParticle != null)
        {
            _cookingParticle.Stop();
        }

    }

    public override bool TryAddIngredient(Ingredient newIngredient)
    {
        if (IsCooked)
        {
            return false;
        }

        IngreDientData data = newIngredient.GetIngredientData();

        if (data.kind != IngreDientKind.rice)
        {
            return false;
        }

        if (currentIngredients.Count > 0)
        {
            return false;
        }

        currentIngredients.Add(newIngredient);
        newIngredient.gameObject.SetActive(false);
        newIngredient.transform.SetParent(this.transform);

        if (_contentObject != null)
        {
            _contentObject.SetActive(true);
        }

        if (_isOnStove && _cookingParticle != null)
        {
            _cookingParticle.Play();
        }

        return true;
    }

    protected override void FinishCooking(RecipeManager recipeManager)
    {
        foreach (var ing in currentIngredients)
        {
            if (ing != null)
            {
                ing.AddStatus(CookBehaivior.boil);
            }
        }

        base.FinishCooking(recipeManager);

        if (_cookingParticle != null)
        {
            _cookingParticle.Stop();
        }
    }
}
