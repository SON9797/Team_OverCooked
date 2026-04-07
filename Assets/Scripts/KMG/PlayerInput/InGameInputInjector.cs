using OverCooked;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using Overcooked.Interfaces;

namespace Overcooked
{
    [RequireComponent(typeof(ApplyInGamePlayerMove))]
    [RequireComponent(typeof(PlayerItemController))]
    public class InGameInputInjector : MonoBehaviour
    {
        private IInGamePlayerInput _input;
        private ApplyInGamePlayerMove _move;
        private PlayerItemController _holdIngredient;

        private SceneFlowManager _sceneFlowManager;
        private InGameUIManager _inGameUIManager;
        private IInGameSoundManager _inGameSoundManager;

        private bool _isPaused = false;

        public bool IsSelected { get; private set; }

        [Inject]
        public void Construct(
            IInGamePlayerInput input,
            SceneFlowManager sceneFlowManager,
            InGameUIManager inGameUIManager,
            IInGameSoundManager inGameSoundManager
        )
        {
            _input = input;
            _sceneFlowManager = sceneFlowManager;
            _inGameUIManager = inGameUIManager;
            _inGameSoundManager = inGameSoundManager;
        }

        private void Awake()
        {
            _move = GetComponent<ApplyInGamePlayerMove>();
            _holdIngredient = GetComponent<PlayerItemController>();
        }

        private void Update()
        {
            if (_move == null || _input == null || _holdIngredient == null || _inGameUIManager == null)
            {
                return;
            }

            if (_sceneFlowManager.IsUIRunning || !IsSelected)
            {
                _move.SetMoveInput(Vector2.zero);
                return;
            }

            if (_input.PauseInput)
            {
                bool targetStatus = !_inGameUIManager.PausePanel.activeSelf;
                _inGameUIManager.SetPause(targetStatus);
            }

            if (_inGameUIManager.PausePanel.activeSelf)
            {
                return;
            }

            // 아이템던지기 - 조준 중에는 이동 막고 방향만 전환
            if (_holdIngredient.IsThrowAiming)
            {
                _move.SetMoveInput(Vector2.zero);
                _holdIngredient.UpdateThrowAim(_input.Move);
            }
            else
            {
                _move.SetMoveInput(_input.Move);

                if (_input.DashInput)
                {
                    _move.TryDash();
                }
            }

            if (_input.InteractionIngredientInput)
            {
                _holdIngredient.TryInteractionIngredient();
            }

            if (_input.InteractionCookPressed)
            {
                if (_holdIngredient.HasIngredient)
                {
                    if (_holdIngredient.CanThrowHeldObject)
                    {
                        _holdIngredient.StartThrowAim();
                    }
                }
                else
                {
                    _holdIngredient.TryInteractionCook();
                }
            }

            if (_input.InteractionCookReleased)
            {
                if (_holdIngredient.IsThrowAiming)
                {
                    _holdIngredient.ReleaseThrowAimAndThrow();
                }
            }
        }

        public void SetSelected(bool isSelected)
        {
            IsSelected = isSelected;

            if (!IsSelected && _move != null)
            {
                _move.SetMoveInput(Vector2.zero);
            }
        }
    }
}