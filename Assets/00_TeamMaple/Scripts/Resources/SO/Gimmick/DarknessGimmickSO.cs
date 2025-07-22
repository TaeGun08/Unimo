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

    public void Init(float d)
    {
        duration = d;
        RenderSettings.ambientLight = Color.black;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > duration)
        {
            RenderSettings.ambientLight = Color.white;
            Destroy(gameObject);
        }
    }
}

