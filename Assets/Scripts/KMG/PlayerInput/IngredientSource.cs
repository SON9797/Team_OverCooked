using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientSource : MonoBehaviour
{
    [SerializeField] private GameObject _ingredientPrefab;

    public GameObject SpawnIngredient()
    {
        if (_ingredientPrefab == null) return null;

        GameObject newIng = Instantiate(_ingredientPrefab, transform.position, Quaternion.identity);
        return newIng;
    }
}
