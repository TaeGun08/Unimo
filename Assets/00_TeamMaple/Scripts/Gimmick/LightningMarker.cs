using System;
using System.Collections;
using UnityEngine;

public class LightningStrikeMarker : MonoBehaviour
{
    private Action onComplete;
    private float duration;
    private Vector3 startScale = Vector3.one * 0.5f;
    private Vector3 endScale = Vector3.one * 3f;

    public void Init(Action onStrikeCallback, float growTime)
    {
        onComplete = onStrikeCallback;
        duration = growTime;
        transform.localScale = startScale;
        StartCoroutine(GrowAndStrike());
    }

    private IEnumerator GrowAndStrike()
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        onComplete?.Invoke();
        Destroy(gameObject);
    }
}