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

    private bool _isDelivering = false;

    [Inject]
    public void Construct(ScoreManager scoreManager, IRecipeService recipeService)
    {
        _scoreManager = scoreManager;
        _recipeManager = (RecipeManager)recipeService;
    }

    public override bool PlaceItem(GameObject item)
    {
        if (_isDelivering)
            return false;

        if (item == null)
            return false;

        Dish dish = item.GetComponent<Dish>();

        if (dish == null || dish.GetRecipe().Count <= 0)
        {
            Debug.Log("접시만 있음 제출실패");
            return false;
        }

        var ingredients = dish.GetRecipe();
        string dishName = _recipeManager.GetDishNameByIngredients(ingredients);

        if (string.IsNullOrEmpty(dishName))
        {
            Debug.Log("레시피 목록에 없음");
            return false;
        }

        _isDelivering = true;

        SubmittedDish submitted = new SubmittedDish { DishName = dishName };
        _scoreManager.OnPlaySubmitItem(submitted);

        Debug.Log($"{dishName}");

        base.PlaceItem(item);

        DisableInteraction(item);

        StartCoroutine(ClearDishAfterDelay(item));

        return true;
    }

    public override GameObject TakeItem()
    {
        if (_isDelivering)
            return null;

        return base.TakeItem();
    }

    private void DisableInteraction(GameObject target)
    {
        if (target == null)
            return;

        int ignoreLayer = LayerMask.NameToLayer("Ignore Raycast");

        if (ignoreLayer != -1)
        {
            SetLayerRecursively(target, ignoreLayer);
        }

        Collider[] colliders = target.GetComponentsInChildren<Collider>();

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = false;
        }
    }

    private void SetLayerRecursively(GameObject target, int layer)
    {
        target.layer = layer;

        for (int i = 0; i < target.transform.childCount; i++)
        {
            SetLayerRecursively(target.transform.GetChild(i).gameObject, layer);
        }
    }

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
        _isDelivering = false;
    }
}