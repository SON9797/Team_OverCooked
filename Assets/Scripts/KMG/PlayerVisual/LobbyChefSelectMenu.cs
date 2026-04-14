using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overcooked
{
    public class LobbyChefSelectMenu : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private GameObject _leftArrow;
        [SerializeField] private GameObject _rightArrow;

        [Header("외형 적용기")]
        [SerializeField] private ChefVisualApplier _chefVisualApplier;

        private ChefType[] _chefTypes;
        private int _currentIndex;

        private void Awake()
        {
            _chefTypes = (ChefType[])Enum.GetValues(typeof(ChefType));

            ChefRuntimeStore.EnsureInitialized();
            _currentIndex = FindChefIndex(ChefRuntimeStore.CurrentChef);

            HideArrows();
        }

        private void Start()
        {
            ApplyCurrentChef();
        }

        public void OpenMenu()
        {
            ShowArrows();
        }

        public void CloseMenu()
        {
            HideArrows();
        }

        public void SelectNextChef()
        {
            if (_chefTypes == null || _chefTypes.Length == 0)
                return;

            _currentIndex++;

            if (_currentIndex >= _chefTypes.Length)
                _currentIndex = 0;

            ApplyCurrentChef();
        }

        public void SelectPreviousChef()
        {
            if (_chefTypes == null || _chefTypes.Length == 0)
                return;

            _currentIndex--;

            if (_currentIndex < 0)
                _currentIndex = _chefTypes.Length - 1;

            ApplyCurrentChef();
        }

        private void ApplyCurrentChef()
        {
            ChefType selectedChef = _chefTypes[_currentIndex];

            if (_chefVisualApplier != null)
            {
                _chefVisualApplier.SetChef(selectedChef);
            }
            else
            {
                ChefRuntimeStore.SetChef(selectedChef);
            }
        }

        private int FindChefIndex(ChefType chefType)
        {
            for (int i = 0; i < _chefTypes.Length; i++)
            {
                if (_chefTypes[i] == chefType)
                    return i;
            }

            return 0;
        }

        private void ShowArrows()
        {
            if (_leftArrow != null)
                _leftArrow.SetActive(true);

            if (_rightArrow != null)
                _rightArrow.SetActive(true);
        }

        private void HideArrows()
        {
            if (_leftArrow != null)
                _leftArrow.SetActive(false);

            if (_rightArrow != null)
                _rightArrow.SetActive(false);
        }
    }
}
