using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "EarthquakeGimmickSO", menuName = "StageGimmick/Earthquake")]
public class EarthquakeGimmickSO : StageGimmickSO
{
    public float shakeDuration = 5f;
    public float interval = 30f;
    public float intensity = 0.3f;

    public float itemSpawnInterval = 15f;
    public Vector3 itemSpawnCenter;
    public float itemSpawnRadius = 10f;

    public override GameObject Execute(Vector3 origin)
    {
        var obj = new GameObject("EarthquakeCycleRunner");
        var runner = obj.AddComponent<EarthquakeCycleRunner>();
        runner.Init(this, origin);
        return obj;
    }

    private void OnEnable()
    {
        GimmickRegistry.Register(StageGimmickType.Earthquake, this);
    }
}

public class EarthquakeCycleRunner : MonoBehaviour
{
    private EarthquakeGimmickSO data;
    private GameObject currentRunner;
    private Coroutine cycleRoutine;
    private Coroutine itemRoutine;

    private Vector3 itemSpawnCenter;
    private float itemSpawnRadius;

    public void Init(EarthquakeGimmickSO so, Vector3 origin)
    {
        data = so;
        itemSpawnCenter = data.itemSpawnCenter == Vector3.zero ? origin : data.itemSpawnCenter;
        itemSpawnRadius = data.itemSpawnRadius;

        cycleRoutine = StartCoroutine(RunEarthquakeCycle());
        itemRoutine = StartCoroutine(SpawnItemRoutine());
    }

    private IEnumerator RunEarthquakeCycle()
    {
        yield return new WaitForSeconds(5f);

        while (true)
        {
            if (currentRunner != null)
                Destroy(currentRunner);

            currentRunner = new GameObject("EarthquakeRunner");
            var runner = currentRunner.AddComponent<EarthquakeRunner>();
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

            if (anyValid) continue;

            if (data.gimmickItemPrefab != null)
            {
                Vector3 randomPos = itemSpawnCenter + new Vector3(
                    Random.Range(-itemSpawnRadius, itemSpawnRadius),
                    0.5f,
                    Random.Range(-itemSpawnRadius, itemSpawnRadius)
                );

                var item = Instantiate(data.gimmickItemPrefab, randomPos, Quaternion.Euler(-90, 0, 0));
                item.layer = itemLayer;
                item.GetComponent<GimmickItem>()?.Init(StageGimmickType.Earthquake, this, data.pickupEffect, null, 0f);
            }
        }
    }

    private void OnDestroy()
    {
        if (cycleRoutine != null) StopCoroutine(cycleRoutine);
        if (itemRoutine != null) StopCoroutine(itemRoutine);
    }
}

public class EarthquakeRunner : MonoBehaviour
{
    private float shakeDuration;
    private float intensity;
    private float slowAmount = 0.5f;

    private Vector3 originalCamPos;
    private Coroutine shakeRoutine;

    public static EarthquakeRunner Instance { get; private set; }

    public void Init(EarthquakeGimmickSO data)
    {
        Instance = this;

        shakeDuration = data.shakeDuration;
        intensity = data.intensity;

        StartCoroutine(ApplySlow());
        shakeRoutine = StartCoroutine(ShakeCamera());
    }

    private IEnumerator ApplySlow()
    {
        Time.timeScale = slowAmount;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        Debug.Log("[Earthquake] 슬로우 시작");

        yield return new WaitForSecondsRealtime(shakeDuration);

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        Debug.Log("[Earthquake] 슬로우 해제");

        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
            if (Camera.main != null)
                Camera.main.transform.localPosition = originalCamPos;

            Debug.Log("[Earthquake] 카메라 흔들림 강제 종료");
        }

        Destroy(gameObject);
    }

    private IEnumerator ShakeCamera()
    {
        if (Camera.main == null) yield break;

        originalCamPos = Camera.main.transform.localPosition;
        float elapsed = 0f;
        bool logOnce = false;

        Debug.Log("[Earthquake] 카메라 흔들림 시작");

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            Camera.main.transform.localPosition = originalCamPos + Random.insideUnitSphere * intensity;

            if (!logOnce)
            {
                Debug.Log("[Earthquake] 흔들림 중...");
                logOnce = true;
            }

            yield return null;
        }

        Camera.main.transform.localPosition = originalCamPos;
        Debug.Log("[Earthquake] 카메라 흔들림 정상 종료");
    }

    public static void RemoveSlow()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        Debug.Log("[Earthquake] 아이템 사용으로 슬로우 제거");

        if (Instance != null)
        {
            if (Instance.shakeRoutine != null)
            {
                Instance.StopCoroutine(Instance.shakeRoutine);
                if (Camera.main != null)
                    Camera.main.transform.localPosition = Instance.originalCamPos;

                Debug.Log("[Earthquake] 아이템으로 인해 카메라 흔들림 종료");
            }

            Destroy(Instance.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        if (Camera.main != null)
            Camera.main.transform.localPosition = originalCamPos;
    }
}