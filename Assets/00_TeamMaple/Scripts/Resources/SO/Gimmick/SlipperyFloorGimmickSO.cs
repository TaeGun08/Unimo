// ✅ 아이템 1개 스폰, 픽업/지속 이펙트 최대 1개, 면역 동안 슬리퍼리 정지+속도 0, 종료 후 재적용
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "SlipperyFloorGimmickSO", menuName = "StageGimmick/SlipperyFloor")]
public class SlipperyFloorGimmickSO : StageGimmickSO
{
    [Header("슬리퍼리 파라미터")]
    public float slipperyDuration = 3f;
    public float slipperyForce = 150f;
    public float maxSlipSpeed = 3f;

    [Header("아이템 스폰")]
    public float itemSpawnInterval = 15f;
    public Vector3 itemSpawnCenter;
    public float itemSpawnRadius = 10f;

    [Header("아이템 면역 & 이펙트")]
    public float itemImmuneDuration = 30f;

    public override GameObject Execute(Vector3 origin)
    {
        var runnerObj = new GameObject("SlipperyFloorRunner");
        var runner = runnerObj.AddComponent<SlipperyFloorRunner>();
        runner.Init(this, origin);
        return runnerObj;
    }

    private void OnEnable()
    {
        GimmickRegistry.Register(StageGimmickType.SlipperyFloor, this);
    }
}

public class SlipperyFloorRunner : MonoBehaviour
{
    private SlipperyFloorGimmickSO data;

    // 러너 루틴
    private Coroutine sequenceRoutine;
    private Coroutine reapplyRoutine;
    private Coroutine itemRoutine;

    // 아이템/이펙트 관리
    private float itemSpawnInterval;
    private Vector3 itemSpawnCenter;
    private float itemSpawnRadius;
    private GameObject gimmickItemPrefab;

    private GameObject pickupEffect;
    private GameObject durationEffect;

    private GameObject activePickupEffect;
    private GameObject activeDurationEffect;
    private Coroutine durationEffectLife;

    public static SlipperyFloorRunner Instance { get; private set; }

    public void Init(SlipperyFloorGimmickSO so, Vector3 origin)
    {
        Instance = this;
        data = so;

        itemSpawnInterval = data.itemSpawnInterval;
        itemSpawnCenter = data.itemSpawnCenter == Vector3.zero ? origin : data.itemSpawnCenter;
        itemSpawnRadius = data.itemSpawnRadius;
        gimmickItemPrefab = data.gimmickItemPrefab;

        pickupEffect = data.pickupEffect;
        durationEffect = data.durationEffect;

        sequenceRoutine = StartCoroutine(SlipperySequence());
        itemRoutine = StartCoroutine(SpawnItemRoutine());
    }

    private IEnumerator SlipperySequence()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        var slippery = player ? player.GetComponent<SlipperyReceiver>() : null;

        if (slippery != null && slippery.IsSlipImmune)
        {
            Debug.Log("[Slippery] 면역 상태이므로 최초 슬리퍼리 무시");
            Destroy(gameObject);
            yield break;
        }

        if (slippery != null)
        {
            slippery.SetSlippery(true, data.slipperyForce, data.maxSlipSpeed);
            yield return new WaitForSeconds(data.slipperyDuration);
            slippery.SetSlippery(false); // 자연 종료: hardReset=false
        }

        Destroy(gameObject);
    }

    // ⬇️ 아이템 “최대 1개” 스폰
    private IEnumerator SpawnItemRoutine()
    {
        int itemLayer = LayerMask.NameToLayer("Item");
        int itemMask = 1 << itemLayer;

        while (true)
        {
            yield return new WaitForSeconds(itemSpawnInterval);

            // 현재 필드에 아이템이 남아 있으면 스킵
            Collider[] hits = Physics.OverlapSphere(itemSpawnCenter, itemSpawnRadius * 2f, itemMask);
            bool anyValid = false;
            foreach (var h in hits)
            {
                if (h && h.isTrigger && h.gameObject.activeInHierarchy)
                {
                    anyValid = true; break;
                }
            }
            if (anyValid) continue;

            if (gimmickItemPrefab != null)
            {
                Vector3 randomPos = itemSpawnCenter + new Vector3(
                    Random.Range(-itemSpawnRadius, itemSpawnRadius),
                    0.5f,
                    Random.Range(-itemSpawnRadius, itemSpawnRadius)
                );

                var rot = Quaternion.Euler(-90f, 0f, 0f);
                var item = Instantiate(gimmickItemPrefab, randomPos, rot);
                item.layer = itemLayer;

                // GimmickItem이 이 시그니처를 지원해야 함
                item.GetComponent<GimmickItem>()?.Init(
                    StageGimmickType.SlipperyFloor,
                    this,
                    pickupEffect,
                    durationEffect,
                    data.itemImmuneDuration
                );
            }
        }
    }

    // ⬇️ GimmickItem → 아이템 먹을 때 호출
    public void GrantSlipImmunity(float duration, Transform player)
    {
        // 1) 슬리퍼리 중지 + 강제 정지(속도 0)
        PauseByImmunity(duration);

        // 2) 픽업 이펙트(최대 1개)
        if (pickupEffect && player)
        {
            if (activePickupEffect) Destroy(activePickupEffect);
            activePickupEffect = Instantiate(pickupEffect, player);
            Destroy(activePickupEffect, 5f); // 파티클 안전타임
        }

        // 3) 지속 이펙트(최대 1개, 면역 끝나면 제거)
        if (durationEffect && player)
        {
            if (activeDurationEffect) Destroy(activeDurationEffect);
            activeDurationEffect = Instantiate(durationEffect, player);

            if (durationEffectLife != null) StopCoroutine(durationEffectLife);
            durationEffectLife = StartCoroutine(KillDurationEffectAfter(duration));
        }
    }

    private IEnumerator KillDurationEffectAfter(float t)
    {
        yield return new WaitForSeconds(t);
        if (activeDurationEffect) Destroy(activeDurationEffect);
        activeDurationEffect = null;
        durationEffectLife = null;
    }

    // ⬇️ 면역 기간 동안 슬리퍼리 정지(속도 0으로 하드 리셋), 끝나면 재적용
    public void PauseByImmunity(float immuneDuration)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        var slippery = player ? player.GetComponent<SlipperyReceiver>() : null;
        if (!slippery) return;

        if (sequenceRoutine != null) { StopCoroutine(sequenceRoutine); sequenceRoutine = null; }
        if (reapplyRoutine  != null) { StopCoroutine(reapplyRoutine);  reapplyRoutine  = null; }

        // 슬리퍼리 강제 종료 + 속도 0 (hardReset = true)
        slippery.SetSlippery(false, hardReset: true);
        slippery.ApplySlipImmune(immuneDuration);

        reapplyRoutine = StartCoroutine(ReapplyAfterImmunity(slippery, immuneDuration));
    }

    private IEnumerator ReapplyAfterImmunity(SlipperyReceiver slippery, float immuneDuration)
    {
        yield return new WaitForSeconds(immuneDuration);
        while (slippery && slippery.IsSlipImmune) yield return null;

        if (!slippery) yield break;

        // 면역 종료 → 동일 파라미터로 1회 재적용
        slippery.SetSlippery(true, data.slipperyForce, data.maxSlipSpeed);
        yield return new WaitForSeconds(data.slipperyDuration);
        slippery.SetSlippery(false);

        if (this) Destroy(gameObject);
    }

    // 정적 진입점(원하면 사용)
    public static void ApplySlipImmunityStatic(float duration, Transform player = null)
    {
        if (Instance != null) Instance.GrantSlipImmunity(duration, player);
    }

    private void OnDestroy()
    {
        if (itemRoutine != null) StopCoroutine(itemRoutine);
        if (sequenceRoutine != null) StopCoroutine(sequenceRoutine);
        if (reapplyRoutine  != null) StopCoroutine(reapplyRoutine);
        if (durationEffectLife != null) StopCoroutine(durationEffectLife);

        if (activePickupEffect)   Destroy(activePickupEffect);
        if (activeDurationEffect) Destroy(activeDurationEffect);

        if (Instance == this) Instance = null;
    }
}
