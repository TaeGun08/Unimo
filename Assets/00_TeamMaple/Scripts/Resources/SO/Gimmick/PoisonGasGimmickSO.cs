// PoisonGasGimmickSO.cs
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
    private List<GameObject> spawnedGases = new();

    public void Init(PoisonGasGimmickSO so, Vector3 origin)
    {
        data = so;
        center = origin;
        routine = StartCoroutine(GasRoutine());
    }

    private IEnumerator GasRoutine()
{
    yield return new WaitForSeconds(5f); // 첫 생성까지 대기

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
                groundPos.y -= 0.25f; // 위치 보정

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



    private void OnDestroy()
    {
        if (routine != null) StopCoroutine(routine);
    }
}
