using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoveStation : ItemPlaceAndTake
{
    [Header("시각 효과")]
    [SerializeField] private GameObject _stoveParticle;

    [Header("UI 설정")]
    [SerializeField] private GameObject _canvasObj;
    [SerializeField] private Slider _cookProgressBar;
    [SerializeField] private GameObject _cookingTick;
    [SerializeField] private GameObject _burnWarning;

    private Cookware _currentCookware;

    private CanvasGroup _tickCanvasGroup;
    private Coroutine _fadeCoroutine;
    private bool _cookedVisualTriggered = false;

    private float _overcookTimer = 0f;
    private float _blinkTimer = 0f;

    protected override void Start()
    {
        base.Start();

        if (_stoveParticle != null)
        {
            _stoveParticle.SetActive(false);
        }

        if (_canvasObj != null)
        {
            _canvasObj.SetActive(false);
        }

        if (_cookProgressBar != null)
        {
            _cookProgressBar.value = 0f;
        }

        if (_burnWarning != null)
        {
            _burnWarning.SetActive(false);
        }

        if (_cookingTick != null)
        {
            _tickCanvasGroup = _cookingTick.GetComponent<CanvasGroup>();

            if (_tickCanvasGroup != null)
            {
                _cookingTick.SetActive(false);
            }
        }
    }
    

    protected override void Update()
    {
        base.Update();
        
        if (_currentCookware != null)
        {
            if (_currentCookware.HasIngredients && !_currentCookware.IsCooked)
            {
                _currentCookware.CookTick(Time.deltaTime);

                if (_stoveParticle != null && !_stoveParticle.activeSelf)
                {
                    _stoveParticle.SetActive(true);
                }

                if (_canvasObj != null && !_canvasObj.activeSelf)
                {
                    _canvasObj.SetActive(true);
                }

                SetCookingUIActive(true);

                if (_cookProgressBar != null)
                {
                    _cookProgressBar.value = _currentCookware.CurrentCookTime / _currentCookware.MaxCookTime;
                }

                _cookedVisualTriggered = false;
            }

            else
            {
                if (_stoveParticle != null && _stoveParticle.activeSelf)
                {
                    _stoveParticle.SetActive(false);
                }

                if (_currentCookware.IsCooked)
                {
                    if (_canvasObj != null && !_canvasObj.activeSelf)
                    {
                        _canvasObj.SetActive(true);
                    }

                    if (!_cookedVisualTriggered)
                    {
                        _cookedVisualTriggered = true;

                        SetCookingUIActive(false);

                        if (_fadeCoroutine != null)
                        {
                            StopCoroutine(_fadeCoroutine);
                        }

                        _fadeCoroutine = StartCoroutine(FadeOutCookingTick());
                    }

                    _overcookTimer += Time.deltaTime;

                    if (_overcookTimer >= 3f && _overcookTimer < 8f)
                    {
                        if (_cookingTick != null && _cookingTick.activeSelf)
                        {
                            _cookingTick.SetActive(false);
                        }

                        float warningProgress = (_overcookTimer - 3f) / 5f;

                        _blinkTimer -= Time.deltaTime;

                        if (_blinkTimer <= 0f)
                        {
                            float blinkInterval = Mathf.Lerp(0.5f, 0.08f, warningProgress);
                            _blinkTimer = blinkInterval;

                            bool turnOn = _burnWarning != null && !_burnWarning.activeSelf;

                            if (_burnWarning != null)
                            {
                                _burnWarning.SetActive(turnOn);
                            }

                            if (turnOn)
                            {
                                // 사운드 추가 예정
                            }
                        }
                    }

                    else if (_overcookTimer >= 8f)
                    {
                        if (_burnWarning != null && _burnWarning.activeSelf)
                        {
                            _burnWarning.SetActive(false);
                            // 음식 타는 사운드 예정
                        }

                        // 요리 완성에서 타는 상태로 변경 로직 추가 예정
                    }

                }

                else if (!_currentCookware.HasIngredients)
                {
                    if (_canvasObj != null && _canvasObj.activeSelf)
                    {
                        _canvasObj.SetActive(false);
                    }

                    ResetCookedVisual();
                }
            }
        }
    }

    private void SetCookingUIActive(bool isCooking)
    {
        if (_cookProgressBar != null)
        {
            _cookProgressBar.gameObject.SetActive(isCooking);
        }

        if (_cookingTick != null && !isCooking && !_cookingTick.activeSelf)
        {
            if (_tickCanvasGroup != null)
            {
                _tickCanvasGroup.alpha = 1f;
            }

            _cookingTick.SetActive(true);
        }
    }

    private void ResetCookedVisual()
    {
        _cookedVisualTriggered = false;

        _overcookTimer = 0f;
        _blinkTimer = 0f;

        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }

        if (_cookingTick != null)
        {
            _cookingTick.SetActive(false);
            
            if (_tickCanvasGroup != null)
            {
                _tickCanvasGroup.alpha = 1f;
            }
        }

        if (_burnWarning != null)
        {
            _burnWarning.SetActive(false);
        }
    }

    private IEnumerator FadeOutCookingTick()
    {
        if (_tickCanvasGroup == null)
        {
            yield break;
        }

        float duration = 1.5f;
        float currentTime = 0f;

        yield return new WaitForSeconds(0.5f);

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;

            _tickCanvasGroup.alpha = Mathf.Lerp(1f, 0f, currentTime / duration);

            yield return null;
        }

        _tickCanvasGroup.alpha = 0f;
        _cookingTick.SetActive(false);
        _fadeCoroutine = null;
    }

    public override bool PlaceItem(GameObject item)
    {
        bool success = base.PlaceItem(item);

        if (success)
        {
            if (_onCounterItem != null && _onCounterItem.TryGetComponent<Cookware>(out Cookware cookware))
            {
                if (_currentCookware != cookware)
                {
                    _currentCookware = cookware;
                    _currentCookware.SetOnStove(true);

                    ResetCookedVisual();
                }
            }
        }

        return success;
    }

    public override GameObject TakeItem()
    {
        if (_currentCookware != null)
        {
            _currentCookware.SetOnStove(false);
            _currentCookware = null;

            if (_stoveParticle != null)
            {
                _stoveParticle.SetActive(false);
            }

            if (_canvasObj != null)
            {
                _canvasObj.SetActive(false);
            }

            ResetCookedVisual();
        }

        return base.TakeItem();
    }

}
