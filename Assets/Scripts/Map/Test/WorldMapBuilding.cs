using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapBuilding : MonoBehaviour
{
    [SerializeField] private float _appearDuration = 0.5f;
    [SerializeField] private AnimationCurve _appearCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private bool _isAppeared = false;

    private void Awake()
    {
        transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }
    private void Start()
    {
        // 즉시 모드가 아니면 자동으로 숨김 유지
        if (!_isAppeared)
        {
            transform.localScale = Vector3.zero;
            gameObject.SetActive(false);
        }
    }
    public void AppearImmediate()
    {
        _isAppeared = true;
        
        gameObject.SetActive(true);
        transform.localScale = Vector3.one;

        StopAllCoroutines();
    }
    public void Appear()
    {
        _isAppeared = true;
        gameObject.SetActive(true);
        transform.localScale = Vector3.zero;
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
