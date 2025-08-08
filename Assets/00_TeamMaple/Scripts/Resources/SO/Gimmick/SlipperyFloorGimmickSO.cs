// ✅ SlipperyFloorGimmickSO 및 Runner 수정: 슬리퍼리 면역 체크를 PlayerStatHolder → SlipperyReceiver 로 변경

using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "SlipperyFloorGimmickSO", menuName = "StageGimmick/SlipperyFloor")]
public class SlipperyFloorGimmickSO : StageGimmickSO
{
    public float slipperyDuration = 3f;
    public float slipperyForce = 150f;
    public float maxSlipSpeed = 3f;

    public float itemSpawnInterval = 15f;
    public Vector3 itemSpawnCenter;
    public float itemSpawnRadius = 10f;

    public override GameObject Execute(Vector3 origin)
    {
        var runnerObj = new GameObject("SlipperyFloorRunner");
        var runner = runnerObj.AddComponent<SlipperyFloorRunner>();
        runner.Init(this, origin);
        return runnerObj;
    }

    private void OnEnable()
    {
        GimmickRegistry.Register(StageGimmickType.SlipperyFloor, this);
    }
}

public class SlipperyFloorRunner : MonoBehaviour
{
    private SlipperyFloorGimmickSO data;
    private Coroutine itemRoutine;

    private float itemSpawnInterval;
    private Vector3 itemSpawnCenter;
    private float itemSpawnRadius;
    private GameObject gimmickItemPrefab;

    public static SlipperyFloorRunner Instance { get; private set; }

    public void Init(SlipperyFloorGimmickSO so, Vector3 origin)
    {
        Instance = this;
        data = so;

        itemSpawnInterval = data.itemSpawnInterval;
        itemSpawnCenter = data.itemSpawnCenter == Vector3.zero ? origin : data.itemSpawnCenter;
        itemSpawnRadius = data.itemSpawnRadius;
        gimmickItemPrefab = data.gimmickItemPrefab;

        StartCoroutine(SlipperySequence());
        itemRoutine = StartCoroutine(SpawnItemRoutine());
    }

    private IEnumerator SlipperySequence()
    {
        var player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            var slippery = player.GetComponent<SlipperyReceiver>();

            if (slippery != null && slippery.IsSlipImmune)
            {
                Debug.Log("[Slippery] 면역 상태이므로 슬리퍼리 무시됨");
                Destroy(gameObject);
                yield break;
            }

            if (slippery != null)
            {
                slippery.SetSlippery(
                    enable: true,
                    force: data.slipperyForce,
                    max: data.maxSlipSpeed
                );

                yield return new WaitForSeconds(data.slipperyDuration);

                slippery.SetSlippery(false);
            }
        }

        Destroy(gameObject);
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
                item.GetComponent<GimmickItem>()?.Init(StageGimmickType.SlipperyFloor);
            }
        }
    }

    public static void ApplySlipImmunity(float duration)
    {
        var slippery = LocalPlayer.Instance?.GetComponent<SlipperyReceiver>();
        if (slippery != null)
        {
            slippery.ApplySlipImmune(duration);
            Debug.Log("[Slippery] 슬리퍼리 면역 적용: " + duration + "초");
        }
    }

    private void OnDestroy()
    {
        if (itemRoutine != null) StopCoroutine(itemRoutine);
        if (Instance == this) Instance = null;
    }
}
