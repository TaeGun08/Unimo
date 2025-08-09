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
    private GameObject pickupEffectPrefab;
    private GameObject durationEffectPrefab;

    private Dictionary<GameObject, float> gasClearTimerDict = new();
    private Dictionary<GameObject, GameObject> durationEffectDict = new();

    public void Init(PoisonGasGimmickSO so, Vector3 origin)
    {
        data = so;
        center = origin;

        itemSpawnInterval = data.itemSpawnInterval;
        itemSpawnCenter = data.itemSpawnCenter == Vector3.zero ? origin : data.itemSpawnCenter;
        itemSpawnRadius = data.itemSpawnRadius;
        gimmickItemPrefab = data.gimmickItemPrefab;
        pickupEffectPrefab = data.pickupEffect;
        durationEffectPrefab = data.durationEffect;

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
        int itemLayer = LayerMask.NameToLayer("Item");
        int itemMask = 1 << itemLayer;

        while (true)
        {
            yield return new WaitForSeconds(itemSpawnInterval);

            Collider[] hits = Physics.OverlapSphere(transform.position, 100f, itemMask);
            bool exists = false;

            foreach (var hit in hits)
            {
                if (hit.GetComponent<GimmickItem>() != null && hit.gameObject.activeInHierarchy)
                {
                    exists = true;
                    break;
                }
            }

            if (exists) continue;

            if (gimmickItemPrefab != null)
            {
                Vector3 randomPos = itemSpawnCenter + new Vector3(
                    Random.Range(-itemSpawnRadius, itemSpawnRadius),
                    0.5f,
                    Random.Range(-itemSpawnRadius, itemSpawnRadius)
                );

                var item = Instantiate(gimmickItemPrefab, randomPos, Quaternion.Euler(-90f, 0f, 0f));
                item.layer = itemLayer;
                item.GetComponent<GimmickItem>()?.Init(StageGimmickType.PoisonGas, this, pickupEffectPrefab, durationEffectPrefab, 30f);
            }
        }
    }

    private void Update()
    {
        if (gasClearTimerDict.Count == 0) return;

        List<GameObject> expired = new();

        foreach (var kvp in new Dictionary<GameObject, float>(gasClearTimerDict))
        {
            GameObject player = kvp.Key;
            float timeLeft = kvp.Value - Time.deltaTime;

            bool removeNow = false;

            if (timeLeft <= 0f)
            {
                removeNow = true;
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

                        removeNow = true;
                        break;
                    }
                }
            }

            if (removeNow)
            {
                expired.Add(player);

                if (durationEffectDict.TryGetValue(player, out var effect))
                {
                    var ps = effect.GetComponent<ParticleSystem>();
                    if (ps != null) ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);

                    Destroy(effect);
                    durationEffectDict.Remove(player);
                    Debug.Log("[PoisonGas] 지속 이펙트 제거 완료");
                }
            }
        }

        foreach (var p in expired)
            gasClearTimerDict.Remove(p);
    }

    public static void ApplyTemporaryGasClear(float duration)
    {
        var runner = FindObjectOfType<PoisonGasRunner>();
        if (runner == null) return;

        var player = LocalPlayer.Instance?.gameObject;
        if (player == null) return;

        if (runner.gasClearTimerDict.TryGetValue(player, out float existing))
        {
            if (existing >= duration)
            {
                Debug.Log("[PoisonGas] 기존 지속시간이 더 김 → 재적용 안 함");
                return;
            }
            runner.gasClearTimerDict[player] = duration;
        }
        else
        {
            runner.gasClearTimerDict.Add(player, duration);
        }

        Debug.Log($"[PoisonGas] {duration}초간 가스 제거 권한 부여");

        if (runner.pickupEffectPrefab != null)
        {
            Debug.Log($"[PoisonGas] 픽업 이펙트 생성: {runner.pickupEffectPrefab.name}");
            GameObject vfx = Instantiate(runner.pickupEffectPrefab, player.transform.position + Vector3.up * 1.5f, Quaternion.identity);
            Destroy(vfx, 2f);
        }

        if (runner.durationEffectPrefab != null && !runner.durationEffectDict.ContainsKey(player))
        {
            Debug.Log($"[PoisonGas] 지속 이펙트 생성: {runner.durationEffectPrefab.name}");
            GameObject effect = Instantiate(runner.durationEffectPrefab, player.transform);
            effect.transform.localPosition = Vector3.up * 1.5f;
            runner.durationEffectDict.Add(player, effect);
        }
    }

    private void OnDestroy()
    {
        if (routine != null) StopCoroutine(routine);
        if (itemRoutine != null) StopCoroutine(itemRoutine);
    }
}
