using UnityEngine;
public class BlackHoleRecover : MonoBehaviour
{
    private bool isFallingFromBlackhole = false;
    private Rigidbody rb;

    [SerializeField] private LayerMask groundMask;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void TriggerFall()
    {
        isFallingFromBlackhole = true;
        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }
    }

    private void FixedUpdate()
    {
        if (!isFallingFromBlackhole) return;
    
        // 1. Ray 시작 위치를 위로 충분히 올림
        Vector3 rayStart = transform.position + Vector3.up * 1.0f;
    
        // 2. 탐지 거리 여유 있게 확보
        float castDistance = 2.5f;
    
        if (Physics.SphereCast(rayStart, 1.0f, Vector3.down, out RaycastHit hit, castDistance, groundMask))
        {
            Debug.Log("[블랙홀] 착지 감지됨 (SphereCast)");
    
            isFallingFromBlackhole = false;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
    
            Vector3 corrected = transform.position;
            corrected.y = 0f;
            transform.position = corrected;
    
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
            BlackHoleRunner.ResetHit(gameObject);
    
            Debug.Log("[블랙홀] 착지 완료 및 재진입 가능 상태");
        }
    }
}