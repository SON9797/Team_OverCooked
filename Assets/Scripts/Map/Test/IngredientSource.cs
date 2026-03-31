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
        if (_animator != null && _animator.GetBool("Open"))
        {
            if (_placeAndTake != null && _placeAndTake.HasItem) return null;

            GameObject newIng = Instantiate(_ingredientPrefab);

            // [추가] 생성된 아이템의 레이어를 플레이어가 잡을 수 있는 'Default'로 강제 설정
            newIng.layer = LayerMask.NameToLayer("Default");

            // 만약 자식 객체들도 있다면 모두 바꿔줘야 합니다.
            foreach (Transform child in newIng.GetComponentsInChildren<Transform>())
            {
                child.gameObject.layer = LayerMask.NameToLayer("Default");
            }

            return newIng;
        }
        return null;
    }
}
