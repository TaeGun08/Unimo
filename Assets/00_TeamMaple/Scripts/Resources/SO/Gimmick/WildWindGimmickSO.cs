using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "WildWindGimmickSO", menuName = "StageGimmick/WildWind")]
public class WildWindGimmickSO : StageGimmickSO
{
    public float delayBeforeStart = 5f; // 기믹 시작 시간
    public float windDuration = 20f;     // 바람 지속 시간
    public float windInterval = 30f;     // 바람 간격 주기
    public Vector3 windDirection = Vector3.left; // 초기 바람 방향
    public AnimationCurve decayCurve = AnimationCurve.Linear(0, 1f, 1, 0f); // ✅ 시간에 따라 점점 약해짐
    public GameObject windVisualPrefab; // 바람 시각화 프리팹
    public bool alternateDirection = true; // 좌우로 번갈아 바람
    public float windForce = 10f; // 바람 세기 조절
    public float groundClampRadius = 15f; // 맵 밖으로 벗어나지 않도록 반경 제한

    public override GameObject Execute(Vector3 origin)
    {
        var runnerObj = new GameObject("WildWindRunner");
        var runner = runnerObj.AddComponent<WildWindRunner>();
        runner.Init(this);
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

    public void Init(WildWindGimmickSO so)
    {
        data = so;
        StartCoroutine(WindRoutine());
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
        if (!windActive || playerRb == null) return;

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
}
