// MeteorFallGimmickSO.cs
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "MeteorFallGimmickSO", menuName = "StageGimmick/MeteorFall")]
public class MeteorFallGimmickSO : StageGimmickSO
{
    public GameObject meteorPrefab;
    public GameObject markerPrefab;
    public float interval = 5f;
    public float spawnRadius = 10f;
    public Vector3 fallDirection = new Vector3(0.5f, -1f, 0f);
    public float fallHeight = 50f;
    public float fallSpeed = 20f;
    

    public override GameObject Execute(Vector3 origin)
    {
        GameObject runnerObj = new GameObject("MeteorGimmickRunner");
        MeteorFallRunner runner = runnerObj.AddComponent<MeteorFallRunner>();
        runner.Init(this, origin);
        return runnerObj;
    }
    
    private void OnEnable()
    {
        GimmickRegistry.Register(StageGimmickType.MeteorFall, this);
    }
}

public class MeteorFallRunner : MonoBehaviour
{
    private MeteorFallGimmickSO data;
    private Vector3 center;
    private Coroutine routine;
    private List<GameObject> activeMeteors = new();
    private int maxMeteors = 1;

    public void Init(MeteorFallGimmickSO so, Vector3 origin)
    {
        data = so;
        center = origin;
        routine = StartCoroutine(FallRoutine());
    }

    private IEnumerator FallRoutine()
    {
        WaitForSeconds intervalWait = new WaitForSeconds(data.interval);

        while (true)
        {
            if (activeMeteors.Count < maxMeteors)
            {
                Vector3? pos = FindGroundPosition();

                if (pos.HasValue)
                    SpawnMeteor(pos.Value);
                else
                    Debug.LogWarning("[메테오] 지면을 못 찾아 생성 스킵");

                yield return null; // Raycast 시도 후 프레임 쉬기
            }

            yield return intervalWait;
        }
    }

    // ✅ 지면 탐색 시도 제한 및 yield 포함
    private Vector3? FindGroundPosition()
    {
        const int maxAttempts = 10;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 randomXZ = center + new Vector3(
                Random.Range(-data.spawnRadius, data.spawnRadius),
                0f,
                Random.Range(-data.spawnRadius, data.spawnRadius)
            );

            Vector3 rayOrigin = randomXZ + Vector3.up * 50f;

            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 100f, LayerMask.GetMask("Ground")))
            {
                return hit.point;
            }
        }

        return null;
    }

    private void SpawnMeteor(Vector3 groundPos)
    {
        Vector3 fallDir = data.fallDirection.normalized;

        if (fallDir.y >= 0f)
        {
            Debug.LogWarning("[Meteor] fallDirection.y는 반드시 음수여야 합니다.");
            fallDir.y = -0.01f;
        }

        float distance = data.fallHeight / -fallDir.y;
        Vector3 offset = fallDir * distance;
        Vector3 spawnPos = groundPos - offset;

        float fallDistance = offset.magnitude;
        float markerDuration = fallDistance / data.fallSpeed;

        // ✅ 마커 생성
        GameObject marker = Instantiate(data.markerPrefab, groundPos, Quaternion.identity);
        MeteorMarker markerComp = marker.GetComponent<MeteorMarker>();
        if (markerComp != null)
        {
            markerComp.duration = markerDuration;
        }

        // ✅ 메테오 생성
        GameObject meteor = Instantiate(data.meteorPrefab, spawnPos, Quaternion.identity);
        activeMeteors.Add(meteor);

        Debug.Log($"[메테오 생성] 위치: {spawnPos} / 지면: {groundPos} / 거리: {fallDistance}");
    }

    public void NotifyMeteorDestroyed(GameObject meteor)
    {
        if (activeMeteors.Contains(meteor))
        {
            activeMeteors.Remove(meteor);
        }
    }

    private void OnDestroy()
    {
        if (routine != null) StopCoroutine(routine);
    }
}
