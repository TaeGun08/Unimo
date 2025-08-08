using UnityEngine;

public class BlackHoleRecover : MonoBehaviour
{
    private Rigidbody rb;
    private bool isFalling = false;

    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundCheckDistance = 0.3f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void TriggerFall()
    {
        isFalling = true;
    }

    private void FixedUpdate()
    {
        if (!isFalling) return;

        if (IsGrounded())
        {
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.linearVelocity = Vector3.zero;

            isFalling = false;
            Debug.Log("[블랙홀] 착지 감지됨 - 중력 OFF, 낙하 종료");
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundMask);
    }
}