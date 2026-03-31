using Overcooked;
using Overcooked.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StuffBoxOpen : MonoBehaviour
{
    private ItemPlaceAndTake _placeAndTake;

    [SerializeField] private Animator _animator;
    [SerializeField] Transform _rayPoint;
    [SerializeField] private float _interactionDistance = 2.0f; // 상호작용 가능 거리

    [SerializeField] private PlayerItemController _playerController;

    private ItemPlaceAndTake _placeAndTakeComponent;

    private int _originalLayer;

    void Start()
    {
        // 만약 에디터에서 할당하지 않았다면 자동으로 가져옴
        if (_animator == null)
            _animator = GetComponent<Animator>();

        _placeAndTakeComponent = GetComponent<ItemPlaceAndTake>();

        _originalLayer = gameObject.layer;
    }
    private void Update()
    {
        if (_placeAndTakeComponent != null)
        {
            gameObject.layer = _placeAndTakeComponent.HasItem
                ? LayerMask.NameToLayer("Ignore Raycast")
                : _originalLayer;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TryOpenBox();
            }
        }
    }

    private void TryOpenBox()
    {
        // 주변의 모든 플레이어 레이어를 가진 콜라이더를 찾습니다.
        // "Player" 레이어가 설정되어 있어야 합니다.
        Collider[] hitPlayers = Physics.OverlapSphere(transform.position, _interactionDistance, LayerMask.GetMask("Player"));

        foreach (var playerCol in hitPlayers)
        {
            PlayerItemController controller = playerCol.GetComponent<PlayerItemController>();

            // 이 플레이어가 현재 선택된(조종 중인) 플레이어인지 확인
            // (InGameInputInjector의 IsSelected 활용)
            if (controller != null && controller.IsSelectedPlayer())
            {
                // 플레이어가 아무것도 들고 있지 않을 때만 상자가 열리도록 설정
                if (controller.GetCurrentHeldObject() == null)
                {
                    CheckForBoxFromPlayer(controller);
                }
            }
        }
    }

    private void CheckForBoxFromPlayer(PlayerItemController player)
    {
        if (_placeAndTakeComponent == null || _placeAndTakeComponent.HasItem) return;

        // 플레이어의 RayPoint 방향으로 상자가 있는지 최종 확인
        RaycastHit hit;
        Transform pRay = player.GetRayPoint(); // 플레이어의 레이지점 가져오기

        if (Physics.Raycast(pRay.position, pRay.forward, out hit, _interactionDistance))
        {
            if (hit.transform == this.transform || hit.transform.IsChildOf(this.transform))
            {
                _animator.SetBool("Open", true);
                Debug.Log($"{player.gameObject.name}가 상자를 열었습니다!");
            }
        }
    }
}
