using Overcooked.Interfaces;
using OverCooked;
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
        private OrderManager _orderManager;
        private LevelData _levelData;

        private List<RecipeUI> _activeUIs = new List<RecipeUI>();


        [Inject]
        public void Construct(IRecipeService recipeService, OrderManager orderManager, LevelData levelData)
        {
            _recipeService = recipeService;
            _orderManager = orderManager;
            _levelData = levelData;

            _recipeService.OnOrderAdded += CreateOrderUI;
            _recipeService.OnOrderCompleted += OnOrderCompletedFromService;
        }

        private void CreateOrderUI(RecipeData data)
        {
            GameObject obj = Instantiate(_recipeUIPrefab, _contentParent);
            var ui = obj.GetComponent<RecipeUI>();
            ui.Setup(data, _levelData.recipeTimer);
            ui.OnTimeOut += HandleTimeOut;

            _activeUIs.Add(ui);
        }

        private void OnOrderCompletedFromService(int index)
        {
            if (index >= 0 && index < _activeUIs.Count)
            {
                RecipeUI targetUI = _activeUIs[index];

                _activeUIs.RemoveAt(index);

                StartCoroutine(PlaySuccessAndRemoveData(targetUI));
            }

        }

        private IEnumerator PlaySuccessAndRemoveData(RecipeUI ui)
        {
            RecipeData successData = ui.CurrentRecipeData;

            yield return StartCoroutine(ui.PlaySuccessEffect(() =>
            {
                Destroy(ui.gameObject);
            }));

            if (_recipeService is RecipeManager manager)
            {
                manager.RemoveCompletedOrder(successData);
            }
        }

        private void HandleTimeOut(RecipeUI ui)
        {
            ui.OnTimeOut -= HandleTimeOut;

            _orderManager.HandleOrderTimeOut(ui.CurrentRecipeData);

            StartCoroutine(PlayFailAndRemoveData(ui));
        }

        private IEnumerator PlayFailAndRemoveData(RecipeUI ui)
        {
            RecipeData failedData = ui.CurrentRecipeData;

            yield return StartCoroutine(ui.PlayFailEffect(() =>
            {
                Destroy(ui.gameObject);
            }));

            _recipeService.RemoveFailedOrder(failedData);
        }

        private void OnDestroy()
        {
            if (_recipeService != null)
            {
                _recipeService.OnOrderAdded -= CreateOrderUI;
                _recipeService.OnOrderCompleted -= OnOrderCompletedFromService;
            }
        }
    }
}
