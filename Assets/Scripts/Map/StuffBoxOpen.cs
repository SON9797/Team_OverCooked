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
            if (_playerController != null && _playerController.GetCurrentHeldObject() == null)
            {
                CheckForBox();
            }
        }
    }
    private void CheckForBox()
    {
        if (_rayPoint == null || _placeAndTakeComponent == null)
        {
            return;
        }

        if (_placeAndTakeComponent.HasItem)
        {
            return;
        }

        RaycastHit hit;
        
        if (Physics.Raycast(_rayPoint.position, _rayPoint.forward, out hit, _interactionDistance))
        {
            if (hit.transform == this.transform)
            {
                _animator.SetBool("Open", true);
                Debug.Log("상자를 열었습니다!");
            }
        }
    }

}
