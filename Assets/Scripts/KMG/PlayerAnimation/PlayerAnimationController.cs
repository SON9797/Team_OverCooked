using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overcooked
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(PlayerItemController))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerAnimationController : MonoBehaviour
    {
        private static readonly int IsMoveHash = Animator.StringToHash("IsMove");
        private static readonly int HasItemHash = Animator.StringToHash("HasItem");
        private static readonly int IsChoppingHash = Animator.StringToHash("IsChopping");

        private Animator _animator;
        private PlayerItemController _itemController;
        private Rigidbody _rb;

        [SerializeField] private float _moveThreshold = 0.01f;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _itemController = GetComponent<PlayerItemController>();
            _rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            Vector3 flatVelocity = _rb.velocity;
            flatVelocity.y = 0f;

            bool isMove = flatVelocity.sqrMagnitude > _moveThreshold;
            bool hasItem = _itemController.HasIngredient;

            _animator.SetBool(IsMoveHash, isMove);
            _animator.SetBool(HasItemHash, hasItem);
        }

        public void SetChopping(bool isChopping)
        {
            if (isChopping && _itemController.HasIngredient)
            {
                _animator.SetBool(IsChoppingHash, false);
                return;
            }

            _animator.SetBool(IsChoppingHash, isChopping);
        }
    }
}
