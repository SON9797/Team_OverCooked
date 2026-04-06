using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapBuilding : MonoBehaviour
{
    [SerializeField] private float _appearDuration = 0.5f;
    [SerializeField] private AnimationCurve _appearCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private void Awake()
    {
        transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }

    public void Appear()
    {
        gameObject.SetActive(true);
        StartCoroutine(AppearRoutine());
    }

    private IEnumerator AppearRoutine()
    {
        float elapsed = 0f;

        while (elapsed < _appearDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _appearDuration;

            float scale = _appearCurve.Evaluate(t);
            transform.localScale = new Vector3(scale, scale, scale);

            yield return null;
        }

        transform.localScale = Vector3.one;
    }
}
