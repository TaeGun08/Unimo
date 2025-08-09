// ✅ TimeSlowGimmickSO — Earthquake 패턴으로 전면 수정
// - 시작 5초 정상속도 → 슬로우/노말 사이클
// - 아이템 1개 제한, 먹으면 즉시 슬로우 해제(RemoveSlow)
// - 면역 지속 동안 슬로우 스킵
// - 픽업/지속 이펙트 각 1개 유지
using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "TimeSlowGimmickSO", menuName = "StageGimmick/TimeSlow")]
public class TimeSlowGimmickSO : StageGimmickSO
{
    [Header("슬로우 설정")]
    public float slowTimeScale = 0.5f;   // 느려지는 배속
    public float slowDuration  = 5f;     // 느려지는 시간(초, realtime)
    public float normalDuration = 3f;    // 정상 구간(초, realtime)

    [Header("아이템/이펙트")]
    public float itemSpawnInterval = 15f;
    public Vector3 itemSpawnCenter;
    public float itemSpawnRadius = 10f;
    [Header("아이템 효과")]
    public float itemImmuneDuration = 10f; // 면역 시간(초, realtime)

    public override GameObject Execute(Vector3 origin)
    {
        var obj = new GameObject("TimeSlowCycleRunner");
        var runner = obj.AddComponent<TimeSlowCycleRunner>();
        runner.Init(this, origin);
        return obj;
    }

    private void OnEnable()
    {
        GimmickRegistry.Register(StageGimmickType.TimeSlow, this);
    }
}

public class TimeSlowCycleRunner : MonoBehaviour
{
    private TimeSlowGimmickSO data;

    // 사이클
    private Coroutine cycleRoutine;

    // 아이템 스폰
    private Coroutine itemRoutine;
    private Vector3 itemSpawnCenter;
    private float itemSpawnRadius;

    // 면역 상태
    private bool isImmune = false;
    private Coroutine immuneRoutine;

    // 이펙트(최대 1개 유지)
    private GameObject activePickupEffect;
    private GameObject activeDurationEffect;
    private Coroutine durationEffectLife;

    // 원래 시간 복원용(안전)
    private float originalTimeScale;
    private float originalFixedDelta;

    public void Init(TimeSlowGimmickSO so, Vector3 origin)
    {
        data = so;

        originalTimeScale  = Time.timeScale;
        originalFixedDelta = Time.fixedDeltaTime;

        itemSpawnCenter = data.itemSpawnCenter == Vector3.zero ? origin : data.itemSpawnCenter;
        itemSpawnRadius = data.itemSpawnRadius;

        cycleRoutine = StartCoroutine(RunCycle());
        itemRoutine  = StartCoroutine(SpawnItemRoutine());
    }

    private IEnumerator RunCycle()
    {
        // 시작 5초 정상속도 유지
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        yield return new WaitForSecondsRealtime(5f);

        while (true)
        {
            // 면역 중이면 슬로우 러너 자체를 건너뜀
            if (!isImmune)
            {
                // 기존 슬로우 러너가 있으면 정리
                if (TimeSlowRunner.Instance != null)
                    Destroy(TimeSlowRunner.Instance.gameObject);

                // 새 러너 생성 및 슬로우 시작
                var go = new GameObject("TimeSlowRunner");
                var runner = go.AddComponent<TimeSlowRunner>();
                runner.Init(data.slowTimeScale, data.slowDuration);
            }

            // 슬로우 구간이 끝난 후 normalDuration 대기
            yield return new WaitForSecondsRealtime(data.slowDuration + data.normalDuration);
        }
    }

    private IEnumerator SpawnItemRoutine()
    {
        int itemLayer = LayerMask.NameToLayer("Item");
        int itemMask  = 1 << itemLayer;

        while (true)
        {
            yield return new WaitForSecondsRealtime(data.itemSpawnInterval);

            // 이미 필드에 아이템 있으면 스킵
            Collider[] hits = Physics.OverlapSphere(itemSpawnCenter, itemSpawnRadius * 2f, itemMask);
            bool anyValid = false;
            foreach (var h in hits)
            {
                if (h && h.isTrigger && h.gameObject.activeInHierarchy) { anyValid = true; break; }
            }
            if (anyValid) continue;

            if (data.gimmickItemPrefab != null)
            {
                Vector3 randomPos = itemSpawnCenter + new Vector3(
                    Random.Range(-itemSpawnRadius, itemSpawnRadius),
                    0.5f,
                    Random.Range(-itemSpawnRadius, itemSpawnRadius)
                );

                var rot  = Quaternion.Euler(-90f, 0f, 0f);
                var item = Instantiate(data.gimmickItemPrefab, randomPos, rot);
                item.layer = itemLayer;

                // GimmickItem이 이 시그니처를 지원해야 함
                item.GetComponent<GimmickItem>()?.Init(
                    StageGimmickType.TimeSlow,
                    this,                         // runner ref
                    data.pickupEffect,            // pickup effect
                    data.durationEffect,          // duration effect
                    data.itemImmuneDuration       // effect duration (sec)
                );
            }
        }
    }

    // GimmickItem → 먹었을 때 호출
    public void GrantTimeSlowImmunity(float duration, Transform player)
    {
        // 1) 현재 슬로우 즉시 해제
        TimeSlowRunner.RemoveSlow();

        // 2) 면역 시작 (슬로우 스킵)
        if (immuneRoutine != null) StopCoroutine(immuneRoutine);
        immuneRoutine = StartCoroutine(ImmuneRoutine(duration));

        // 3) 픽업 이펙트 1개
        if (data.pickupEffect && player)
        {
            if (activePickupEffect) Destroy(activePickupEffect);
            activePickupEffect = Instantiate(data.pickupEffect, player);
            Destroy(activePickupEffect, 5f); // 안전 타임
        }

        // 4) 지속 이펙트 1개 (면역 끝나면 제거)
        if (data.durationEffect && player)
        {
            if (activeDurationEffect) Destroy(activeDurationEffect);
            activeDurationEffect = Instantiate(data.durationEffect, player);

            if (durationEffectLife != null) StopCoroutine(durationEffectLife);
            durationEffectLife = StartCoroutine(KillDurationEffectAfter(duration));
        }
    }

    private IEnumerator ImmuneRoutine(float t)
    {
        isImmune = true;
        yield return new WaitForSecondsRealtime(t);
        isImmune = false;
        immuneRoutine = null;
    }

    private IEnumerator KillDurationEffectAfter(float t)
    {
        yield return new WaitForSecondsRealtime(t);
        if (activeDurationEffect) Destroy(activeDurationEffect);
        activeDurationEffect = null;
        durationEffectLife = null;
    }

    private void OnDestroy()
    {
        if (cycleRoutine != null) StopCoroutine(cycleRoutine);
        if (itemRoutine  != null) StopCoroutine(itemRoutine);
        if (immuneRoutine!= null) StopCoroutine(immuneRoutine);
        if (durationEffectLife != null) StopCoroutine(durationEffectLife);

        if (activePickupEffect)   Destroy(activePickupEffect);
        if (activeDurationEffect) Destroy(activeDurationEffect);

        // 안전 복구
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }
}

public class TimeSlowRunner : MonoBehaviour
{
    private float slowScale = 0.5f;
    private float slowDuration = 5f;

    private float originalTimeScale;
    private float originalFixedDelta;

    private Coroutine slowRoutine;

    public static TimeSlowRunner Instance { get; private set; }

    public void Init(float scale, float duration)
    {
        Instance = this;
        slowScale    = scale;
        slowDuration = duration;
        originalTimeScale  = Time.timeScale;
        originalFixedDelta = Time.fixedDeltaTime;

        slowRoutine = StartCoroutine(ApplySlow());
    }

    private IEnumerator ApplySlow()
    {
        // 슬로우 시작
        Time.timeScale = slowScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        yield return new WaitForSecondsRealtime(slowDuration);

        // 자동 복귀(정상)
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        Destroy(gameObject);
    }

    public static void RemoveSlow()
    {
        // 즉시 정상 속도 복귀
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        if (Instance != null)
        {
            if (Instance.slowRoutine != null)
                Instance.StopCoroutine(Instance.slowRoutine);

            Destroy(Instance.gameObject);
            // Debug.Log("[TimeSlow] 아이템으로 슬로우 제거");
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
        // 안전 복구
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }
}
