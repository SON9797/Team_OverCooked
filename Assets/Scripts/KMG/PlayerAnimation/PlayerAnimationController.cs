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

        // 아이템던지기 - 실제 던지기 트리거만 사용
        private static readonly int ThrowHash = Animator.StringToHash("Throw");

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

            bool isMove = flatVelocity.sqrMagnitude > _moveThreshold * _moveThreshold;
            bool hasItem = _itemController.HasIngredient;

            _animator.SetBool(IsMoveHash, isMove);
            _animator.SetBool(HasItemHash, hasItem);

            // 이동 중이거나 아이템을 들고 있으면 칼질 애니메이션 강제 종료
            if (isMove || hasItem)
            {
                _animator.SetBool(IsChoppingHash, false);
            }
        }

        public void SetChopping(bool isChopping)
        {
            Vector3 flatVelocity = _rb.velocity;
            flatVelocity.y = 0f;

            bool isMove = flatVelocity.sqrMagnitude > _moveThreshold * _moveThreshold;
            bool hasItem = _itemController.HasIngredient;

            if (isChopping && (hasItem || isMove))
            {
                _animator.SetBool(IsChoppingHash, false);
                return;
            }

            _animator.SetBool(IsChoppingHash, isChopping);
        }

        // 아이템던지기 - 실제 던지기 트리거
        public void PlayThrow()
        {
            _animator.SetTrigger(ThrowHash);
        }
    }
}