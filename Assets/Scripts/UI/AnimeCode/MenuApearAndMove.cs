using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuApearAndMove : MonoBehaviour
{
    [SerializeField] GameObject menu;
    [SerializeField] Vector3 originPos;
    [SerializeField] Vector3 targetPos;
    [SerializeField] float duration=0.5f;
    public void MoveAndApear()
    {
        menu.SetActive(true);
        menu.transform.position = originPos;
        StartCoroutine(MoveToTarget());
    }
    IEnumerator MoveToTarget()
    {
        float t = 0;
        while(t < duration) {
        
            t += Time.deltaTime;
            menu.transform.position = Vector3.Lerp(transform.position, targetPos, t / duration);
            yield return null;
        }
        menu.transform.position= targetPos;

    }
}
