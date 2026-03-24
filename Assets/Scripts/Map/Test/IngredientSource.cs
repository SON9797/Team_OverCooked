using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientSource : MonoBehaviour
{
    [SerializeField] private GameObject _ingredientPrefab;

    // 플레이어가 재료를 꺼낼 때 호출할 함수
    public GameObject SpawnIngredient()
    {
        GameObject newIng = Instantiate(_ingredientPrefab);

        return newIng;

    }
}
