using UnityEngine;

[CreateAssetMenu(fileName = "DarknessGimmickSO", menuName = "StageGimmick/Darkness")]
public class DarknessGimmickSO : StageGimmickSO
{
    public float duration = 15f;
    public GameObject darknessPrefab;

    public override GameObject Execute(Vector3 origin)
    {
        var obj = new GameObject("DarknessRunner");
        var runner = obj.AddComponent<DarknessRunner>();
        runner.duration = duration;
        runner.darknessPrefab = darknessPrefab;
        runner.Init();
        return obj;
    }
    
    private void OnEnable()
    {
        GimmickRegistry.Register(StageGimmickType.Darkness, this);
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

        darknessInstance = Instantiate(darknessPrefab);
        darknessInstance.transform.SetParent(player);
        darknessInstance.transform.localPosition = Vector3.zero;
        darknessInstance.transform.localScale = Vector3.one * 50f;

        // ✅ 머티리얼 인스턴싱
        var renderer = darknessInstance.GetComponent<Renderer>();
        if (renderer != null)
        {
            darknessMaterial = renderer.material; // 인스턴스 복제됨
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

        // ✅ 플레이어 위치를 쉐이더에 전달
        if (darknessMaterial != null && player != null)
        {
            darknessMaterial.SetVector("_PlayerWorldPosition", player.position);
        }
    }
}
