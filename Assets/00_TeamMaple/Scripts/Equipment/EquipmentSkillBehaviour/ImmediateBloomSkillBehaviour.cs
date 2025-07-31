using System.Collections;
using UnityEngine;

// 개화 범위 내 꽃 즉시 개화 스킬   
public class ImmediateBloomSkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    private AuraController auraController;
    
    private Coroutine activeCoroutine;
    
    private void Awake()
    {
        auraController = FindObjectOfType<AuraController>();
    }

    public void Excute(GameObject caster, EquipmentSkillData skillData)
    {
        var bloomSpeed = LocalPlayer.Instance.PlayerStatHolder.BloomSpeed;    // 기존 값
        var duration = 0.1f;    // 즉시 개화
        var addBloomSpeed = 999f;    // 개화 속도 최대로
        
        switch (skillData.Type)
        { 
            case EquipmentSkillType.Passive:
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] 개화 범위 내 꽃 즉시 개화 액티브 발동");
                if (activeCoroutine != null)
                {
                    StopCoroutine(activeCoroutine);
                }
                activeCoroutine = StartCoroutine(ActiveImmediateBloom(bloomSpeed, duration, addBloomSpeed));
                break;
        }
    }
    
    private IEnumerator ActiveImmediateBloom(ClampedFloat bloomSpeed, float duration, float addBloomSpeed)
    {
        var subtractValue = addBloomSpeed - bloomSpeed.Value;
        
        bloomSpeed.Add(addBloomSpeed);    // 개화 속도 일시 증가
        auraController.InitAura();    // 오라 세팅 재설정 (PlayerStatHolder 스탯 기반)
        yield return new WaitForSeconds(duration);    // duration초 대기
        bloomSpeed.Subtract(subtractValue);    // 증가분 다시 감소
        auraController.InitAura();    // 오라 세팅 재설정 (PlayerStatHolder 스탯 기반)
        activeCoroutine = null;
    }
}
