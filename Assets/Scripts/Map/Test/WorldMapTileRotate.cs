using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapTileRotate : MonoBehaviour
{
    [Header("Settings")]
    public float duration = 1.0f; // 뒤집히는 데 걸리는 시간
    public Material nextMaterial; // 바뀔 머테리얼

    private bool isFlipping = false;
    private MeshRenderer meshRenderer;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Flip()
    {
        if (!isFlipping)
        {
            StartCoroutine(FlipRoutine());
        }
    }

    private IEnumerator FlipRoutine()
    {
        isFlipping = true;

        Quaternion startRotation = transform.rotation;
        // 현재 회전값에서 Y축(혹은 설계에 따라 X축)으로 180도 회전한 목표값
        Quaternion endRotation = startRotation * Quaternion.Euler(0, 0, 180f);

        float elapsed = 0f;
        bool materialChanged = false;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // 부드러운 가속/감속을 위해 Lerp 대신 Slerp나 AnimationCurve 사용 가능
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);

            // 90도 정도 회전했을 때(절반 지점) 머테리얼을 교체하여 자연스럽게 보이게 함
            if (!materialChanged && t >= 0.5f)
            {
                meshRenderer.material = nextMaterial;
                materialChanged = true;
            }

            yield return null;
        }

        transform.rotation = endRotation;
        isFlipping = false;
    }
}