using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace Overcooked
{
    public class PlayerSwitchManager : MonoBehaviour
    {
        [SerializeField] private List<InGameInputInjector> _players = new();
        [SerializeField] private int _currentIndex = 0;

        private IInGamePlayerInput _input;

        [Inject]
        public void Construct(IInGamePlayerInput input)
        {
            _input = input;
        }

        private void Start()
        {
            if (_players == null || _players.Count == 0)
            {
                return;
            }

            SelectPlayer(_currentIndex);
        }

        private void Update()
        {
            if (_input == null)
            {
                return;
            }

            if (_input.SwitchingInput)
            {
                SwitchNextPlayer();
            }
        }

        public void SwitchNextPlayer()
        {
            if (_players == null || _players.Count == 0)
            {
                return;
            }

            _currentIndex = (_currentIndex + 1) % _players.Count;
            SelectPlayer(_currentIndex);
        }

        private void SelectPlayer(int index)
        {
            for (int i = 0; i < _players.Count; i++)
            {
                if (_players[i] == null) continue;
                _players[i].SetSelected(i == index);
            }
        }
    }
}
