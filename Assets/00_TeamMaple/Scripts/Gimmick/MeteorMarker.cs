// MeteorMarker.cs
using System.Collections;
using UnityEngine;

public class MeteorMarker : MonoBehaviour
{
    public float duration = 2f;
    public float startXZ = 3f;
    public float endXZ = 0f;
    public float fixedY = 0.1f;

    private void Start()
    {
        transform.localScale = new Vector3(startXZ, fixedY, startXZ);
        StartCoroutine(Shrink());
    }

    private IEnumerator Shrink()
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            float xz = Mathf.Lerp(startXZ, endXZ, t);
            transform.localScale = new Vector3(xz, fixedY, xz);

            yield return null;
        }

        Destroy(gameObject);
    }
}
