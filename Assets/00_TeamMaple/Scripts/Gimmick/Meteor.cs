// Meteor.cs
using UnityEngine;

public class Meteor : MonoBehaviour
{
    public float destroyDelay = 0.2f;
    public float fallSpeed = 20f;
    public Vector3 fallDirection = new Vector3(0.5f, -1, 0);

    private bool hasExploded = false;
    private Rigidbody rb;

    void Start()
    {
        transform.rotation = Quaternion.Euler(70f, 90f, 0f);

        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.linearVelocity = fallDirection.normalized * fallSpeed;
        }

        int meteorLayer = LayerMask.NameToLayer("Meteor");
        if (gameObject.layer != meteorLayer)
        {
            gameObject.layer = meteorLayer;
        }

        // Meteor 레이어 간 충돌 비활성화
        Physics.IgnoreLayerCollision(meteorLayer, meteorLayer, true);
    }

    void Update()
    {
        transform.Rotate(0f, 0f, 720f * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasExploded) return;
        if (!collision.collider.CompareTag("Ground")) return;

        hasExploded = true;

        MeteorFallRunner runner = FindObjectOfType<MeteorFallRunner>();
        if (runner != null)
        {
            runner.NotifyMeteorDestroyed(gameObject);
        }

        Destroy(gameObject, destroyDelay);
    }

    private void OnDestroy()
    {
        MeteorFallRunner runner = FindObjectOfType<MeteorFallRunner>();
        if (runner != null)
        {
            runner.NotifyMeteorDestroyed(gameObject);
        }
    }
}