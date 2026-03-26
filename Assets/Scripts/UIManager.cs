using Overcooked.Interfaces;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
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

        [Header("스코어 UI")]
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _tipText;
        [SerializeField] private Animator _coinAnimator;
        [SerializeField] private float _fadeDuration = 1.0f;
        [SerializeField] private float _moveSpeed = 50f;
        [SerializeField] private GameObject _flameEffect;
        [SerializeField] private Animator _flameAnimator;

        private Coroutine _tipCoroutine;

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

            if (_flameEffect != null)
            {
                bool isMaxCombo = (combo == 4);

                if (_flameEffect.activeSelf != isMaxCombo)
                {
                    _flameEffect.SetActive(isMaxCombo);

                    if (isMaxCombo && _flameEffect != null)
                    {
                        _flameAnimator.Play("FlameAnim", 0, 0f);
                    }
                }
            }
        }

        public void UpdateScoreText(int currentScore)
        {
            if (_scoreText != null)
            {
                _scoreText.text = currentScore.ToString();
            }

            if (_coinAnimator != null)
            {
                _coinAnimator.Play("CoinSpinAnim", 0, 0f);
            }
        }

        public void ShowTipEffect(int tipAmount)
        {
            if (_tipText == null || tipAmount <= 0)
            {
                return;
            }

            if (_tipCoroutine != null)
            {
                StopCoroutine( _tipCoroutine);
            }

            _tipCoroutine = StartCoroutine(TipFadeOutCoroutine(tipAmount));
        }

        private IEnumerator TipFadeOutCoroutine(int tipAmount)
        {
            _tipText.text = $"+ {tipAmount} Tip!";
            _tipText.gameObject.SetActive(true);

            Vector2 startPos = _tipText.rectTransform.anchoredPosition;
            Color startColor = _tipText.color;
            startColor.a = 1f;
            _tipText.color = startColor;

            float elapsed = 0f;

            while (elapsed < _fadeDuration)
            {
                elapsed += Time.deltaTime;
                float normalizedTime = elapsed / _fadeDuration;

                _tipText.rectTransform.anchoredPosition += Vector2.up * _moveSpeed * Time.deltaTime;

                Color currentColor = _tipText.color;
                currentColor.a = Mathf.Lerp(1f, 0f, normalizedTime);
                _tipText.color = currentColor;

                yield return null;
            }

            _tipText.gameObject.SetActive(false);

            _tipText.rectTransform.anchoredPosition = startPos;
        }
    }
}
