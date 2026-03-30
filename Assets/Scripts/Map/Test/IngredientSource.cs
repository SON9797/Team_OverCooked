using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientSource : MonoBehaviour
{
    [SerializeField] private GameObject _ingredientPrefab;

    private Animator _animator;
    private ItemPlaceAndTake _placeAndTake;

    void Start()
    {
        // 상자의 애니메이터를 가져옵니다.
        _animator = GetComponent<Animator>();
        _placeAndTake = GetComponent<ItemPlaceAndTake>();
    }

    public GameObject SpawnIngredient()
    {
        if (_animator != null)
        {
            bool isOpen = _animator.GetBool("Open");

            // 상자가 열려 있다면(isOpen == true) 재료를 생성하지 않고 null을 반환합니다.
            if (isOpen)
            {
                GameObject newIng = Instantiate(_ingredientPrefab);
                return newIng;
            }
            if (_placeAndTake != null && _placeAndTake.HasItem)
            {
                Debug.Log("상자 위에 물건이 있어 재료를 꺼낼 수 없습니다.");
                return null;
            }
        }
        Debug.Log($"{gameObject.name}이(가) 닫혀 있어서 재료를 꺼낼 수 없습니다.");
        return null;
        // 상자가 닫혀 있을 때만 재료 생성

    }
}
