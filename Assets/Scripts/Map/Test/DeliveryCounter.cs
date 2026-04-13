using Overcooked;
using Overcooked.Interfaces;
using OverCooked;
using System.Collections;
using UnityEngine;
using VContainer;

public class DeliveryCounter : ItemPlaceAndTake
{
    [SerializeField] private float _deliveryDelay = 0.5f;
    [SerializeField] private PlateRespawn _plateSpawner;

    private ScoreManager _scoreManager;
    private RecipeManager _recipeManager;

    [Inject]
    public void Construct(ScoreManager scoreManager, IRecipeService recipeService)
    {
        _scoreManager = scoreManager;
        _recipeManager = (RecipeManager)recipeService;
    }

    // 완성된 음식이 올라오면 제출 처리
    public override bool PlaceItem(GameObject item)
    {
        Dish dish = item.GetComponent<Dish>();

        if (dish != null && dish.GetRecipe().Count > 0)
        {
            var ingredients = dish.GetRecipe();
            string dishName = _recipeManager.GetDishNameByIngredients(ingredients);

            if (!string.IsNullOrEmpty(dishName))
            {
                SubmittedDish submitted = new SubmittedDish { DishName = dishName };
                _scoreManager.OnPlaySubmitItem(submitted);

                Debug.Log($"{dishName}");
            }
            else
            {
                Debug.Log("레시피 목록에 없음");
            }

            base.PlaceItem(item);
            StartCoroutine(ClearDishAfterDelay(item));

            return true;
        }
        else
        {
            Debug.Log("접시만 있음 제출실패");
            return false;
        }
    }

    // 제출 후 일정 시간 뒤 접시를 제거하고 리스폰 쪽에 알림
    private IEnumerator ClearDishAfterDelay(GameObject dishObj)
    {
        yield return new WaitForSeconds(_deliveryDelay);

        Debug.Log($"[DeliveryCounter:{name}] plateSpawner = {(_plateSpawner != null ? _plateSpawner.name : "NULL")}");

        if (_plateSpawner != null)
        {
            _plateSpawner.OnPlateDestroyed(dishObj);
        }

        Destroy(dishObj);
        _onCounterItem = null;
    }
}