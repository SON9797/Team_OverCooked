using OverCooked;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Overcooked
{
    public class RecipeUI : MonoBehaviour
    {
        [Header("Top UI")]
        [SerializeField] private Image _dishImage;              // 완성 이미지

        [Header("Bottom UI")]
        [SerializeField] private Transform _bottomGroupParent;
        [SerializeField] private GameObject _recipeBottomPrefab;

        [Header("UI Move Setting")]
        [SerializeField] private float _moveDuration = 0.3f;
        [SerializeField] private float _startOffsetX = 500f;

        [Header("Timer UI")]
        [SerializeField] private Image _timerFillImage;

        private RectTransform _rectTransform;

        public RecipeData CurrentRecipeData { get; private set; }
        public Action<RecipeUI> OnTimeOut;

        private bool _isEnding = false;


        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void Setup(RecipeData data, float timeLimit)
        {
            CurrentRecipeData = data;

            if (_dishImage != null && data.FinishedDishImage != null)
            {
                _dishImage.sprite = data.FinishedDishImage;
            }

            foreach (Transform child in _bottomGroupParent)
            {
                Destroy(child.gameObject);
            }

            foreach (var ingredient in data.Ingredients)
            {
                GameObject bottomObj = Instantiate(_recipeBottomPrefab, _bottomGroupParent);

                Transform iconTransform = bottomObj.transform.Find("Bottom_Image");
                if (iconTransform != null)
                {
                    Image iconImage = iconTransform.GetComponent<Image>();
                    if (iconImage != null)
                    {
                        iconImage.sprite = ingredient.icon;
                        iconImage.enabled = true;
                    }
                }

                Transform techTransform = bottomObj.transform.Find("Technique_Image");
                if (techTransform != null)
                {
                    if (ingredient.icon_techniques != null)
                    {
                        Image techImage = techTransform.GetComponent<Image>();
                        if (techImage != null)
                        {
                            techImage.sprite = ingredient.icon_techniques;
                            techImage.enabled = true;
                            techTransform.gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        techTransform.gameObject.SetActive(false);
                    }
                }
            }

            Canvas.ForceUpdateCanvases();

            RectTransform bottomRect = _bottomGroupParent.GetComponent<RectTransform>();
            LayoutRebuilder.ForceRebuildLayoutImmediate(bottomRect);

            float newWidth = bottomRect.rect.width;
            _rectTransform.sizeDelta = new Vector2(newWidth, _rectTransform.sizeDelta.y);

            LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);

            StartCoroutine(LinearSlideIn());
            StartCoroutine(TimerRoutine(timeLimit));
        }

        
        private IEnumerator TimerRoutine(float timeLimit)
        {
            float currentTime = timeLimit;

            while (currentTime > 0)
            {
                currentTime -= Time.deltaTime;

                if (_timerFillImage != null)
                {
                    float fillValue = currentTime / timeLimit;
                    _timerFillImage.fillAmount = fillValue;

                    if (fillValue <= 0.1f)
                    {
                        _timerFillImage.color = Color.red;
                    }

                    else if (fillValue <= 0.5f)
                    {
                        _timerFillImage.color = Color.Lerp(Color.red, new Color(1f, 0.5f, 0f), (fillValue - 0.1f) / 0.4f);
                    }

                    else
                    {
                        _timerFillImage.color = Color.Lerp(new Color(1f, 0.5f, 0f), Color.green, (fillValue - 0.5f) / 0.5f);
                    }
                }

                yield return null;
            }

            OnTimeOut?.Invoke(this);
        }

        private IEnumerator LinearSlideIn()
        {
            yield return new WaitForEndOfFrame();

            Vector2 targetPos = _rectTransform.anchoredPosition;
            Vector2 startPos = targetPos + new Vector2(_startOffsetX, 0);

            float elapsedTime = 0f;

            while (elapsedTime < _moveDuration)
            {
                elapsedTime += Time.deltaTime;

                float t = elapsedTime / _moveDuration;

                _rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);

                yield return null;
            }

            _rectTransform.anchoredPosition = targetPos;
        }

        public IEnumerator PlaySuccessEffect(Action onComplete)
        {
            if (_isEnding)
            {
                yield break;
            }
            _isEnding = true;

            StopAllCoroutines();

            Color targetColor = new Color(0.5f, 1f, 0.5f, 1f);

            yield return StartCoroutine(FadeOutRoutine(targetColor, onComplete));
        }

        public IEnumerator PlayFailEffect(Action onComplete)
        {
            if (_isEnding)
            {
                yield break;
            }
            _isEnding = true;

            StopAllCoroutines();

            Color targetColor = new Color(1f, 0.3f, 0.3f, 1f);

            yield return StartCoroutine(FadeOutRoutine(targetColor, onComplete));
        }

        private IEnumerator FadeOutRoutine(Color tintColor, Action onComplete)
        {
            float duration = 0.5f;
            float elapsed = 0f;

            Image[] allImages = GetComponentsInChildren<Image>();
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();

            foreach (var img in allImages)
            {
                if (img != null)
                {
                    img.color = Color.Lerp(img.color, tintColor, 0.5f);
                }
            }

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                if (canvasGroup != null)
                {
                    canvasGroup.alpha = Mathf.Lerp(1f, 0f, t * t);
                }

                yield return null;
            }

            onComplete?.Invoke();
        }
    }
}
