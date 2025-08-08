using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonArea : MonoBehaviour
{
    public float scaleDuration = 2f;
    public float targetScale = 6f;

    private float tickInterval;
    private float percentDamage;
    private float initialDelay;

    public float detectionRadius = 2.5f;
    public LayerMask playerLayer;

    private Dictionary<GameObject, Coroutine> activePoisonCoroutines = new();

    private void Awake()
    {
        transform.localScale = Vector3.zero;
    }

    private void Start()
    {
        StartCoroutine(ScaleOverTime(transform, Vector3.one * targetScale, scaleDuration));
        StartCoroutine(PoisonDetectionLoop());
    }

    public void Init(float tick, float percent, float delay)
    {
        tickInterval = tick;
        percentDamage = percent;
        initialDelay = delay;
    }

    private IEnumerator PoisonDetectionLoop()
    {
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            HashSet<GameObject> currentFramePlayers = new();

            Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);

            foreach (var col in hits)
            {
                if (!col.CompareTag("Player")) continue;

                GameObject player = col.gameObject;
                currentFramePlayers.Add(player);

                if (!activePoisonCoroutines.ContainsKey(player))
                {
                    Coroutine routine = StartCoroutine(ApplyPoison(player));
                    activePoisonCoroutines.Add(player, routine);
                }
            }

            // 감지되지 않은 플레이어는 코루틴 중단
            List<GameObject> toRemove = new();
            foreach (var kvp in activePoisonCoroutines)
            {
                if (!currentFramePlayers.Contains(kvp.Key))
                {
                    StopCoroutine(kvp.Value);
                    toRemove.Add(kvp.Key);
                }
            }
            foreach (var target in toRemove)
            {
                activePoisonCoroutines.Remove(target);
            }

            yield return new WaitForSeconds(0.5f); // 감지 주기
        }
    }

    private IEnumerator ApplyPoison(GameObject target)
    {
        yield return new WaitForSeconds(0.01f); // 즉시 적용 가능 (지연 무시)

        var statHolder = LocalPlayer.Instance.PlayerStatHolder;

        while (true)
        {
            if (LocalPlayer.Instance.IsInvincible || statHolder.HasInvincible || statHolder.HasOnceInvalid)
            {
                if (statHolder.HasOnceInvalid)
                    statHolder.OnInvalidation();

                yield return new WaitForSeconds(tickInterval);
                continue;
            }

            int maxHp = statHolder.Hp.MaxValue;
            int damage = Mathf.CeilToInt(maxHp * percentDamage);

            if (!StageLoader.IsBonusStageByIndex(Base_Mng.Data.data.CurrentStage))
            {
                statHolder.Hp.Subtract(damage);
            }

            Debug.Log($"[PoisonArea] {target.name}에게 독 데미지 {damage} 적용");

            if (statHolder.Hp.Value <= 0)
            {
                LocalPlayer.Instance.playerController.ChangeState(IPlayerState.EState.Dead);
                yield break;
            }

            yield return new WaitForSeconds(tickInterval);
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
        foreach (var kv in activePoisonCoroutines)
        {
            if (kv.Value != null)
                StopCoroutine(kv.Value);
        }
        activePoisonCoroutines.Clear();
    }
}
