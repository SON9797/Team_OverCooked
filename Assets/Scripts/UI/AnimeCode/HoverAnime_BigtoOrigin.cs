using Overcooked.Interfaces;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

public class HoverAnime_BigtoOrigin : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] float bigScale=1.5f;
    [SerializeField] float duration=0.5f;


    Coroutine current;
    Vector3 originScale;

    private ICommonSoundManager _soundManager;

    [Inject]
    public void Construct(ICommonSoundManager soundManager)
    {
        Debug.Log($"<color=green>[성공]</color> {gameObject.name}에 사운드 매니저 들어옴!");
        _soundManager = soundManager;
    }

    private void Start()
    {
        originScale = transform.localScale;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_soundManager != null)
        {
            _soundManager.PlaySFX(OverCooked.SFXType.UI_Click);
        }

        if (current!=null)
        {
            StopCoroutine(current);
            transform.localScale = originScale;
        }
        current=StartCoroutine(BigToOrigin());
    }

    IEnumerator BigToOrigin()
    {
       
        originScale = transform.localScale;
        Vector3 targetScale = transform.localScale * bigScale;
        transform.localScale = targetScale;
        float t = 0;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            transform.localScale = Vector3.Lerp(transform.localScale, originScale, t / duration);
            yield return null;
        }
        transform.localScale = originScale;
        
    }
}
