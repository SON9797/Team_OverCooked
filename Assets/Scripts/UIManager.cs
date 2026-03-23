using Overcooked.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Overcooked
{
    public class UIManager : MonoBehaviour, IUIManager
    {
        [Header("¼¼ÆÃ")]
        [SerializeField] private GameObject _loadingPanel;
        [SerializeField] private GameObject _recipeTutorialPanel;
        [SerializeField] private GameObject _readyPanel;
        [SerializeField] private GameObject _startPanel;
        [SerializeField] private GameObject _coinPanel;
        [SerializeField] private GameObject _recipePanel;
        [SerializeField] private GameObject _timerPanel;
        [SerializeField] private GameObject _endingPanel;
        [SerializeField] private GameObject _timesUpPanel;

        public GameObject LoadingPanel => _loadingPanel;
        public GameObject TutorialPanel => _recipeTutorialPanel;
        public GameObject ReadyPanel => _readyPanel;
        public GameObject StartPanel => _startPanel;
        public GameObject CoinPanel => _coinPanel;
        public GameObject RecipePanel => _recipePanel;
        public GameObject TimerPanel => _timerPanel;
        public GameObject EndingPanel => _endingPanel;        
        public GameObject TimesUpPanel => _timesUpPanel;


        public void SetPanelActive(GameObject panel, bool isActive)
        {
            if (panel != null)
            {
                panel.SetActive(isActive);
            }
        }

        public void StartManagerCoroutine(System.Collections.IEnumerator routine)
        {
            StartCoroutine(routine);
        }
    }
}
