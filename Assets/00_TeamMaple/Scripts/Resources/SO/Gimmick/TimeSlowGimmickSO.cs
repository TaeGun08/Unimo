// ✅ TimeSlowGimmickSO 및 Runner 수정: 아이템 생성 추가, 빨라지는 효과 제거, 아이템 먹으면 10초간 느려짐 면역 적용

using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "TimeSlowGimmickSO", menuName = "StageGimmick/TimeSlow")]
public class TimeSlowGimmickSO : StageGimmickSO
{
    public float slowTimeScale = 0.5f;
    public float slowDuration = 5f;
    public float normalDuration = 3f;

    public float itemSpawnInterval = 15f;
    public Vector3 itemSpawnCenter;
    public float itemSpawnRadius = 10f;

    public override GameObject Execute(Vector3 origin)
    {
        var obj = new GameObject("TimeSlowCycleRunner");
        var runner = obj.AddComponent<TimeSlowCycleRunner>();
        runner.so = this;
        runner.Init(origin);
        return obj;
    }

    private void OnEnable()
    {
        GimmickRegistry.Register(StageGimmickType.TimeSlow, this);
    }
}

public class TimeSlowCycleRunner : MonoBehaviour
{
    public TimeSlowGimmickSO so;

    private float timer = 0f;
    private int stateIndex = 0;

    private float[] timeScales;
    private float[] durations;

    private float originalTimeScale;
    private float originalFixedDelta;

    private float itemSpawnInterval;
    private Vector3 itemSpawnCenter;
    private float itemSpawnRadius;
    private GameObject gimmickItemPrefab;
    private Coroutine itemRoutine;

    private bool isImmune = false;
    private Coroutine immuneRoutine;

    public void Init(Vector3 origin)
    {
        originalTimeScale = Time.timeScale;
        originalFixedDelta = Time.fixedDeltaTime;

        // 느려짐 + 정상 반복만 남김
        timeScales = new float[]
        {
            so.slowTimeScale,
            1f
        };

        durations = new float[]
        {
            so.slowDuration,
            so.normalDuration
        };

        ApplyState(0);

        itemSpawnInterval = so.itemSpawnInterval;
        itemSpawnCenter = so.itemSpawnCenter == Vector3.zero ? origin : so.itemSpawnCenter;
        itemSpawnRadius = so.itemSpawnRadius;
        gimmickItemPrefab = so.gimmickItemPrefab;

        itemRoutine = StartCoroutine(SpawnItemRoutine());
    }

    private void Update()
    {
        timer += Time.unscaledDeltaTime;

        if (timer >= durations[stateIndex])
        {
            timer = 0f;
            stateIndex = (stateIndex + 1) % timeScales.Length;
            ApplyState(stateIndex);
        }
    }

    private void ApplyState(int index)
    {
        float scale = timeScales[index];

        // 면역이면 느려짐 무시
        if (scale < 1f && isImmune)
        {
            Debug.Log("[TimeSlow] 느려짐 면역 상태 - 정상 속도 유지");
            scale = 1f;
        }

        Time.timeScale = scale;
        Time.fixedDeltaTime = 0.02f * scale;

        Debug.Log($"[Time] 상태 {index}: 배속 {scale}");
    }

    private IEnumerator SpawnItemRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(itemSpawnInterval);

            if (gimmickItemPrefab != null)
            {
                Vector3 randomPos = itemSpawnCenter + new Vector3(
                    Random.Range(-itemSpawnRadius, itemSpawnRadius),
                    0.5f,
                    Random.Range(-itemSpawnRadius, itemSpawnRadius)
                );

                var item = Instantiate(gimmickItemPrefab, randomPos, Quaternion.identity);
                item.GetComponent<GimmickItem>()?.Init(StageGimmickType.TimeSlow);
            }
        }
    }

    public static void ApplyTimeSlowImmune(float duration)
    {
        var runner = FindObjectOfType<TimeSlowCycleRunner>();
        if (runner != null)
        {
            runner.SetImmune(duration);
        }
    }

    private void SetImmune(float duration)
    {
        if (immuneRoutine != null)
            StopCoroutine(immuneRoutine);

        immuneRoutine = StartCoroutine(TimeSlowImmuneRoutine(duration));
    }

    private IEnumerator TimeSlowImmuneRoutine(float duration)
    {
        isImmune = true;
        Debug.Log($"[TimeSlow] 느려짐 면역 {duration}초 시작");

        yield return new WaitForSeconds(duration);

        isImmune = false;
        immuneRoutine = null;
        Debug.Log("[TimeSlow] 느려짐 면역 종료");
    }

    private void OnDestroy()
    {
        Time.timeScale = originalTimeScale;
        Time.fixedDeltaTime = originalFixedDelta;
        if (itemRoutine != null) StopCoroutine(itemRoutine);
    }
}