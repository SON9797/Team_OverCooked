using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCan : ItemPlaceAndTake
{
    [Header("쓰레기통 연출 설정")]
    [SerializeField] private float _shrinkSpeed = 3f;     // 작아지는 속도
    [SerializeField] private float _rotateSpeed = 720f;   // 회전 속도 (초당 720도)

    public override bool CanPlaceItem() => true;

    public override void PlaceItem(GameObject item)
    {
        // 1. 플레이어의 손에서 부모 연결을 끊어줍니다. (플레이어가 이동해도 아이템은 쓰레기통에 남도록)
        item.transform.SetParent(null);

        // 2. 애니메이션 연출 시작
        StartCoroutine(ShrinkAndRotateEffect(item));

        // 3. 쓰레기통 상태는 항상 비어있게 유지
        _onCounterItem = null;
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