using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;


namespace Overcooked
{
    
            [RequireComponent(typeof(ApplyInGamePlayerMove))]
            [RequireComponent(typeof(PlayerItemController))]
            public class InGameInputInjector : MonoBehaviour
        {
            private IInGamePlayerInput _input;
            private ApplyInGamePlayerMove _move;
            private PlayerItemController _holdIngredient;

            public bool IsSelected { get; private set; }

            [Inject]
            public void Construct(IInGamePlayerInput input)
            {
                _input = input;
            }

            private void Awake()
            {
                _move = GetComponent<ApplyInGamePlayerMove>();
                _holdIngredient = GetComponent<PlayerItemController>();
            }

            private void Start()
            {
                SetSelected(true);
            }

            private void Update()
            {
                if (!IsSelected)
                {
                    _move.SetMoveInput(Vector2.zero);
                    return;
                }

                _move.SetMoveInput(_input.Move);

                if (_input.DashInput)
                {
                    _move.TryDash();
                }

                if (_input.InteractionIngredientInput)
                {
                    _holdIngredient.TryInteractionIngredient();
                }

                if (_input.InteractionCookInput)
                {
                    _holdIngredient.TryInteractionCook();
                }

                if (_input.SwitchingInput)
                {
                    // 여기서 플레이어 스위칭 처리 연결
                }
            }

            public void SetSelected(bool isSelected)
            {
                IsSelected = isSelected;
            }
        }

    }

