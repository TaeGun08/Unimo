using UnityEngine;

[CreateAssetMenu(fileName = "EarthquakeGimmickSO", menuName = "StageGimmick/Earthquake")]
public class EarthquakeGimmickSO : StageGimmickSO
{
    public float shakeDuration = 5f;
    public float intensity = 0.3f;

    public override GameObject Execute(Vector3 origin)
    {
        var obj = new GameObject("EarthquakeRunner");
        var runner = obj.AddComponent<EarthquakeRunner>();
        runner.Init(this);
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

    public void Init(EarthquakeGimmickSO so)
    {
        duration = so.shakeDuration;
        intensity = so.intensity;
    }

    private void Update()
    {
        if (timer < duration)
        {
            Camera.main.transform.localPosition += Random.insideUnitSphere * intensity * Time.deltaTime;
            timer += Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

