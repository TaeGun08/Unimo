using System;
using UnityEngine;

// 개화 범위 증가 스킬
public class BloomRangeUpSkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    private AuraController auraController;
    
    private void Awake()
    {
        auraController = FindObjectOfType<AuraController>();
    }
    
    public void Excute(GameObject caster, EquipmentSkillData skillData)
    {
        var range = LocalPlayer.Instance.PlayerStatHolder.BloomRange;    // 기존 값
        var addRange = (int)(range.Value * skillData.Param);    // 기존 값 * 증가 퍼센트

        switch (skillData.Type)
        {
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] 개화 범위 증가 패시브 발동");
                range.Add(addRange);    // 개화 범위 증가
                auraController.InitAura();    // 오라 세팅 재설정 (PlayerStatHolder 스탯 기반)
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] 개화 범위 증가 액티브 발동");
                range.Add(addRange);    // 개화 범위 일시 증가
                auraController.ChangeScale(range.Value, skillData.Duration);    // 오라 크기 증가 후 복구
                range.Subtract(addRange);    // 증가분 다시 감소
                break;
        }
    }
}
