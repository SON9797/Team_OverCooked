using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickAnime_BigtoOrigin : MonoBehaviour
{
    [SerializeField] float bigScale = 1.5f;
    [SerializeField] float duration = 0.5f;


    Coroutine current;
    Vector3 originScale;
    private void Start()
    {
        originScale = transform.localScale;
    }
    public void OnClcicked()
    {
        if (current != null)
        {
            StopCoroutine(current);
            transform.localScale = originScale;
        }
        current = StartCoroutine(BigToOrigin());
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
