// PoisonGasGimmickSO.cs
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "PoisonGasGimmickSO", menuName = "StageGimmick/PoisonGas")]
public class PoisonGasGimmickSO : StageGimmickSO
{
    public GameObject gasPrefab;
    public float gasDuration = 20f;
    public float interval = 30f;
    public float spawnRadius = 8f;
    public float scaleDuration = 2f;
    public float targetScale = 6f;

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

    public void Init(PoisonGasGimmickSO so, Vector3 origin)
    {
        data = so;
        center = origin;
        routine = StartCoroutine(GasRoutine());
    }

    private IEnumerator GasRoutine()
    {
        while (true)
        {
            // ✅ 위치 생성
            Vector3 randomPos = center + new Vector3(
                Random.Range(-data.spawnRadius, data.spawnRadius),
                0,
                Random.Range(-data.spawnRadius, data.spawnRadius)
            );

            // ✅ 지면 확인
            Vector3 rayOrigin = randomPos + Vector3.up * 20f;
            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 40f))
            {
                Vector3 groundPos = hit.point;

                // ✅ 가스 생성
                GameObject gas = Instantiate(data.gasPrefab, groundPos, Quaternion.identity);
                gas.transform.localScale = Vector3.zero;

                // ✅ PoisonArea 초기화
                PoisonArea area = gas.GetComponent<PoisonArea>();
                if (area != null)
                {
                    area.scaleDuration = data.scaleDuration;
                    area.targetScale = data.targetScale;
                    Debug.Log($"[가스 생성됨] 위치: {groundPos}, 범위: {data.targetScale}");
                }
                else
                {
                    Debug.LogWarning("[가스] PoisonArea 컴포넌트 없음!");
                }

                Destroy(gas, data.gasDuration);
            }
            else
            {
                Debug.LogWarning("[가스] 지면 감지 실패, 생성 스킵");
            }

            yield return new WaitForSeconds(data.interval);
        }
    }


    private void OnDestroy()
    {
        if (routine != null) StopCoroutine(routine);
    }
}