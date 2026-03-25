using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCan : ItemPlaceAndTake
{
    [Header("쓰레기통 연출 설정")]
    [SerializeField] private float _shrinkSpeed = 3f;     // 작아지는 속도
    [SerializeField] private float _rotateSpeed = 720f;   // 회전 속도 (초당 720도)

    public override bool CanPlaceItem() => true;

    public override bool PlaceItem(GameObject item)
    {
        Dish dish = item.GetComponent<Dish>();

        if (dish != null)
        {
            // [수정] 접시 위에 음식이 있는지 확인 (mix가 비어있지 않다면)
            if (dish.GetRecipy().Count > 0)
            {
                Debug.Log("접시의 음식을 비웁니다!");
                dish.ClearDish(); 

                return false; // false를 반환하여 플레이어가 접시를 계속 들고 있게 합니다.
            }
            else
            {
                Debug.Log("이미 비어있는 접시입니다.");
                return false;
            }
        }

        item.transform.SetParent(null);

        // 애니메이션 연출
        StartCoroutine(ShrinkAndRotateEffect(item));

        // 쓰레기통 비우기
        _onCounterItem = null;

        return true;
    }

    private IEnumerator ShrinkAndRotateEffect(GameObject item)
    {
        if (item == null)
        {
            yield break;
        }

        if (item.TryGetComponent<Collider>(out Collider col))
        {
            col.enabled = false;
        }

        Vector3 targetPos = _snapPoint != null ? _snapPoint.position : transform.position;

        while (item != null && item.transform.localScale.x > 0.01f)
        {
            // 회전
            item.transform.Rotate(Vector3.up * _rotateSpeed * Time.deltaTime);

            // 2. 크기 줄이기
            item.transform.localScale = Vector3.Lerp(item.transform.localScale, Vector3.zero, _shrinkSpeed * Time.deltaTime);

            yield return null; // 다음 프레임까지 대기
        }

        // 연출이 끝나면 진짜로 파괴
        if (item != null)
        {
            Destroy(item);
        }
    }

    public override GameObject TakeItem() => null;
}