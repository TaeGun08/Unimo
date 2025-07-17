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
    
    private PlayerStatHolder playerStatHolder;
    private Coroutine changeScaleCoroutine;

    private void Start()
    {
        playerStatHolder = LocalPlayer.Instance.PlayerStatHolder;
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

    public void InitAura()
    {
        transform.localScale = playerStatHolder.BloomRange.Value * 0.1f * Vector3.one;    // 0.1은 수치 조정용
        originalScale = transform.localScale;
        originalGrowth = playerStatHolder.BloomSpeed.Value;
    }

    public void ChangeScale(int prev, int next, int duration)
    {
        // 만약 이미 변경 중이라면 기존 코루틴 중단
        if (changeScaleCoroutine != null)
            StopCoroutine(changeScaleCoroutine);

        // 코루틴 시작
        changeScaleCoroutine = StartCoroutine(ChangeScaleCoroutine(prev, next, duration));
    }
    
    private IEnumerator ChangeScaleCoroutine(int prev, int next, int duration)
    {
        float elapsed = 0f;
        Vector3 startScale = prev * 0.1f * Vector3.one;
        Vector3 targetScale = next * 0.1f * Vector3.one;

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
