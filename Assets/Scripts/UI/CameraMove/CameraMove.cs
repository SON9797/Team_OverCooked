using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public static CameraMove instance;
    private void Awake()
    {
        instance = this;
    }

    
    public void MoveToTargetAsync(Transform target,float duration)
    {
        StopAllCoroutines();
        StartCoroutine( Move(target,duration));
    }
    IEnumerator Move(Transform target,float duration)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, target.position, t / duration);
            transform.rotation=Quaternion.Lerp(transform.rotation, target.rotation, t / duration);
            yield return null;
        }
        yield return null;
    }
}
