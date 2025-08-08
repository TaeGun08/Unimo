// ✅ SlipperyReceiver.cs 전체 수정: 슬리퍼리 면역 기능 포함

using UnityEngine;
using System.Collections;

public class SlipperyReceiver : MonoBehaviour
{
    [Header("슬리퍼리 설정")]
    public float drag = 0.5f;
    public float directionSmoothness = 3f;

    [Header("맵 제한 설정")]
    public Vector3 mapCenter = Vector3.zero;
    public float mapRadius = 17f;

    private Rigidbody rb;
    private bool isSlippery = false;
    private float slipperyForce;
    private float maxSpeed;

    private Vector3 lastMoveDir = Vector3.zero;
    private VirtualJoystickCtrl_ST001 joystick;

    private bool isImmune = false;
    private Coroutine immuneRoutine;
    public bool IsSlipImmune => isImmune;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        joystick = FindObjectOfType<VirtualJoystickCtrl_ST001>();

        if (joystick == null)
            Debug.LogWarning("[SlipperyReceiver] VirtualJoystickCtrl_ST001을 찾을 수 없습니다.");
    }

    private void FixedUpdate()
    {
        if (!isSlippery || rb == null || joystick == null)
            return;

        Vector2 input = joystick.Dir;

        if (input != Vector2.zero)
        {
            Vector3 inputDir = new Vector3(input.x, 0f, input.y).normalized;
            lastMoveDir = Vector3.Lerp(lastMoveDir, inputDir, Time.fixedDeltaTime * directionSmoothness);

            if (rb.linearVelocity.magnitude < maxSpeed)
                rb.AddForce(lastMoveDir * slipperyForce, ForceMode.Acceleration);
        }
        else
        {
            lastMoveDir = Vector3.Lerp(lastMoveDir, Vector3.zero, Time.fixedDeltaTime * directionSmoothness);
        }

        rb.linearDamping = drag;
    }

    private void LateUpdate()
    {
        if (isSlippery)
            ClampPositionToCircle();
    }

    private void ClampPositionToCircle()
    {
        Vector3 pos = transform.position;
        Vector3 flatPos = new Vector3(pos.x, 0f, pos.z);
        Vector3 flatCenter = new Vector3(mapCenter.x, 0f, mapCenter.z);

        Vector3 fromCenter = flatPos - flatCenter;
        float dist = fromCenter.magnitude;

        if (dist > mapRadius)
        {
            Vector3 clampedPos = flatCenter + fromCenter.normalized * mapRadius;
            transform.position = new Vector3(clampedPos.x, pos.y, clampedPos.z);
        }
    }

    public void SetSlippery(bool enable, float force = 0f, float max = 0f)
    {
        if (enable && isImmune)
        {
            Debug.Log("[SlipperyReceiver] 면역 상태로 인해 슬리퍼리 무시됨");
            return;
        }

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
            lastMoveDir = Vector3.zero;
            Debug.Log("[SlipperyReceiver] 슬리퍼리 종료");
        }
    }

    public void ApplySlipImmune(float duration)
    {
        if (immuneRoutine != null)
            StopCoroutine(immuneRoutine);

        immuneRoutine = StartCoroutine(SlipImmuneCoroutine(duration));
    }

    private IEnumerator SlipImmuneCoroutine(float duration)
    {
        isImmune = true;
        Debug.Log($"[SlipperyReceiver] 슬리퍼리 면역 적용: {duration}초");

        yield return new WaitForSeconds(duration);

        isImmune = false;
        immuneRoutine = null;
        Debug.Log("[SlipperyReceiver] 슬리퍼리 면역 해제됨");
    }
}