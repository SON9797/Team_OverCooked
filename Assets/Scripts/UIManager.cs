using Overcooked.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

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
        [SerializeField] private GameObject _timerPanel;
        [SerializeField] private GameObject _endingPanel;
        [SerializeField] private GameObject _timesUpPanel;
        [SerializeField] private Image _timerGauge;

        [Header("스테이지 정보 업데이트용 UI")]
        [SerializeField] private TextMeshProUGUI _loadingLevelText;
        [SerializeField] private Image _loadingImage;
        [SerializeField] private Image _tutorialImage;
        [SerializeField] private TextMeshProUGUI _ingameLevelText;
        [SerializeField] private TextMeshProUGUI _timerText;

        [Header("모래시계 연출")]
        [SerializeField] private RectTransform _hourglassIcon;

        [Header("콤보 UI")]
        [SerializeField] private GameObject[] _comboIcons;

        private bool _isHourglassShaking = false;
        private float _shakeSpeed = 20f;
        private float _shakeAmount = 15f;


        public GameObject LoadingPanel => _loadingPanel;
        public GameObject TutorialPanel => _recipeTutorialPanel;
        public GameObject ReadyPanel => _readyPanel;
        public GameObject StartPanel => _startPanel;
        public GameObject CoinPanel => _coinPanel;
        public GameObject TimerPanel => _timerPanel;
        public GameObject EndingPanel => _endingPanel;        
        public GameObject TimesUpPanel => _timesUpPanel;

     
        private void Update()
        {
            if (_isHourglassShaking && _hourglassIcon != null)
            {
                float angle = Mathf.Sin(Time.time * _shakeSpeed) * _shakeAmount;
                _hourglassIcon.localRotation = Quaternion.Euler(0, 0, angle);
            }
            else if (_hourglassIcon != null)
            {
                _hourglassIcon.localRotation = Quaternion.identity;
            }
            
        }

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

        public void UpdateTimerGauge(float currentTime, float maxTime)
        {
            if (_timerGauge == null || maxTime <= 0)
            {
                return;
            }

            float fillValue = currentTime / maxTime;

            _timerGauge.fillAmount = fillValue;

            _isHourglassShaking = (fillValue <= 0.2f && fillValue > 0);

            UpdateTimerColor(fillValue);
        }

        public void UpdateTimerColor(float fillValue)
        {
            if (_timerGauge == null)
            {
                return;
            }

            if (fillValue <= 0.1f)
            {
                _timerGauge.color = Color.red;
            }

            else if (fillValue <= 0.5f)
            {
                _timerGauge.color = Color.Lerp(Color.red, new Color(1f, 0.5f, 0f), (fillValue - 0.1f) / 0.4f);
            }

            else
            {
                _timerGauge.color = Color.Lerp(new Color(1f, 0.5f, 0f), Color.green, (fillValue - 0.5f) / 0.5f);
            }
        }

        public void UpdateComboUI(int combo)
        {
            for (int i = 0; i < _comboIcons.Length; i++)
            {
                if (_comboIcons[i] != null)
                {
                    _comboIcons[i].SetActive(false);
                }
            }

            int targetIndex = 0;

            if (combo > 0 && combo <= _comboIcons.Length)
            {
                targetIndex = combo - 1;
            }

            if (_comboIcons[targetIndex] != null)
            {
                _comboIcons[targetIndex].SetActive(true);
            }
        }
    }
}
