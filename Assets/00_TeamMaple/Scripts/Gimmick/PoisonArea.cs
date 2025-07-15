using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonArea : MonoBehaviour
{
    public float initialDelay = 3f;
    public float tickInterval = 0.1f;
    public float percentDamage = 0.05f; // 5% of MaxHealth
    public string targetTag = "Player";
    public float scaleDuration = 2f;
    public float targetScale = 6f;

    private Dictionary<GameObject, Coroutine> activeTargets = new();

    private void Start()
    {
        StartCoroutine(ScaleOverTime(transform, Vector3.one * targetScale, scaleDuration));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(targetTag)) return;
        if (!activeTargets.ContainsKey(other.gameObject))
        {
            Coroutine routine = StartCoroutine(ApplyPoison(other.gameObject));
            activeTargets.Add(other.gameObject, routine);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (activeTargets.TryGetValue(other.gameObject, out Coroutine routine))
        {
            StopCoroutine(routine);
            activeTargets.Remove(other.gameObject);
        }
    }

    private IEnumerator ApplyPoison(GameObject target)
    {
        IDamageAble damageTarget = target.GetComponent<IDamageAble>();
        if (damageTarget == null) yield break;

        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            
        }
    }

    private IEnumerator ScaleOverTime(Transform t, Vector3 target, float duration)
    {
        Vector3 start = Vector3.zero;
        float time = 0f;
        while (time < duration)
        {
            t.localScale = Vector3.Lerp(start, target, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        t.localScale = target;
    }

    private void OnDestroy()
    {
        foreach (var kv in activeTargets)
        {
            if (kv.Value != null)
                StopCoroutine(kv.Value);
        }
        activeTargets.Clear();
    }
}