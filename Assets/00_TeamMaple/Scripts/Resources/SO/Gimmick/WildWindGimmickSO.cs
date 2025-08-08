// ✅ WildWindGimmickSO 및 Runner 수정: 아이템 생성 추가, 20초간 바람 면역 적용 기능 포함

using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "WildWindGimmickSO", menuName = "StageGimmick/WildWind")]
public class WildWindGimmickSO : StageGimmickSO
{
    public float delayBeforeStart = 5f;
    public float windDuration = 20f;
    public float windInterval = 30f;
    public Vector3 windDirection = Vector3.left;
    public AnimationCurve decayCurve = AnimationCurve.Linear(0, 1f, 1, 0f);
    public GameObject windVisualPrefab;
    public bool alternateDirection = true;
    public float windForce = 10f;
    public float groundClampRadius = 15f;

    public float itemSpawnInterval = 15f;
    public Vector3 itemSpawnCenter;
    public float itemSpawnRadius = 10f;

    public override GameObject Execute(Vector3 origin)
    {
        var runnerObj = new GameObject("WildWindRunner");
        var runner = runnerObj.AddComponent<WildWindRunner>();
        runner.Init(this, origin);
        return runnerObj;
    }

    private void OnEnable()
    {
        GimmickRegistry.Register(StageGimmickType.WildWind, this);
    }
}

public class WildWindRunner : MonoBehaviour
{
    private WildWindGimmickSO data;
    private GameObject windVisual;
    private Rigidbody playerRb;
    private Transform player;

    private float timer = 0f;
    private bool windActive = false;
    private Vector3 currentDirection;

    public static Vector3 CurrentWindDirection { get; private set; } = Vector3.zero;

    private float itemSpawnInterval;
    private Vector3 itemSpawnCenter;
    private float itemSpawnRadius;
    private GameObject gimmickItemPrefab;
    private Coroutine itemRoutine;

    private bool isImmune = false;
    private Coroutine immuneRoutine;

    public void Init(WildWindGimmickSO so, Vector3 origin)
    {
        data = so;

        itemSpawnInterval = data.itemSpawnInterval;
        itemSpawnCenter = data.itemSpawnCenter == Vector3.zero ? origin : data.itemSpawnCenter;
        itemSpawnRadius = data.itemSpawnRadius;
        gimmickItemPrefab = data.gimmickItemPrefab;

        StartCoroutine(WindRoutine());
        itemRoutine = StartCoroutine(SpawnItemRoutine());
    }

    private IEnumerator WindRoutine()
    {
        yield return new WaitForSeconds(data.delayBeforeStart);

        player = LocalPlayer.Instance.transform;
        playerRb = player.GetComponent<Rigidbody>();

        if (playerRb == null)
        {
            Debug.LogError("[WildWind] 플레이어에 Rigidbody 없음!");
            yield break;
        }

        currentDirection = data.windDirection.normalized;

        while (true)
        {
            CurrentWindDirection = currentDirection;

            if (data.windVisualPrefab != null)
            {
                Quaternion rotation = (currentDirection.x < 0)
                    ? Quaternion.Euler(90f, 180f, 0f)
                    : Quaternion.Euler(90f, 0f, 0f);

                windVisual = Instantiate(
                    data.windVisualPrefab,
                    player.position + Vector3.up * 2f,
                    rotation
                );
            }

            timer = 0f;
            windActive = true;

            while (timer < data.windDuration)
            {
                timer += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }

            windActive = false;
            CurrentWindDirection = Vector3.zero;

            if (windVisual != null)
                Destroy(windVisual);

            if (playerRb != null)
            {
                Vector3 velocity = playerRb.linearVelocity;
                velocity.x = 0f;
                velocity.z = 0f;
                playerRb.linearVelocity = velocity;
            }

            if (data.alternateDirection)
                currentDirection *= -1;

            float waitTime = Mathf.Max(0f, data.windInterval - data.windDuration);
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void FixedUpdate()
    {
        if (!windActive || playerRb == null || isImmune) return;

        float normalizedTime = timer / data.windDuration;
        float decay = data.decayCurve.Evaluate(normalizedTime);
        Vector3 targetVelocity = CurrentWindDirection * decay * data.windForce;

        Vector3 velocity = playerRb.linearVelocity;
        velocity.x = targetVelocity.x;
        velocity.z = targetVelocity.z;
        playerRb.linearVelocity = velocity;

        Vector3 center = Vector3.zero;
        Vector3 offset = player.position - center;
        if (offset.magnitude > data.groundClampRadius)
        {
            Vector3 clampedPos = center + offset.normalized * data.groundClampRadius;
            player.position = clampedPos;
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
                item.GetComponent<GimmickItem>()?.Init(StageGimmickType.WildWind);
            }
        }
    }

    public static void ApplyWindResist(float duration)
    {
        var runner = FindObjectOfType<WildWindRunner>();
        if (runner != null)
            runner.SetImmune(duration);
    }

    private void SetImmune(float duration)
    {
        if (immuneRoutine != null)
            StopCoroutine(immuneRoutine);

        immuneRoutine = StartCoroutine(WindImmuneRoutine(duration));
    }

    private IEnumerator WindImmuneRoutine(float duration)
    {
        isImmune = true;
        Debug.Log($"[WildWind] 바람 면역 {duration}초 시작");

        yield return new WaitForSeconds(duration);

        isImmune = false;
        immuneRoutine = null;
        Debug.Log("[WildWind] 바람 면역 종료");
    }

    private void OnDestroy()
    {
        if (itemRoutine != null) StopCoroutine(itemRoutine);
    }
}
