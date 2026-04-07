using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingPot : Cookware
{
    [Header("ºñÁê¾ó ¼³Á¤")]
    [SerializeField] private GameObject _contentObject;
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

        if (_contentObject != null)
        {
            _contentObject.SetActive(true);
        }

        if (_isOnStove && _cookingParticle != null)
        {
            _cookingParticle.Play();
        }

        Destroy(newIngredient.gameObject);

        return true;
    }
}
