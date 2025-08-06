using UnityEngine;

public class SlipperyReceiver : MonoBehaviour
{
    [Header("슬리퍼리 설정")]
    public float drag = 3f;

    private Rigidbody rb;
    private bool isSlippery = false;
    private float slipperyForce;
    private float maxSpeed;

    private VirtualJoystickCtrl_ST001 joystick;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // 버츄얼 조이스틱을 씬에서 찾아오기
        joystick = FindObjectOfType<VirtualJoystickCtrl_ST001>();
        if (joystick == null)
        {
            Debug.LogWarning("[SlipperyReceiver] VirtualJoystickCtrl_ST001을 찾을 수 없습니다.");
        }
    }

    private void FixedUpdate()
    {
        if (!isSlippery || rb == null || joystick == null)
            return;

        // 조이스틱 입력값 가져오기
        Vector2 input = joystick.Dir;

        if (input != Vector2.zero)
        {
            Vector3 moveDir = new Vector3(input.x, 0f, input.y).normalized;

            // 속도가 maxSpeed를 넘지 않도록 제한
            if (rb.linearVelocity.magnitude < maxSpeed)
            {
                rb.AddForce(moveDir * slipperyForce, ForceMode.Acceleration);
            }
        }

        // 감속 효과를 위한 Drag 적용
        rb.linearDamping = drag;
    }

    public void SetSlippery(bool enable, float force = 0f, float max = 0f)
    {
        isSlippery = enable;

        if (enable)
        {
            slipperyForce = force;
            maxSpeed = max;
            rb.linearDamping = drag;
            Debug.Log("[SlipperyReceiver] 슬리퍼리 적용됨");
        }
        else
        {
            rb.linearDamping = 0f;
            Debug.Log("[SlipperyReceiver] 슬리퍼리 종료");
        }
    }
}