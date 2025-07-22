using System;
using System.Collections;
using UnityEngine;

public class LightningStrikeMarker : MonoBehaviour
{
    private Action onComplete;
    private float duration;
    private Vector3 startXZ = new Vector2(0.5f, 0.5f);
    private Vector3 endXZ = new Vector2(3f, 3f);

    private const float fixedY = 0.1f;

    public void Init(Action onStrikeCallback, float growTime)
    {
        onComplete = onStrikeCallback;
        duration = growTime;

        transform.localScale = new Vector3(startXZ.x, fixedY, startXZ.y);

        StartCoroutine(GrowAndStrike());
    }

    private IEnumerator GrowAndStrike()
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            float x = Mathf.Lerp(startXZ.x, endXZ.x, t);
            float z = Mathf.Lerp(startXZ.y, endXZ.y, t);
            transform.localScale = new Vector3(x, fixedY, z);

            yield return null;
        }

        onComplete?.Invoke();
        Destroy(gameObject);
    }
}
