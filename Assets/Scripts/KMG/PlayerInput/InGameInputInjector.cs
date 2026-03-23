using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;


namespace Overcooked
{ 
    [RequireComponent(typeof(ApplyInGamePlayerMove))]
    public class InGameInputInjector : MonoBehaviour
    {
        private IInGamePlayerInput _input;
        private ApplyInGamePlayerMove _move;
    
    
        public bool IsSelected { get; private set; }
    
        [Inject]
        public void Construct(IInGamePlayerInput input)
        {
            _input = input;
        }


        private void Start()
        {
            SetSelected(true);
        }


        private void Awake()
        {
            _move = GetComponent<ApplyInGamePlayerMove>();
        }
    
        private void Update()
        {
            if (!IsSelected)
            {
                _move.SetMoveInput(Vector2.zero);
                return;
            }
    
            _move.SetMoveInput(_input.Move);
    
            if(_input.DashInput)
            {
                _move.TryDash();
            }
        }
    
    
        public void SetSelected(bool isSelected)
        {
            IsSelected = isSelected;
        }

    }



}
