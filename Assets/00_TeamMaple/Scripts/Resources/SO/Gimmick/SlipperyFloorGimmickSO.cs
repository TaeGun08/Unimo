using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "SlipperyFloorGimmickSO", menuName = "StageGimmick/SlipperyFloor")]
public class SlipperyFloorGimmickSO : StageGimmickSO
{
    public float slipperyDuration = 2f;

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
        // 플레이어 찾기
        var player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            var slippery = player.GetComponent<SlipperyReceiver>();
            if (slippery != null)
            {
                // 현재 이동 방향 계산
                Vector3 moveDir = Vector3.zero;

                Rigidbody rb = player.GetComponent<Rigidbody>();
                if (rb != null && rb.linearVelocity.magnitude > 0.1f)
                {
                    moveDir = rb.linearVelocity.normalized;
                }
                else
                {
                    moveDir = player.transform.forward;
                }

                slippery.slipperyDuration = data.slipperyDuration;
                slippery.ApplySlippery(moveDir);
            }
        }

        yield return new WaitForSeconds(data.slipperyDuration);
        Destroy(gameObject);
    }
}