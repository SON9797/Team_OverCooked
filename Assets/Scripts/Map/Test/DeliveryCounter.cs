using Overcooked;
using Overcooked.Interfaces;
using OverCooked;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class DeliveryCounter : ItemPlaceAndTake
{
    [SerializeField] private float _deliveryDelay = 0.5f;

    [SerializeField] private PlateReSpawn _plateSpawner;

    private ScoreManager _scoreManager;
    private RecipeManager _recipeManager;

    [Inject]
    public void Construct(ScoreManager scoreManager, IRecipeService recipeService)
    {
        _scoreManager = scoreManager;
        _recipeManager = (RecipeManager)recipeService;
    }

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
                // UI
            }

            else
            {
                Debug.Log("레시피 목록에 없음");
                // UI
            }

                base.PlaceItem(item);
            StartCoroutine(ClearDishAfterDelay(item));

            return true;
        }

        return false;
    }


    private IEnumerator ClearDishAfterDelay(GameObject dishObj)
    {
        yield return new WaitForSeconds(_deliveryDelay);

        if (_plateSpawner != null)
        {
            _plateSpawner.OnPlateDestroyed(dishObj);
        }
        Destroy(dishObj);
        _onCounterItem = null;

    }
}
