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
    private PoisonGasGimmickSO data;
    private Vector3 center;
    private Coroutine routine;
    private Coroutine itemRoutine;
    private List<GameObject> spawnedGases = new();

    private float itemSpawnInterval;
    private Vector3 itemSpawnCenter;
    private float itemSpawnRadius;
    private GameObject gimmickItemPrefab;

    // ✅ 신규: 플레이어별 가스 제거 권한 타이머
    private Dictionary<GameObject, float> gasClearTimerDict = new();

    // ✅ (선택) 아이템 획득 시 이펙트
    public GameObject itemPickupEffect;

    public void Init(PoisonGasGimmickSO so, Vector3 origin)
    {
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

            Vector3 groundPos = GetValidGroundPosition();
            if (groundPos != Vector3.zero)
            {
                GameObject gas = Instantiate(data.gasPrefab, groundPos, Quaternion.identity);
                gas.SetActive(true);
                gas.transform.localScale = Vector3.zero;

                PoisonArea area = gas.GetComponent<PoisonArea>();
                if (area != null)
                {
                    area.Init(data.tickInterval, data.percentDamage, data.initialDelay);
                    area.scaleDuration = data.scaleDuration;
                    area.targetScale = data.targetScale;
                }

                spawnedGases.Add(gas);
                Destroy(gas, data.gasDuration);
                Debug.Log($"[PoisonGasRunner] 가스 생성 at {groundPos}");
            }

            yield return new WaitForSeconds(data.interval);
        }
    }

    private Vector3 GetValidGroundPosition()
    {
        for (int i = 0; i < 20; i++)
        {
            Vector3 pos = center + new Vector3(
                Random.Range(-data.spawnRadius, data.spawnRadius),
                0f,
                Random.Range(-data.spawnRadius, data.spawnRadius)
            );

            Vector3 rayOrigin = pos + Vector3.up * 20f;
            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 40f) &&
                hit.collider.CompareTag("Ground"))
            {
                Vector3 groundPos = hit.point;
                groundPos.y -= 0.25f;
                return groundPos;
            }
        }
        return Vector3.zero;
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

    private void Update()
    {
        if (gasClearTimerDict.Count == 0) return;

        List<GameObject> expired = new();

        foreach (var kvp in gasClearTimerDict)
        {
            GameObject player = kvp.Key;
            float timeLeft = kvp.Value - Time.deltaTime;

            if (timeLeft <= 0f)
            {
                expired.Add(player);
            }
            else
            {
                gasClearTimerDict[player] = timeLeft;

                foreach (var gas in spawnedGases.ToArray())
                {
                    if (gas == null) continue;
                    if (Vector3.Distance(gas.transform.position, player.transform.position) < 5f)
                    {
                        Destroy(gas);
                        spawnedGases.Remove(gas);
                        Debug.Log("[PoisonGas] 가스 제거: 플레이어 근처 접근");
                    }
                }
            }
        }

        foreach (var p in expired)
            gasClearTimerDict.Remove(p);
    }

    // ✅ 외부에서 호출: 아이템 획득 시 30초 타이머 시작
    public static void ApplyTemporaryGasClear(float duration)
    {
        var runner = FindObjectOfType<PoisonGasRunner>();
        if (runner == null) return;

        var player = LocalPlayer.Instance?.gameObject;
        if (player == null) return;

        if (runner.gasClearTimerDict.ContainsKey(player))
            runner.gasClearTimerDict[player] = duration;
        else
            runner.gasClearTimerDict.Add(player, duration);

        Debug.Log($"[PoisonGas] {duration}초간 가스 제거 권한 부여");

        // ✅ (선택) 이펙트 표시
        if (runner.itemPickupEffect != null)
        {
            GameObject vfx = Instantiate(runner.itemPickupEffect, player.transform.position + Vector3.up * 1.5f, Quaternion.identity);
            Destroy(vfx, 2f);
        }
    }

    private void OnDestroy()
    {
        if (routine != null) StopCoroutine(routine);
        if (itemRoutine != null) StopCoroutine(itemRoutine);
    }
}
