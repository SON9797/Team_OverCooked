using Overcooked.Interfaces;
using OverCooked;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class IrisFader : MonoBehaviour
{
    private static IrisFader _instance;

    public static IrisFader Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<IrisFader>();
            }
            return _instance;
        }
    }

    [Header("이미지")]
    [SerializeField] private Image _irisImage;
    [SerializeField] private Image _BlackImage;

    [Header("설정")]
    [SerializeField] private float _fadeDuration = 0.6f;
    [SerializeField] private Vector2 _maxSize = new Vector2(55000f, 55000f);
    [SerializeField] private Vector2 _minSize = new Vector2(1920f, 1920f);

    private IInGameSoundManager _inGameSoundManager;

    [Inject]
    public void Construct(IInGameSoundManager inGameSoundManager)
    {
        _inGameSoundManager = inGameSoundManager;
    }

    private void Awake()
    {
        _instance = this;
    }

    // 거대한 구멍이 화면 사이즈로 줄어들며 까맣게 닫힘
    public IEnumerator IrisInToBlack()
    {
        if (_irisImage == null)
        {
            yield break;
        }

        _irisImage.gameObject.SetActive(true);

        if (_inGameSoundManager != null)
        {
            _inGameSoundManager.StopAllSounds();
            _inGameSoundManager.PlaySFX(SFXType.UI_Screen_In);
        }

        float elapsedTime = 0f;

        while (elapsedTime < _fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsedTime / _fadeDuration);

            _irisImage.rectTransform.sizeDelta = Vector2.Lerp(_maxSize, _minSize, t);
            yield return null;
        }

        _irisImage.rectTransform.sizeDelta = _minSize;

        _BlackImage.gameObject.SetActive(true);
    }

    // 화면을 덮던 구멍이 거대해지며 게임 화면이 나타남
    public IEnumerator IrisOutFromBlack()
    {
        if (_irisImage == null)
        {
            yield break;
        }

        if (_inGameSoundManager != null)
        {
            _inGameSoundManager.StopAllSounds();
            _inGameSoundManager.PlaySFX(SFXType.UI_Screen_Out);
        }

        _irisImage.gameObject.SetActive(true);

        float elapsedTime = 0f;

        while (elapsedTime < _fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsedTime / _fadeDuration);

            _irisImage.rectTransform.sizeDelta = Vector2.Lerp(_minSize, _maxSize, t);


            yield return null;
        }

        _irisImage.rectTransform.sizeDelta = _maxSize;
    }
}

