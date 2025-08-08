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
        var obj = new GameObject("DarknessCycleRunner");
        var runner = obj.AddComponent<DarknessCycleRunner>();
        runner.duration = duration;
        runner.interval = interval;
        runner.itemSpawnInterval = itemSpawnInterval;
        runner.darknessPrefab = darknessPrefab;
        runner.gimmickItemPrefab = gimmickItemPrefab;
        runner.itemSpawnCenter = itemSpawnCenter == Vector3.zero ? origin : itemSpawnCenter;
        runner.itemSpawnRadius = itemSpawnRadius;
        runner.Init();
        return obj;
    }

    private void OnEnable()
    {
        GimmickRegistry.Register(StageGimmickType.Darkness, this);
    }
}

public class DarknessCycleRunner : MonoBehaviour
{
    public float duration;
    public float interval;
    public float itemSpawnInterval;

    public GameObject darknessPrefab;
    public GameObject gimmickItemPrefab;
    public Vector3 itemSpawnCenter;
    public float itemSpawnRadius;

    private GameObject currentRunner;
    private Coroutine cycleRoutine;

    public void Init()
    {
        cycleRoutine = StartCoroutine(RunDarknessCycle());
        StartCoroutine(SpawnItemRoutine());
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
            runner.duration = duration;
            runner.darknessPrefab = darknessPrefab;
            runner.Init();

            yield return new WaitForSeconds(interval);
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
                item.GetComponent<GimmickItem>()?.Init(StageGimmickType.Darkness);
            }
        }
    }

    private void OnDestroy()
    {
        if (cycleRoutine != null)
            StopCoroutine(cycleRoutine);
    }
}

public class DarknessRunner : MonoBehaviour
{
    public static DarknessRunner Instance { get; private set; }

    public float duration = 15f;
    public GameObject darknessPrefab;

    private GameObject darknessInstance;
    private Material darknessMaterial;
    private Transform player;
    private float timer = 0f;
    private bool suppressed = false;

    public void Init()
    {
        Instance = this;

        player = LocalPlayer.Instance.transform;

        if (darknessPrefab == null)
        {
            Debug.LogWarning("❌ darknessPrefab is null!");
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
