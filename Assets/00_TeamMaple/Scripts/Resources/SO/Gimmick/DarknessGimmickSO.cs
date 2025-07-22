using UnityEngine;

[CreateAssetMenu(fileName = "DarknessGimmickSO", menuName = "StageGimmick/Darkness")]
public class DarknessGimmickSO : StageGimmickSO
{
    public float duration = 15f;

    public override GameObject Execute(Vector3 origin)
    {
        var obj = new GameObject("DarknessRunner");
        var runner = obj.AddComponent<DarknessRunner>();
        runner.Init(duration);
        return obj;
    }
    
    private void OnEnable()
    {
        GimmickRegistry.Register(StageGimmickType.Darkness, this);
    }
}

public class DarknessRunner : MonoBehaviour
{
    private float timer;
    private float duration;
    private Color originalAmbient;
    private Color originalFog;
    private bool fogInitiallyEnabled;

    public void Init(float d)
    {
        duration = d;

        // ✅ 기존 설정 백업
        originalAmbient = RenderSettings.ambientLight;
        originalFog = RenderSettings.fogColor;
        fogInitiallyEnabled = RenderSettings.fog;

        // ✅ 어둠 적용
        RenderSettings.ambientLight = Color.black;
        RenderSettings.fog = true;
        RenderSettings.fogColor = Color.black;

        // 옵션: Directional Light 끄기
        var light = GameObject.FindObjectOfType<Light>();
        if (light != null && light.type == LightType.Directional)
            light.enabled = false;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > duration)
        {
            // ✅ 원복
            RenderSettings.ambientLight = originalAmbient;
            RenderSettings.fogColor = originalFog;
            RenderSettings.fog = fogInitiallyEnabled;

            var light = GameObject.FindObjectOfType<Light>();
            if (light != null && light.type == LightType.Directional)
                light.enabled = true;

            Destroy(gameObject);
        }
    }
}


