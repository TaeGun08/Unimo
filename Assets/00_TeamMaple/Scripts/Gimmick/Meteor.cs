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
        Transform explosionEffect = transform.Find("NukeExplosionFire 1");
        if (explosionEffect != null)
            explosionEffect.gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasExploded) return;

        bool hitGround = collision.collider.CompareTag("Ground");
        bool hitUnimo = collision.collider.CompareTag("Player");

        if (!hitGround && !hitUnimo) return;

        hasExploded = true;

        // 💥 이펙트 실행
        Transform explosion = transform.Find("NukeExplosionFire 1");
        if (explosion != null)
        {
            explosion.gameObject.SetActive(true);

            ParticleSystem ps = explosion.GetComponent<ParticleSystem>();
            if (ps != null)
                ps.Play();
        }

        MeteorFallRunner runner = FindObjectOfType<MeteorFallRunner>();
        if (runner != null)
        {
            runner.NotifyMeteorDestroyed(gameObject);
        }
        
        if (hitUnimo)
        {
            Rigidbody unimoRb = collision.rigidbody;
            if (unimoRb != null)
            {
                unimoRb.linearVelocity = Vector3.zero;
                unimoRb.angularVelocity = Vector3.zero;
            }
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