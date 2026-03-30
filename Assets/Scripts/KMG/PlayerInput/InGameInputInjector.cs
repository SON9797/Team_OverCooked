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

        private SceneFlowManager _sceneFlowManager;

        private UIManager _uiManager;

        private bool _isPaused = false;

        public bool IsSelected { get; private set; }

        [Inject]
        public void Construct(IInGamePlayerInput input, SceneFlowManager sceneFlowManager, UIManager uIManager)
        {
            _input = input;
            _sceneFlowManager = sceneFlowManager;
            _uiManager = uIManager;
        }

        private void Awake()
        {
            _move = GetComponent<ApplyInGamePlayerMove>();
            _holdIngredient = GetComponent<PlayerItemController>();
        }

        private void Update()
        {

            if (_move == null || _input == null || _holdIngredient == null || _uiManager == null)
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
                bool targetStatus = !_uiManager.PausePanel.activeSelf;
                _uiManager.SetPause(targetStatus);
            }

            if (_uiManager.PausePanel.activeSelf)
            {
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

