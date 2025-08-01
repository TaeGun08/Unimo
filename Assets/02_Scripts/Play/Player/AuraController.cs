using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraController : MonoBehaviour
{
    [SerializeField] private Transform target; 
    
    private float growthperSec = 12f;   // 개화 속도
    private float originalGrowth = 12f;    // 개화 속도 초기값
    private Vector3 auraScale;    // 오라 범위
    private Vector3 originalScale;    // 오라 범위 초기값
    
    private float lerpDuration = 0.5f;    // 오라 범위 변경되는 시간
    
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

    // 오라 세팅 재설정
    public void InitAura()
    {
        playerStatHolder = LocalPlayer.Instance.PlayerStatHolder;
        
        transform.localScale = playerStatHolder.BloomRange.Value * Vector3.one;
        originalScale = transform.localScale;
        
        growthperSec = playerStatHolder.BloomSpeed.Value;
        originalGrowth = growthperSec;
    }

    // 오라 크기 증가 후 복구
    public void ChangeScale(int next, float duration)
    {
        // 만약 이미 변경 중이라면 기존 코루틴 중단
        if (changeScaleCoroutine != null)
            StopCoroutine(changeScaleCoroutine);

        // 코루틴 시작
        changeScaleCoroutine = StartCoroutine(ChangeScaleCoroutine(next, duration));
    }
    
    private IEnumerator ChangeScaleCoroutine(int next, float duration)
    {
        skillManager.effectController.PlaySkillEffect(1);
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = next * Vector3.one;

        // 1. prev → next로 부드럽게 증가
        while (elapsed < lerpDuration)
        {
            float t = elapsed / lerpDuration;
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetScale;

        // 2. 일정 시간 유지(duration > 0일 때만)
        if (duration > 0f)
            yield return new WaitForSeconds(duration);

        // 3. next → prev로 복귀(duration > 0일 때만)
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
