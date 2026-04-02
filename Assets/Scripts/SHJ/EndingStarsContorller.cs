using Overcooked;
using Overcooked.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace OverCooked
{
    public class EndingStarsContorller : MonoBehaviour
    {
        [Header("꽉찬 별")]
        public GameObject oneStarImage;
        public GameObject twoStarImage;
        public GameObject threeStarImage;

        [Header("연출 설정")]
        public float dealyBetweenStars = 0.5f;
        public float popAnimSpeed = 3f;
        public float startScaleMultiplier = 1.5f;

        private LevelData _levelData;
        private IInGameSoundManager _inGameSoundManager;

        private Vector3 _oneStarOriginScale;
        private Vector3 _twoStarOriginScale;
        private Vector3 _threeStarOriginScale;

        [Inject]
        public void Construct(LevelData levelData, IInGameSoundManager inGameSoundManager)
        {
            _levelData = levelData;
            _inGameSoundManager = inGameSoundManager;
        }

        private void Awake()
        {
            if (oneStarImage != null)
            {
                _oneStarOriginScale = oneStarImage.transform.localScale;
            }

            if (twoStarImage != null)
            {
                _twoStarOriginScale = twoStarImage.transform.localScale;
            }

            if (threeStarImage != null)
            {
                _threeStarOriginScale = threeStarImage.transform.localScale;
            }
        }

        public void ShowEndingStarEffect(int totalScore)
        {
            oneStarImage.SetActive(false);
            twoStarImage.SetActive(false);
            threeStarImage.SetActive(false);

            gameObject.SetActive(true);

            StartCoroutine(ShowStarsRoutine(totalScore));
        }

        private IEnumerator ShowStarsRoutine(int score)
        {
            yield return new WaitForSeconds(0.5f);

            if (score >= _levelData.OneStar)
            {
                _inGameSoundManager.PlaySFX(SFXType.UI_ResultStar);
                StartCoroutine(ScaleDownRoutin(oneStarImage, _oneStarOriginScale));
                yield return new WaitForSeconds(dealyBetweenStars);
            }

            if (score >= _levelData.TwoStar)
            {
                _inGameSoundManager.PlaySFX(SFXType.UI_ResultStar);
                StartCoroutine(ScaleDownRoutin(twoStarImage, _twoStarOriginScale));
                yield return new WaitForSeconds(dealyBetweenStars);
            }

            if (score >= _levelData.ThreeStar)
            {
                _inGameSoundManager.PlaySFX(SFXType.UI_ResultStar);
                StartCoroutine(ScaleDownRoutin(threeStarImage, _threeStarOriginScale));
            }
        }

        private IEnumerator ScaleDownRoutin(GameObject starObj, Vector3 oriongScale)
        {
            Vector3 startScale = oriongScale * startScaleMultiplier;
            starObj.transform.localScale = startScale;

            starObj.SetActive(true);

            float t = 0;
            while (t < 1f)
            {
                t += Time.deltaTime * popAnimSpeed;
                starObj.transform.localScale = Vector3.Lerp(startScale, oriongScale, t);
                yield return null;
            }

            starObj.transform.localScale = oriongScale;
        }
    }
}
