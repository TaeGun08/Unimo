// LightningStrikeGimmickSO.cs (전체 리팩토링 버전)
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "LightningStrikeGimmickSO", menuName = "StageGimmick/LightningStrike")]
public class LightningStrikeGimmickSO : StageGimmickSO
{
    public GameObject markerPrefab;
    public GameObject lightningPrefab;
    public float markerGrowTime = 1.5f;
    public float interval = 5f;
    public float radius = 6f;
    public float stunDuration = 2f;

    // 아이템 생성 관련 설정
    public float itemSpawnInterval = 15f;
    public Vector3 itemSpawnCenter;
    public float itemSpawnRadius = 10f;

    public override GameObject Execute(Vector3 origin)
    {
        var runnerObj = new GameObject("LightningStrikeRunner");
        var runner = runnerObj.AddComponent<LightningStrikeRunner>();
        runner.Init(this, origin);
        return runnerObj;
    }

    private void OnEnable()
    {
        GimmickRegistry.Register(StageGimmickType.LightningStrike, this);
    }
}

public class LightningStrikeRunner : MonoBehaviour
{
    private LightningStrikeGimmickSO data;
    private Vector3 center;
    private Coroutine strikeRoutine;
    private Coroutine itemRoutine;
    private bool strikeInProgress = false;

    // 아이템 관련
    private float itemSpawnInterval;
    private float itemSpawnRadius;
    private Vector3 itemSpawnCenter;
    private GameObject gimmickItemPrefab;

    // 면역 관련
    private bool isImmune = false;
    private Coroutine immuneRoutine;

    public void Init(LightningStrikeGimmickSO so, Vector3 origin)
    {
        data = so;
        center = origin;

        itemSpawnInterval = data.itemSpawnInterval;
        itemSpawnCenter = data.itemSpawnCenter == Vector3.zero ? origin : data.itemSpawnCenter;
        itemSpawnRadius = data.itemSpawnRadius;
        gimmickItemPrefab = data.gimmickItemPrefab;

        strikeRoutine = StartCoroutine(StrikeLoop());
        itemRoutine = StartCoroutine(SpawnItemRoutine());
    }

    private IEnumerator StrikeLoop()
    {
        while (true)
        {
            if (!strikeInProgress)
            {
                Vector3 pos = GetRandomGroundPosition();
                if (pos != Vector3.zero)
                {
                    StartCoroutine(ExecuteStrike(pos));
                }
            }
            yield return new WaitForSeconds(data.interval);
        }
    }

    private IEnumerator ExecuteStrike(Vector3 pos)
    {
        strikeInProgress = true;

        GameObject marker = Instantiate(data.markerPrefab, pos, Quaternion.identity);
        var markerScript = marker.GetComponent<LightningStrikeMarker>();

        if (markerScript != null)
        {
            markerScript.Init(() =>
            {
                Vector3 lightningPos = pos + Vector3.up * 0.1f;
                GameObject lightning = Instantiate(data.lightningPrefab, lightningPos, Quaternion.identity);
                Destroy(lightning, 2f);
                ApplyStun(pos);
            }, data.markerGrowTime);
        }

        yield return new WaitForSeconds(data.markerGrowTime + 0.5f);
        strikeInProgress = false;
    }

    private void ApplyStun(Vector3 center)
    {
        Collider[] hits = Physics.OverlapSphere(center, 1.5f);
        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Player")) continue;

            if (isImmune)
            {
                Debug.Log("[낙뢰] 면역 상태 - 스턴 무효화");
                return;
            }

            var restrictor = hit.GetComponent<StunRestrictor>();
            if (restrictor != null)
            {
                restrictor.TemporarilyDisable(0.35f);
                restrictor.ApplyStun(data.stunDuration);
            }
        }
    }

    private Vector3 GetRandomGroundPosition()
    {
        Vector3 randomPos = transform.position + new Vector3(
            Random.Range(-data.radius, data.radius), 20f, Random.Range(-data.radius, data.radius));

        Ray ray = new Ray(randomPos, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, 50f, LayerMask.GetMask("Ground")))
        {
            return hit.point;
        }
        else
        {
            return Vector3.zero;
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
                item.GetComponent<GimmickItem>()?.Init(StageGimmickType.LightningStrike);
            }
        }
    }

    public void GrantLightningImmunity(float duration)
    {
        if (immuneRoutine != null)
            StopCoroutine(immuneRoutine);

        immuneRoutine = StartCoroutine(ApplyImmunity(duration));
    }

    private IEnumerator ApplyImmunity(float duration)
    {
        isImmune = true;
        Debug.Log("[낙뢰] 면역 상태 시작");

        yield return new WaitForSeconds(duration);

        isImmune = false;
        Debug.Log("[낙뢰] 면역 상태 종료");
    }

    private void OnDestroy()
    {
        if (strikeRoutine != null) StopCoroutine(strikeRoutine);
        if (itemRoutine != null) StopCoroutine(itemRoutine);
    }
}
