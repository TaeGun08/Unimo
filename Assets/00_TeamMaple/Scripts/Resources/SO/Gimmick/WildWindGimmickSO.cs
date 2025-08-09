// ✅ WildWindGimmickSO — 아이템 1개 제한 + 픽업/지속 이펙트 + 면역 시 즉시 바람 제거 + 실행순서/속도강제
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "WildWindGimmickSO", menuName = "StageGimmick/WildWind")]
public class WildWindGimmickSO : StageGimmickSO
{
    [Header("바람 설정")]
    public float delayBeforeStart = 5f;
    public float windDuration = 20f;
    public float windInterval = 30f;
    public Vector3 windDirection = Vector3.left;
    public AnimationCurve decayCurve = AnimationCurve.Linear(0, 1f, 1, 0f);
    public GameObject windVisualPrefab;
    public bool alternateDirection = true;
    public float windForce = 10f;
    public float groundClampRadius = 15f;

    [Header("아이템 & 이펙트")]
    public float itemSpawnInterval = 15f;
    public Vector3 itemSpawnCenter;
    public float itemSpawnRadius = 10f;
    public float itemImmuneDuration = 20f; // 면역/지속 이펙트 시간

    public override GameObject Execute(Vector3 origin)
    {
        var go = new GameObject("WildWindRunner");
        var runner = go.AddComponent<WildWindRunner>();
        runner.Init(this, origin);
        return go;
    }

    private void OnEnable()
    {
        GimmickRegistry.Register(StageGimmickType.WildWind, this);
    }
}

[DefaultExecutionOrder(1000)] // ✅ 플레이어 이동 이후에 실행되어 최종 속도 덮어쓰기
public class WildWindRunner : MonoBehaviour
{
    private WildWindGimmickSO data;

    // 플레이어/물리
    private Transform player;
    private Rigidbody playerRb;

    // 바람 상태
    private float timer = 0f;
    private bool windActive = false;
    private Vector3 currentDirection;
    private GameObject windVisual;

    public static Vector3 CurrentWindDirection { get; private set; } = Vector3.zero;

    // 아이템 스폰
    private float itemSpawnInterval;
    private Vector3 itemSpawnCenter;
    private float itemSpawnRadius;
    private GameObject gimmickItemPrefab;
    private Coroutine itemRoutine;

    // 면역
    private bool isImmune = false;
    private Coroutine immuneRoutine;

    public void Init(WildWindGimmickSO so, Vector3 origin)
    {
        data = so;

        itemSpawnInterval = data.itemSpawnInterval;
        itemSpawnCenter   = data.itemSpawnCenter == Vector3.zero ? origin : data.itemSpawnCenter;
        itemSpawnRadius   = data.itemSpawnRadius;
        gimmickItemPrefab = data.gimmickItemPrefab;

        StartCoroutine(WindRoutine());
        itemRoutine = StartCoroutine(SpawnItemRoutine());
    }

    private IEnumerator WindRoutine()
    {
        // 시작 지연
        yield return new WaitForSeconds(data.delayBeforeStart);

        player = LocalPlayer.Instance != null ? LocalPlayer.Instance.transform : null;
        if (player == null) yield break;

        // 루트가 아니라 자식에 붙었을 가능성까지 커버
        playerRb = player.GetComponent<Rigidbody>() ?? player.GetComponentInChildren<Rigidbody>();
        if (playerRb == null)
        {
            Debug.LogError("[WildWind] Rigidbody를 찾지 못했습니다.");
            yield break;
        }

        currentDirection = data.windDirection.normalized;

        while (true)
        {
            CurrentWindDirection = currentDirection;

            // 비주얼
            if (data.windVisualPrefab != null)
            {
                Quaternion rot = (currentDirection.x < 0)
                    ? Quaternion.Euler(90f, 180f, 0f)
                    : Quaternion.Euler(90f, 0f, 0f);
                windVisual = Instantiate(data.windVisualPrefab, player.position + Vector3.up * 2f, rot);
            }

            // 바람 구간
            timer = 0f;
            windActive = true;
            while (timer < data.windDuration)
            {
                timer += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }

            // 종료 처리
            windActive = false;
            CurrentWindDirection = Vector3.zero;
            if (windVisual) Destroy(windVisual);

            // 잔여 관성 제거(수평 0)
            ZeroHorizontalVelocity();

            // 방향 반전
            if (data.alternateDirection) currentDirection *= -1;

            // 다음 사이클까지 대기
            float waitTime = Mathf.Max(0f, data.windInterval - data.windDuration);
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void FixedUpdate()
    {
        if (!windActive || playerRb == null || isImmune) return;

        // 감쇠 곡선에 따른 목표 수평 속도
        float t = Mathf.Clamp01(timer / data.windDuration);
        float k = data.decayCurve.Evaluate(t);
        Vector3 desired = CurrentWindDirection * (k * data.windForce);

#if UNITY_6_0_OR_NEWER
        var v = playerRb.linearVelocity;
        v.x = desired.x; v.z = desired.z;
        playerRb.linearVelocity = v;          // ✅ 최종 덮어쓰기
#else
        var v = playerRb.linearVelocity;
        v.x = desired.x; v.z = desired.z;
        playerRb.linearVelocity = v;                // ✅ 최종 덮어쓰기
#endif

        // 경계 클램프(옵션)
        Vector3 center = Vector3.zero;
        Vector3 offset = player.position - center;
        if (offset.magnitude > data.groundClampRadius)
        {
            player.position = center + offset.normalized * data.groundClampRadius;
        }
    }

    private void ZeroHorizontalVelocity()
    {
        if (playerRb == null) return;
#if UNITY_6_0_OR_NEWER
        var v = playerRb.linearVelocity; v.x = 0f; v.z = 0f; playerRb.linearVelocity = v;
#else
        var v = playerRb.linearVelocity; v.x = 0f; v.z = 0f; playerRb.linearVelocity = v;
#endif
    }

    // ───────── 아이템 “최대 1개” 스폰 + 이펙트 프리팹/지속시간 전달 ─────────
    private IEnumerator SpawnItemRoutine()
    {
        int itemLayer = LayerMask.NameToLayer("Item");
        int itemMask  = 1 << itemLayer;

        while (true)
        {
            yield return new WaitForSeconds(itemSpawnInterval);

            // 필드에 이미 있으면 스킵
            Collider[] hits = Physics.OverlapSphere(itemSpawnCenter, itemSpawnRadius * 2f, itemMask);
            bool any = false;
            foreach (var h in hits)
            {
                if (h && h.GetComponent<GimmickItem>() && h.gameObject.activeInHierarchy) { any = true; break; }
            }
            if (any) continue;

            if (gimmickItemPrefab != null)
            {
                Vector3 pos = itemSpawnCenter + new Vector3(
                    Random.Range(-itemSpawnRadius, itemSpawnRadius),
                    0.5f,
                    Random.Range(-itemSpawnRadius, itemSpawnRadius)
                );
                var item = Instantiate(gimmickItemPrefab, pos, Quaternion.Euler(-90f, 0f, 0f));
                item.layer = itemLayer;

                // WildWind는 GimmickItem이 이펙트(픽업/지속 1개) 붙임
                item.GetComponent<GimmickItem>()?.Init(
                    StageGimmickType.WildWind,
                    this,
                    data.pickupEffect,
                    data.durationEffect,
                    data.itemImmuneDuration
                );
            }
        }
    }

    // ───────── 아이템 효과: 면역 적용 (즉시 영향 제거 + 면역 중 미적용) ─────────
    public static void ApplyWindResist(float duration)
    {
        var r = FindObjectOfType<WildWindRunner>();
        if (r != null) r.SetImmune(duration);
    }

    private void SetImmune(float duration)
    {
        if (immuneRoutine != null) StopCoroutine(immuneRoutine);

        // 먹는 순간 즉시 영향 제거
        if (windVisual) { Destroy(windVisual); windVisual = null; }
        CurrentWindDirection = Vector3.zero;
        ZeroHorizontalVelocity();

        immuneRoutine = StartCoroutine(WindImmuneRoutine(duration));
    }

    private IEnumerator WindImmuneRoutine(float duration)
    {
        isImmune = true;
        Debug.Log($"[WildWind] 면역 {duration}초 시작");
        yield return new WaitForSeconds(duration);
        isImmune = false;
        immuneRoutine = null;
        Debug.Log("[WildWind] 면역 종료");
    }

    private void OnDestroy()
    {
        if (itemRoutine != null) StopCoroutine(itemRoutine);
    }
}
