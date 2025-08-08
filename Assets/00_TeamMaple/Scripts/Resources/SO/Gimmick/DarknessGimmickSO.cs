using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "DarknessGimmickSO", menuName = "StageGimmick/Darkness")]
public class DarknessGimmickSO : StageGimmickSO
{
    public float duration = 20f;               // 어둠 지속 시간
    public float interval = 30f;               // 주기
    public GameObject darknessPrefab;

    public override GameObject Execute(Vector3 origin)
    {
        var obj = new GameObject("DarknessCycleRunner");
        var runner = obj.AddComponent<DarknessCycleRunner>();
        runner.duration = duration;
        runner.interval = interval;
        runner.darknessPrefab = darknessPrefab;
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
    public GameObject darknessPrefab;

    private float timer = 0f;
    private GameObject currentRunner;

    public void Init()
    {
        StartCoroutine(DarknessCycleRoutine());
    }

    private IEnumerator DarknessCycleRoutine()
    {
        // ✅ 첫 실행 전 5초 대기
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

            // ✅ 이후 interval 주기마다 반복
            yield return new WaitForSeconds(interval);
        }
    }

}

public class DarknessRunner : MonoBehaviour
{
    public float duration = 15f;
    public GameObject darknessPrefab;

    private GameObject darknessInstance;
    private Material darknessMaterial;
    private Transform player;
    private float timer = 0f;

    public void Init()
    {
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

    private void Update()
    {
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
}
