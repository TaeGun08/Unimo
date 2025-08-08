using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "DarknessGimmickSO", menuName = "StageGimmick/Darkness")]
public class DarknessGimmickSO : StageGimmickSO
{
    public float duration = 20f;
    public float interval = 30f;

    public float itemSpawnInterval = 15f;
    public Vector3 itemSpawnCenter;
    public float itemSpawnRadius = 10f;

    public GameObject darknessPrefab;

    public override GameObject Execute(Vector3 origin)
    {
        var runnerObj = new GameObject("DarknessCycleRunner");
        var runner = runnerObj.AddComponent<DarknessCycleRunner>();
        runner.Init(this, origin);
        return runnerObj;
    }

    private void OnEnable()
    {
        GimmickRegistry.Register(StageGimmickType.Darkness, this);
    }
}

public class DarknessCycleRunner : MonoBehaviour
{
    private DarknessGimmickSO data;
    private GameObject currentRunner;
    private Coroutine cycleRoutine;
    private Coroutine itemRoutine;

    private Vector3 itemSpawnCenter;
    private float itemSpawnRadius;

    public void Init(DarknessGimmickSO so, Vector3 origin)
    {
        data = so;
        itemSpawnCenter = data.itemSpawnCenter == Vector3.zero ? origin : data.itemSpawnCenter;
        itemSpawnRadius = data.itemSpawnRadius;

        cycleRoutine = StartCoroutine(RunDarknessCycle());
        itemRoutine = StartCoroutine(SpawnItemRoutine());
    }

    private IEnumerator RunDarknessCycle()
    {
        yield return new WaitForSeconds(5f);

        while (true)
        {
            if (currentRunner != null)
                Destroy(currentRunner);

            currentRunner = new GameObject("DarknessRunner");
            var runner = currentRunner.AddComponent<DarknessRunner>();
            runner.Init(data);

            yield return new WaitForSeconds(data.interval);
        }
    }

    private IEnumerator SpawnItemRoutine()
    {
        int itemLayer = LayerMask.NameToLayer("Item");
        int itemMask = 1 << itemLayer;

        while (true)
        {
            yield return new WaitForSeconds(data.itemSpawnInterval);

            Collider[] hits = Physics.OverlapSphere(transform.position, 100f, itemMask);
            bool anyValid = false;

            foreach (var hit in hits)
            {
                if (hit.isTrigger && hit.gameObject.activeInHierarchy)
                {
                    anyValid = true;
                    break;
                }
            }

            if (anyValid)
                continue;

            if (data.gimmickItemPrefab != null)
            {
                Vector3 randomPos = itemSpawnCenter + new Vector3(
                    Random.Range(-itemSpawnRadius, itemSpawnRadius),
                    0.5f,
                    Random.Range(-itemSpawnRadius, itemSpawnRadius)
                );

                var item = Instantiate(data.gimmickItemPrefab, randomPos, Quaternion.Euler(-90, 0, 0));
                item.layer = itemLayer;
                item.GetComponent<GimmickItem>()?.Init(StageGimmickType.Darkness, this, data.pickupEffect, null, 0f);
            }
        }
    }

    private void OnDestroy()
    {
        if (cycleRoutine != null) StopCoroutine(cycleRoutine);
        if (itemRoutine != null) StopCoroutine(itemRoutine);
    }
}

public class DarknessRunner : MonoBehaviour
{
    public static DarknessRunner Instance { get; private set; }

    private float duration;
    private GameObject darknessPrefab;
    private GameObject darknessInstance;
    private Material darknessMaterial;
    private Transform player;

    private float timer = 0f;
    private bool suppressed = false;

    public void Init(DarknessGimmickSO data)
    {
        Instance = this;

        duration = data.duration;
        darknessPrefab = data.darknessPrefab;

        player = LocalPlayer.Instance.transform;

        if (darknessPrefab == null)
        {
            Debug.LogWarning("[Darkness] 프리팹 누락");
            return;
        }

        darknessInstance = Instantiate(darknessPrefab);
        darknessInstance.name = "DarknessInstance";
        darknessInstance.transform.SetParent(player);
        darknessInstance.transform.localPosition = Vector3.zero;
        darknessInstance.transform.localRotation = Quaternion.identity;
        darknessInstance.transform.localScale = new Vector3(100f, 0.1f, 100f);

        var renderer = darknessInstance.GetComponent<Renderer>();
        if (renderer != null)
        {
            darknessMaterial = renderer.material;
        }
    }

    public void Suppress()
    {
        suppressed = true;
        if (darknessInstance != null)
        {
            Destroy(darknessInstance);
            Debug.Log("[Darkness] 어둠 효과 제거됨");
        }
    }

    private void Update()
    {
        if (suppressed) return;

        timer += Time.deltaTime;
        if (timer > duration)
        {
            Destroy(darknessInstance);
            Destroy(gameObject);
            return;
        }

        if (darknessMaterial != null && player != null)
        {
            darknessMaterial.SetVector("_PlayerWorldPosition", player.position);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
