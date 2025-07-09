using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonArea : MonoBehaviour
{
    public float tickInterval = 3f;
    public float percentDamage = 0.05f; // 5%
    public string targetTag = "Player";

    private Dictionary<GameObject, Coroutine> activeTargets = new();

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
        IDamageable damageTarget = target.GetComponent<IDamageable>();
        if (damageTarget == null) yield break;

        while (true)
        {
            float damage = damageTarget.CurrentHealth * percentDamage;
            damageTarget.TakeDamage(damage);
            yield return new WaitForSeconds(tickInterval);
        }
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

public interface IDamageable
{
    float CurrentHealth { get; }
    void TakeDamage(float amount);
}