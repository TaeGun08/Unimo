using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SlipperyReceiver : MonoBehaviour
{
    private Rigidbody rb;
    private bool isSlippery = false;
    private float slipperyTimer = 0f;

    [Header("슬리퍼리 설정")]
    public float slipperyDuration = 2f;
    public float initialSpeed = 5f;
    public float drag = 0.5f; // 작을수록 오래 미끄러짐

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearDamping = 0f;
    }

    private void FixedUpdate()
    {
        if (isSlippery)
        {
            slipperyTimer -= Time.fixedDeltaTime;

            if (slipperyTimer <= 0f)
            {
                isSlippery = false;
                rb.linearDamping = 0f;
                rb.linearVelocity = Vector3.zero;
                Debug.Log("[SlipperyReceiver] 슬리퍼리 종료");
            }
        }
    }

    /// <summary>
    /// 외부에서 호출해 슬리퍼리 효과를 적용
    /// </summary>
    /// <param name="dir">현재 이동 방향</param>
    public void ApplySlippery(Vector3 dir)
    {
        rb.linearVelocity = dir.normalized * initialSpeed;
        rb.linearDamping = drag;

        slipperyTimer = slipperyDuration;
        isSlippery = true;

        Debug.Log("[SlipperyReceiver] 슬리퍼리 적용됨");
    }
}