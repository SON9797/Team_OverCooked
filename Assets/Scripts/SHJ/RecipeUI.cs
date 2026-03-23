using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Overcooked
{
    public class RecipeUI : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Image _dishImage;              // 완성 이미지
        [SerializeField] private Transform _ingredientParent;   // 아이콘이 들어갈 부모
        [SerializeField] private GameObject _ingredientPrefab;  // 재료 아이콘

        public void Setup(RecipeData data)
        {
            // 메인 요리 이미지 설정
            _dishImage.sprite = data.FinishedDishImage;

            foreach (Transform child in _ingredientParent)
            {
                Destroy(child.gameObject);
            }

            foreach (var icon in data.Ingredients)
            {
                GameObject obj = Instantiate(_ingredientPrefab, _ingredientParent);
                obj.GetComponent<Image>().sprite = icon;
            }
        }
    }
}
