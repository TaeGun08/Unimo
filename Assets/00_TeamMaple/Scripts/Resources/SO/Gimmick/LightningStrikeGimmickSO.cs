// LightningStrikeGimmickSO.cs (수정 버전)
using System;
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
    public float stunDuration = 2f; // ✅ SO에서 조정 가능한 스턴 시간

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
    private Coroutine routine;
    private bool strikeInProgress = false;

    public void Init(LightningStrikeGimmickSO so, Vector3 origin)
    {
        data = so;
        center = origin;
        routine = StartCoroutine(StrikeLoop());
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

            var restrictor = hit.GetComponent<StunRestrictor>();
            if (restrictor != null)
            {
                restrictor.ApplyStun(data.stunDuration);
                Debug.Log("[낙뢰 스턴] 이동 제한 적용됨");
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
            Debug.Log($"[낙뢰 Raycast 성공] hit: {hit.point} / normal: {hit.normal} / target: {hit.collider.gameObject.name}");
            return hit.point;
        }
        else
        {
            Debug.LogWarning("[낙뢰 Raycast 실패] 레이: " + ray.origin);
            return Vector3.zero;
        }
    }

    private void OnDestroy()
    {
        if (routine != null) StopCoroutine(routine);
    }
}
