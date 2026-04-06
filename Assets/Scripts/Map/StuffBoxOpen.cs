using Overcooked;
using UnityEngine;

public class StuffBoxOpen : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private float _interactionDistance = 2.0f; // 플레이어가 앞에서 상자를 열 수 있는 거리

    private ItemPlaceAndTake _placeAndTakeComponent;
    private int _originalLayer;

    private void Start()
    {
        // 만약 에디터에서 할당하지 않았다면 자동으로 가져옴
        if (_animator == null)
            _animator = GetComponent<Animator>();

        // 상자 위에 아이템이 있는지 확인하기 위한 컴포넌트
        _placeAndTakeComponent = GetComponent<ItemPlaceAndTake>();

        // 원래 레이어 저장
        _originalLayer = gameObject.layer;
    }

    private void Update()
    {
        if (_placeAndTakeComponent != null)
        {
            // 상자 위에 아이템이 있으면 레이 대상에서 제외
            gameObject.layer = _placeAndTakeComponent.HasItem
                ? LayerMask.NameToLayer("Ignore Raycast")
                : _originalLayer;
        }
    }

    /// <summary>
    /// 선택된 플레이어가 현재 바라보는 방향으로 이 상자를 열 수 있는지 검사 후 열기
    /// </summary>
    public bool TryOpenByPlayer(PlayerItemController player)
    {
        if (player == null)
            return false;

        if (_placeAndTakeComponent == null)
            return false;

        // 이미 위에 아이템이 있으면 열지 않음
        if (_placeAndTakeComponent.HasItem)
            return false;

        // 플레이어가 무언가 들고 있으면 상자를 열지 않음
        if (player.GetCurrentHeldObject() != null)
            return false;

        // 플레이어의 레이 시작 지점 가져오기
        Transform pRay = player.GetRayPoint();
        if (pRay == null)
            return false;

        // 플레이어가 실제로 이 상자를 바라보고 있는지 최종 확인
        RaycastHit hit;
        if (Physics.Raycast(pRay.position, pRay.forward, out hit, _interactionDistance))
        {
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                _animator.SetBool("Open", true);
                Debug.Log($"{player.gameObject.name}가 상자를 열었습니다.");
                return true;
            }
        }

        return false;
    }
}