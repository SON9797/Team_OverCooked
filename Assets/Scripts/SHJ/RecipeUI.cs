using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Overcooked
{
    public class RecipeUI : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Image _dishImage;              // 완성 이미지
        [SerializeField] private Transform _ingredientParent;   // 아이콘이 들어갈 부모
        [SerializeField] private Image _ingredientPrefab;  // 재료 아이콘

        [Header("UI Move Setting")]
        [SerializeField] private float _moveDuration = 0.3f;
        [SerializeField] private float _startOffsetX = 500f;

        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void Setup(RecipeData data)
        {
            // 메인 요리 이미지 설정
            _dishImage.sprite = data.FinishedDishImage;

            foreach (Transform child in _ingredientParent)
            {
                Destroy(child.gameObject);
            }

            foreach (var icon in data.Ingredients)
            {
                Image newIcon = Instantiate(_ingredientPrefab, _ingredientParent);

                newIcon.sprite = icon;

                newIcon.enabled = true;
            }

            StartCoroutine(LinearSlideIn());
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
    }
}
