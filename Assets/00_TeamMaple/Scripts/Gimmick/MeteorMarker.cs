// MeteorMarker.cs
using System.Collections;
using UnityEngine;

public class MeteorMarker : MonoBehaviour
{
    public float duration = 2f;
    public Vector3 startScale = Vector3.one * 3f;
    public Vector3 endScale = Vector3.zero;

    private void Start()
    {
        transform.localScale = startScale;
        StartCoroutine(Shrink());
    }

    private IEnumerator Shrink()
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        Destroy(gameObject); // 마커는 사라짐
    }
}