using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "SlipperyFloorGimmickSO", menuName = "StageGimmick/SlipperyFloor")]
public class SlipperyFloorGimmickSO : StageGimmickSO
{
    public float slipperyDuration = 3f;
    public float slipperyForce = 150f;
    public float maxSlipSpeed = 3f;

    public override GameObject Execute(Vector3 origin)
    {
        var runnerObj = new GameObject("SlipperyFloorRunner");
        var runner = runnerObj.AddComponent<SlipperyFloorRunner>();
        runner.Init(this);
        return runnerObj;
    }

    private void OnEnable()
    {
        GimmickRegistry.Register(StageGimmickType.SlipperyFloor, this);
    }
}

public class SlipperyFloorRunner : MonoBehaviour
{
    private SlipperyFloorGimmickSO data;

    public void Init(SlipperyFloorGimmickSO so)
    {
        data = so;
        StartCoroutine(SlipperySequence());
    }

    private IEnumerator SlipperySequence()
    {
        var player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            var slippery = player.GetComponent<SlipperyReceiver>();
            if (slippery != null)
            {
                // SetSlippery 확장 버전 적용
                slippery.SetSlippery(
                    enable: true,
                    force: data.slipperyForce,
                    max: data.maxSlipSpeed
                );

                yield return new WaitForSeconds(data.slipperyDuration);

                slippery.SetSlippery(false);
            }
        }

        Destroy(gameObject);
    }
}