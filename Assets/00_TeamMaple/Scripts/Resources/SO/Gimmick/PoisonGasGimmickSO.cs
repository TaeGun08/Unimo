// ✅ PoisonGasGimmickSO 및 Runner 전체 수정 (아이템 생성 및 제거 기능 포함)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PoisonGasGimmickSO", menuName = "StageGimmick/PoisonGas")]
public class PoisonGasGimmickSO : StageGimmickSO
{
    public GameObject gasPrefab;
    public float gasDuration = 60f;
    public float interval = 20f;
    public float spawnRadius = 8f;
    public float scaleDuration = 2f;
    public float targetScale = 3f;
    public float tickInterval = 3f;
    public float percentDamage = 0.1f;
    public float initialDelay = 0f;
    
    public float itemSpawnInterval = 15f;
    public Vector3 itemSpawnCenter;
    public float itemSpawnRadius = 10f;

    public override GameObject Execute(Vector3 origin)
    {
        Debug.Log("[PoisonGas] 기믹 실행됨");

        PoisonGasRunner runner = new GameObject("PoisonGasRunner").AddComponent<PoisonGasRunner>();
        runner.Init(this, origin);
        return runner.gameObject;
    }

    private void OnEnable()
    {
        GimmickRegistry.Register(StageGimmickType.PoisonGas, this);
    }
}

public class PoisonGasRunner : MonoBehaviour
{
    public static PoisonGasRunner Instance { get; private set; }

    private PoisonGasGimmickSO data;
    private Vector3 center;
    private Coroutine routine;
    private Coroutine itemRoutine;
    private List<GameObject> spawnedGases = new();

    private float itemSpawnInterval;
    private Vector3 itemSpawnCenter;
    private float itemSpawnRadius;
    private GameObject gimmickItemPrefab;

    public void Init(PoisonGasGimmickSO so, Vector3 origin)
    {
        Instance = this;
        data = so;
        center = origin;

        itemSpawnInterval = data.itemSpawnInterval;
        itemSpawnCenter = data.itemSpawnCenter == Vector3.zero ? origin : data.itemSpawnCenter;
        itemSpawnRadius = data.itemSpawnRadius;
        gimmickItemPrefab = data.gimmickItemPrefab;

        routine = StartCoroutine(GasRoutine());
        itemRoutine = StartCoroutine(SpawnItemRoutine());
    }

    private IEnumerator GasRoutine()
    {
        yield return new WaitForSeconds(5f);

        while (true)
        {
            spawnedGases.RemoveAll(x => x == null);
            if (spawnedGases.Count >= 3)
            {
                yield return new WaitForSeconds(data.interval);
                continue;
            }

            bool spawned = false;
            int maxAttempts = 20;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                Vector3 randomPos = center + new Vector3(
                    Random.Range(-data.spawnRadius, data.spawnRadius),
                    0,
                    Random.Range(-data.spawnRadius, data.spawnRadius)
                );

                Vector3 rayOrigin = randomPos + Vector3.up * 20f;

                if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 40f))
                {
                    if (!hit.collider.CompareTag("Ground"))
                    {
                        Debug.Log($"[PoisonGasRunner] [{attempt + 1}] Ground 아님 → 재시도");
                        continue;
                    }

                    Vector3 groundPos = hit.point;
                    groundPos.y -= 0.25f;

                    GameObject gas = Instantiate(data.gasPrefab, groundPos, Quaternion.identity);
                    gas.SetActive(true);
                    gas.transform.localScale = Vector3.zero;

                    PoisonArea area = gas.GetComponent<PoisonArea>();
                    if (area != null)
                    {
                        area.Init(
                            tick: data.tickInterval,
                            percent: data.percentDamage,
                            delay: data.initialDelay
                        );

                        area.scaleDuration = data.scaleDuration;
                        area.targetScale = data.targetScale;
                    }

                    spawnedGases.Add(gas);
                    Destroy(gas, data.gasDuration);

                    Debug.Log($"[PoisonGasRunner] 생성 성공 at {groundPos}");
                    spawned = true;
                    break;
                }
            }

            if (!spawned)
            {
                Debug.LogWarning("[PoisonGasRunner] 최대 시도 실패 → 이번 생성 건너뜀");
            }

            yield return new WaitForSeconds(data.interval);
        }
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
                item.GetComponent<GimmickItem>()?.Init(StageGimmickType.PoisonGas);
            }
        }
    }

    public static void RemoveNearbyGas()
    {
        if (Instance == null) return;

        int count = 0;
        foreach (var gas in Instance.spawnedGases)
        {
            if (gas != null && Vector3.Distance(gas.transform.position, LocalPlayer.Instance.transform.position) < 5f)
            {
                Destroy(gas);
                count++;
            }
        }

        Instance.spawnedGases.RemoveAll(g => g == null);

        Debug.Log($"[PoisonGas] 플레이어 주변 가스 {count}개 제거됨");
    }

    private void OnDestroy()
    {
        if (routine != null) StopCoroutine(routine);
        if (itemRoutine != null) StopCoroutine(itemRoutine);
        if (Instance == this) Instance = null;
    }
}