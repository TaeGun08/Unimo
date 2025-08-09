using System.Collections;
using System.Collections.Generic;
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

    public float itemSpawnInterval = 15f;
    public Vector3 itemSpawnCenter;
    public float itemSpawnRadius = 10f;

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
    private Coroutine itemRoutine;
    private List<GameObject> activeMeteors = new();
    private int maxMeteors = 1;

    private float itemSpawnInterval;
    private Vector3 itemSpawnCenter;
    private float itemSpawnRadius;
    private GameObject gimmickItemPrefab;
    private GameObject pickupEffectPrefab;

    public void Init(MeteorFallGimmickSO so, Vector3 origin)
    {
        data = so;
        center = origin;

        var statHolder = LocalPlayer.Instance.PlayerStatHolder;
        int bonusHp = Mathf.CeilToInt(statHolder.Hp.MaxValue * 0.1f);
        statHolder.Hp.Add(bonusHp);
        Debug.Log($"[Meteor] 추가 체력 부여됨: {bonusHp}");

        itemSpawnInterval = data.itemSpawnInterval;
        itemSpawnCenter = data.itemSpawnCenter == Vector3.zero ? origin : data.itemSpawnCenter;
        itemSpawnRadius = data.itemSpawnRadius;
        gimmickItemPrefab = data.gimmickItemPrefab;
        pickupEffectPrefab = data.pickupEffect;

        routine = StartCoroutine(FallRoutine());
        itemRoutine = StartCoroutine(SpawnItemRoutine());
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

                yield return null;
            }

            yield return intervalWait;
        }
    }

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

        GameObject marker = Instantiate(data.markerPrefab, groundPos, Quaternion.identity);
        MeteorMarker markerComp = marker.GetComponent<MeteorMarker>();
        if (markerComp != null)
        {
            markerComp.duration = markerDuration;
        }

        GameObject meteor = Instantiate(data.meteorPrefab, spawnPos, Quaternion.identity);
        activeMeteors.Add(meteor);

        Debug.Log($"[메테오 생성] 위치: {spawnPos} / 지면: {groundPos} / 거리: {fallDistance}");
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
                item.GetComponent<GimmickItem>()?.Init(StageGimmickType.MeteorFall, this, pickupEffectPrefab, null, 0f);
            }
        }
    }

    public static void RemoveBurning()
    {
        Debug.Log("[Meteor] 화상 해제 요청됨");

        var player = LocalPlayer.Instance;

        if (player != null && player.ActiveDotCoroutine != null)
        {
            player.StopCoroutine(player.ActiveDotCoroutine);
            player.ActiveDotCoroutine = null;

            Debug.Log("[Meteor] 화상 DOT 해제 완료");
        }
        else
        {
            Debug.Log("[Meteor] DOT 상태가 없거나 이미 해제됨");
        }
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
        if (itemRoutine != null) StopCoroutine(itemRoutine);
    }
}
