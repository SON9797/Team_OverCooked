using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Anime_MoveSelf : MonoBehaviour
{
    [SerializeField] Vector3 originPos;
    [SerializeField] Vector3 targetPos;
    [SerializeField] float duration = 0.5f;

    Coroutine coroutine;
    RectTransform rect;
    void Start()
    {
        rect = GetComponent<RectTransform>();
        ApearAndMove();
    }

    public void ApearAndMove()
    {
        if (coroutine!=null)
        {
            StopCoroutine(coroutine);
        }
        
        gameObject.SetActive(true);
        rect.anchoredPosition =originPos;
        StartCoroutine(MoveToTarget());
    }
    IEnumerator MoveToTarget()
    {
        float t = 0;
        while (t < duration)
        {

            t += Time.deltaTime;
            rect.anchoredPosition = Vector3.Lerp(rect.anchoredPosition, targetPos, t / duration);
            yield return null;
        }
        rect.anchoredPosition = targetPos;

    }
}
