using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "SlipperyFloorGimmickSO", menuName = "StageGimmick/SlipperyFloor")]
public class SlipperyFloorGimmickSO : StageGimmickSO
{
    public float effectDuration = 15f;

    public override GameObject Execute(Vector3 origin)
    {
        Debug.Log("[SlipperyFloor] 얼어붙은 바닥 기믹 실행됨");

        GameObject runnerObj = new GameObject("SlipperyFloorRunner");
        SlipperyFloorRunner runner = runnerObj.AddComponent<SlipperyFloorRunner>();
        runner.Init(effectDuration);
        return runnerObj;
    }
    
    private void OnEnable()
    {
        GimmickRegistry.Register(StageGimmickType.SlipperyFloor, this);
    }
}

public class SlipperyFloorRunner : MonoBehaviour
{
    private float duration;
    private float elapsed;
    private Renderer[] groundRenderers;
    private Dictionary<Renderer, Color> originalColors = new();

    public void Init(float effectDuration)
    {
        duration = effectDuration;

        GameObject[] grounds = GameObject.FindGameObjectsWithTag("Ground");
        groundRenderers = grounds.Select(go => go.GetComponent<Renderer>())
                                  .Where(r => r != null)
                                  .ToArray();

        foreach (var r in groundRenderers)
        {
            originalColors[r] = r.material.color;
            r.material.color = Color.cyan;
        }

        ApplySlipperyEffect(true);
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed >= duration)
        {
            ApplySlipperyEffect(false);

            foreach (var r in groundRenderers)
            {
                if (r != null && originalColors.ContainsKey(r))
                {
                    r.material.color = originalColors[r];
                }
            }

            Destroy(gameObject);
        }
    }

    private void ApplySlipperyEffect(bool enable)
    {
        foreach (var controller in FindObjectsOfType<MonoBehaviour>())
        {
            if (controller is ISlipperyEffectReceiver receiver)
            {
                receiver.SetSlippery(enable);
            }
        }
    }
}
