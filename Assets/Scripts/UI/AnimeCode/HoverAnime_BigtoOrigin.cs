using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverAnime_BigtoOrigin : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] float bigScale=1.5f;
    [SerializeField] float duration=0.5f;


    bool coStart = false;
    private void Start()
    {
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(BigToOrigin());
    }

    IEnumerator BigToOrigin()
    {
        if (coStart)
        {
            yield break;
        }
        coStart = true;
        Vector3 originScale = transform.localScale;
        Vector3 targetScale = transform.localScale * bigScale;
        transform.localScale = targetScale;
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(transform.localScale, originScale, t / duration);
            yield return null;
        }
        transform.localScale = originScale;
        
        coStart = false;
    }
}
