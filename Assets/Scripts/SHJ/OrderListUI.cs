using Overcooked.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace Overcooked
{
    public class OrderListUI : MonoBehaviour
    {
        [SerializeField] private GameObject _recipeUIPrefab;        // UI 프리팹
        [SerializeField] private Transform _contentParent;          // 주문서들이 쌓일 곳

        private IRecipeService _recipeService;

        [Inject]
        public void Construct(IRecipeService recipeService)
        {
            _recipeService = recipeService;

            _recipeService.OnOrderAdded += CreateOrderUI;
            _recipeService.OnOrderCompleted += RemoveOrderUI;
        }

        private void CreateOrderUI(RecipeData data)
        {
            GameObject obj = Instantiate(_recipeUIPrefab, _contentParent);
            var ui = obj.GetComponent<RecipeUI>();
            ui.Setup(data);
        }

        private void RemoveOrderUI(int index)
        {
            if (index < _contentParent.childCount)
            {
                Destroy(_contentParent.GetChild(index).gameObject);
            }
        }

        private void OnDestroy()
        {
            if (_recipeService != null)
            {
                _recipeService.OnOrderAdded -= CreateOrderUI;
                _recipeService.OnOrderCompleted -= RemoveOrderUI;
            }
        }


    }
}
