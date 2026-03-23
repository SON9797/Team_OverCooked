using Overcooked.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Overcooked
{
    public class UIManager : MonoBehaviour, IUIManager
    {
        [Header("세팅")]
        [SerializeField] private GameObject _loadingPanel;
        [SerializeField] private GameObject _recipeTutorialPanel;
        [SerializeField] private GameObject _readyPanel;
        [SerializeField] private GameObject _startPanel;
        [SerializeField] private GameObject _coinPanel;
        [SerializeField] private GameObject _recipePanel;
        [SerializeField] private GameObject _timerPanel;
        [SerializeField] private GameObject _endingPanel;
        [SerializeField] private GameObject _timesUpPanel;

        [Header("스테이지 정보 업데이트용 UI")]
        [SerializeField] private TextMeshProUGUI _loadingLevelText;
        [SerializeField] private Image _loadingImage;
        [SerializeField] private Image _tutorialImage;
        [SerializeField] private TextMeshProUGUI _ingameLevelText;
        [SerializeField] private TextMeshProUGUI _timerText;


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

        public void SetupLevelUI(LevelData levelData)
        {
            if (levelData == null)
            {
                return;
            }

            if (_loadingLevelText != null)
            {
                _loadingLevelText.text = levelData.LevelName;
            }

            if (_ingameLevelText != null)
            {
                _ingameLevelText.text = levelData.LevelName;
            }

            if (_loadingImage != null)
            {
                _loadingImage.sprite = levelData.LoadingImage;
            }

            if (_tutorialImage != null)
            {
                _tutorialImage.sprite = levelData.TutorialImage;
            }
        }

        public void UpdateTimerText(float time)
        {
            if (_timerText != null)
            {
                int minutes = Mathf.FloorToInt(time / 60);
                int seconds = Mathf.FloorToInt(time % 60);

                _timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
        }
    }
}
