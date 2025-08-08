// ✅ EarthquakeGimmickSO 및 Runner 수정: 슬로우 제거 기능도 Runner 내부에 직접 구현

using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "EarthquakeGimmickSO", menuName = "StageGimmick/Earthquake")]
public class EarthquakeGimmickSO : StageGimmickSO
{
    public float shakeDuration = 5f;
    public float intensity = 0.3f;
    
    public float itemSpawnInterval = 15f;
    public Vector3 itemSpawnCenter;
    public float itemSpawnRadius = 10f;

    public override GameObject Execute(Vector3 origin)
    {
        var obj = new GameObject("EarthquakeRunner");
        var runner = obj.AddComponent<EarthquakeRunner>();
        runner.Init(this, origin);
        return obj;
    }

    private void OnEnable()
    {
        GimmickRegistry.Register(StageGimmickType.Earthquake, this);
    }
}

public class EarthquakeRunner : MonoBehaviour
{
    private float duration;
    private float timer;
    private float intensity;

    private GameObject gimmickItemPrefab;
    private float itemSpawnInterval;
    private Vector3 itemSpawnCenter;
    private float itemSpawnRadius;
    private Coroutine itemRoutine;

    public static EarthquakeRunner Instance { get; private set; }

    private bool isSlowed = false;
    private float slowAmount = 0.5f;
    private float slowDuration = 5f;
    private Coroutine slowRoutine;

    public void Init(EarthquakeGimmickSO so, Vector3 origin)
    {
        Instance = this;

        duration = so.shakeDuration;
        intensity = so.intensity;

        gimmickItemPrefab = so.gimmickItemPrefab;
        itemSpawnInterval = so.itemSpawnInterval;
        itemSpawnCenter = so.itemSpawnCenter == Vector3.zero ? origin : so.itemSpawnCenter;
        itemSpawnRadius = so.itemSpawnRadius;

        itemRoutine = StartCoroutine(SpawnItemRoutine());

        // 슬로우 효과 발동
        if (slowRoutine != null) StopCoroutine(slowRoutine);
        slowRoutine = StartCoroutine(ApplySlow());
    }

    private void Update()
    {
        if (timer < duration)
        {
            timer += Time.deltaTime;
            Camera.main.transform.localPosition += Random.insideUnitSphere * intensity * Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator ApplySlow()
    {
        isSlowed = true;
        Time.timeScale = slowAmount;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        Debug.Log("[Earthquake] 슬로우 시작");

        yield return new WaitForSecondsRealtime(slowDuration);

        if (isSlowed)
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
            Debug.Log("[Earthquake] 슬로우 정상화");
        }
    }

    public static void RemoveSlow()
    {
        if (Instance != null && Instance.isSlowed)
        {
            Instance.isSlowed = false;
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
            Debug.Log("[Earthquake] 아이템 사용으로 슬로우 제거됨");
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
                item.GetComponent<GimmickItem>()?.Init(StageGimmickType.Earthquake);
            }
        }
    }

    private void OnDestroy()
    {
        if (itemRoutine != null) StopCoroutine(itemRoutine);
        if (Instance == this) Instance = null;

        if (isSlowed)
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
        }
    }
}