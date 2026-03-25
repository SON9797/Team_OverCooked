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

        public bool IsSelected { get; private set; }

        [Inject]
        public void Construct(IInGamePlayerInput input, SceneFlowManager sceneFlowManager)
        {
            _input = input;
            _sceneFlowManager = sceneFlowManager;
        }

        private void Awake()
        {
            _move = GetComponent<ApplyInGamePlayerMove>();
            _holdIngredient = GetComponent<PlayerItemController>();
        }

        private void Update()
        {

            if (_move == null || _input == null || _holdIngredient == null)
            {
                return;
            }
            

            if (_sceneFlowManager.IsUIRunning || !IsSelected)
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

