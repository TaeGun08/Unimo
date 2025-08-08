using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FogDamageGimmickSO", menuName = "StageGimmick/FogDamage")]
public class FogDamageGimmickSO : StageGimmickSO
{
    public GameObject fogVisualPrefab;
    public GameObject safeZoneTriggerPrefab;
    public Material safeZoneVisualMaterial;
    public float fogRadius = 25f;
    public float initialSafeRadius = 12f;
    public float shrinkDuration = 60f;
    public float damageInterval = 1f;
    public float damagePercent = 0.03f;

    public override GameObject Execute(Vector3 origin)
    {
        var go = new GameObject("FogDamageRunner");
        var runner = go.AddComponent<FogDamageRunner>();
        runner.Init(this, origin);
        return go;
    }

    private void OnEnable()
    {
        GimmickRegistry.Register(StageGimmickType.FogDamage, this);
    }
}

public class FogDamageRunner : MonoBehaviour
{
    private FogDamageGimmickSO config;
    private Transform player;
    private Transform safeZoneTrigger;
    private Renderer visualRenderer;

    private float shrinkTimer;
    private float dotTimer;

    public void Init(FogDamageGimmickSO so, Vector3 origin)
    {
        config = so;
        player = LocalPlayer.Instance.transform;

        transform.position = origin;
        transform.localScale = Vector3.one;

        // Fog Visual 생성
        if (config.fogVisualPrefab != null)
        {
            var fog = Instantiate(config.fogVisualPrefab, transform);
            fog.transform.localScale = new Vector3(config.fogRadius, 0.2f, config.fogRadius);
            fog.transform.localRotation = Quaternion.identity;
        }

        // SafeZone Trigger 생성
        if (config.safeZoneTriggerPrefab != null)
        {
            var trigger = Instantiate(config.safeZoneTriggerPrefab, transform.position, Quaternion.identity, transform);
            safeZoneTrigger = trigger.transform;
            safeZoneTrigger.transform.localScale = new Vector3(config.initialSafeRadius, 0.2f, config.initialSafeRadius);
        }

        // SafeZone Visual Sphere 생성 및 머티리얼 적용
        if (config.safeZoneVisualMaterial != null)
        {
            var visualObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            visualObj.transform.SetParent(transform);
            visualObj.transform.localPosition = Vector3.zero;
            visualObj.transform.localScale = new Vector3(config.initialSafeRadius, 0.2f, config.initialSafeRadius);

            var collider = visualObj.GetComponent<Collider>();
            if (collider != null) Destroy(collider);

            visualRenderer = visualObj.GetComponent<Renderer>();
            visualRenderer.material = new Material(config.safeZoneVisualMaterial);
        }
    }

    private void Update()
    {
        if (player == null || safeZoneTrigger == null) return;

        shrinkTimer += Time.deltaTime;
        float t = Mathf.Clamp01(shrinkTimer / config.shrinkDuration);
        float currentRadius = Mathf.Lerp(config.initialSafeRadius, 0f, t);

        safeZoneTrigger.localScale = Vector3.one * currentRadius * 2f;

        if (visualRenderer != null)
        {
            visualRenderer.transform.localScale = Vector3.one * currentRadius * 2f;
            visualRenderer.material.SetFloat("_SafeRadius", currentRadius);
            visualRenderer.material.SetVector("_Center", transform.position);
        }

        float distance = Vector3.Distance(player.position, transform.position);
        if (distance > currentRadius)
        {
            dotTimer += Time.deltaTime;
            if (dotTimer >= config.damageInterval)
            {
                ApplyFogDamage();
                dotTimer = 0f;
            }
        }
        else
        {
            dotTimer = 0f;
        }
    }

    private void ApplyFogDamage()
    {
        var statHolder = LocalPlayer.Instance.PlayerStatHolder;

        if (LocalPlayer.Instance.IsInvincible || statHolder.HasInvincible || statHolder.HasOnceInvalid)
        {
            Debug.Log("[FogZone] 무적 상태 → 피해 무효");

            if (statHolder.HasOnceInvalid)
            {
                statHolder.OnInvalidation();
                Debug.Log("[FogZone] 1회 무효화 소모");
            }

            return;
        }

        int damage = Mathf.CeilToInt(statHolder.Hp.MaxValue * config.damagePercent);
        statHolder.Hp.Subtract(damage);

        Debug.Log($"[FogZone] 틱 데미지 {damage} 적용됨 (HP: {statHolder.Hp.Value})");

        if (statHolder.Hp.Value <= 0)
        {
            LocalPlayer.Instance.playerController.ChangeState(IPlayerState.EState.Dead);
        }
    }
}
