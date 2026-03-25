using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryCounter : ItemPlaceAndTake
{
    [SerializeField] private float _deliveryDelay = 0.5f;

    [SerializeField] private PlateReSpawn _plateSpawner;

    public override bool PlaceItem(GameObject item)
    {
        // 올린 아이템이 접시인지 확인
        Dish dish = item.GetComponent<Dish>();

        if (dish != null)
        {
            base.PlaceItem(item);

            StartCoroutine(ClearDishAfterDelay(item));

            return true;
        }
        else
        {
            Debug.Log("접시에 담긴 요리만 서빙할 수 있습니다!");

            return false;
        }
    }


    private IEnumerator ClearDishAfterDelay(GameObject dishObj)
    {
        yield return new WaitForSeconds(_deliveryDelay);

        if (_plateSpawner != null && _plateSpawner._spawnedPlate.Contains(dishObj))
        {
            _plateSpawner._spawnedPlate.Remove(dishObj);

        }
        Destroy(dishObj);
        _onCounterItem = null;

    }
}
