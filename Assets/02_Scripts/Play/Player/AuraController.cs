using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraController : MonoBehaviour
{
    [SerializeField] private Transform target; 
    
    private float growthperSec = 12f;   // ��ȭ �ӵ�
    private float originalGrowth = 12f;    // ��ȭ �ӵ� �ʱⰪ
    private Vector3 auraScale;    // ���� ����
    private Vector3 originalScale;    // ���� ���� �ʱⰪ
    
    private float lerpDuration = 0.5f;    // ���� ���� ����Ǵ� �ð�
    
    private Coroutine changeScaleCoroutine;
    private PlayerStatHolder playerStatHolder;
    private EquipmentSkillManager skillManager;

    private void Start()
    {
        skillManager = EquipmentSkillManager.Instance;
        
        InitAura();
    }

    private void Update()
    {
        transform.localPosition = target.localPosition;
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Flower"))
        {
            if (other.TryGetComponent<FlowerController>(out var flower))
            {
                flower.AuraAffectFlower(growthperSec * Time.fixedDeltaTime);
            }
        }
    }

    // ���� ���� �缳��
    public void InitAura()
    {
        playerStatHolder = LocalPlayer.Instance.PlayerStatHolder;
        
        transform.localScale = playerStatHolder.BloomRange.Value * Vector3.one;
        originalScale = transform.localScale;
        
        growthperSec = playerStatHolder.BloomSpeed.Value;
        originalGrowth = growthperSec;
    }

    // ���� ũ�� ���� �� ����
    public void ChangeScale(int next, float duration)
    {
        // ���� �̹� ���� ���̶�� ���� �ڷ�ƾ �ߴ�
        if (changeScaleCoroutine != null)
            StopCoroutine(changeScaleCoroutine);

        // �ڷ�ƾ ����
        changeScaleCoroutine = StartCoroutine(ChangeScaleCoroutine(next, duration));
    }
    
    private IEnumerator ChangeScaleCoroutine(int next, float duration)
    {
        skillManager.effectController.PlaySkillEffect(1);
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = next * Vector3.one;

        // 1. prev �� next�� �ε巴�� ����
        while (elapsed < lerpDuration)
        {
            float t = elapsed / lerpDuration;
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetScale;

        // 2. ���� �ð� ����(duration > 0�� ����)
        if (duration > 0f)
            yield return new WaitForSeconds(duration);

        // 3. next �� prev�� ����(duration > 0�� ����)
        if (duration > 0f)
        {
            elapsed = 0f;
            while (elapsed < lerpDuration)
            {
                float t = elapsed / lerpDuration;
                transform.localScale = Vector3.Lerp(targetScale, startScale, t);
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.localScale = startScale;
            skillManager.effectController.StopSkillEffect(1);
        }
    }

    // public void InitAura(float range)
    // {
    //     transform.localScale = range * Vector3.one;
    //     originalScale = transform.localScale;
    //     growthperSec = originalGrowth;
    // }
    // public void Shrink()
    // {
    //     StartCoroutine(CoroutineExtensions.ScaleInterpCoroutine(transform, 0.5f * originalScale, 0.1f));
    //     growthperSec = 0.3f * originalGrowth;
    // }
    // public void Resume()
    // {
    //     StartCoroutine(CoroutineExtensions.ScaleInterpCoroutine(transform, originalScale, 0.07f));
    //     growthperSec = originalGrowth;
    // }
}
