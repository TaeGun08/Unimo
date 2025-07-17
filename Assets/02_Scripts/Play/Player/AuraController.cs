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
        transform.localScale = playerStatHolder.BloomRange.Value * 0.1f * Vector3.one;    // 0.1�� ��ġ ������
        originalScale = transform.localScale;
        originalGrowth = playerStatHolder.BloomSpeed.Value;
    }

    public void ChangeScale(int prev, int next, int duration)
    {
        // ���� �̹� ���� ���̶�� ���� �ڷ�ƾ �ߴ�
        if (changeScaleCoroutine != null)
            StopCoroutine(changeScaleCoroutine);

        // �ڷ�ƾ ����
        changeScaleCoroutine = StartCoroutine(ChangeScaleCoroutine(prev, next, duration));
    }
    
    private IEnumerator ChangeScaleCoroutine(int prev, int next, int duration)
    {
        float elapsed = 0f;
        Vector3 startScale = prev * 0.1f * Vector3.one;
        Vector3 targetScale = next * 0.1f * Vector3.one;

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
