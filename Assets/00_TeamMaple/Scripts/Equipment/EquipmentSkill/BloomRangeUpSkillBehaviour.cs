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

    public void Excute(GameObject caster, EquipmentSkillType type, float cooldown, float duration, float param)
    {
        var prevRange = LocalPlayer.Instance.StatCalculator.BloomRange;    // 패시브 적용된 값으로 수정 필요
        var nextRange = prevRange + prevRange * param;
        
        switch (type)
        {
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] 개화 범위 증가 패시브 발동");
                auraController.InitAura(nextRange);
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] 개화 범위 증가 액티브 발동");
                auraController.ChangeScale(prevRange, nextRange, duration);
                break;
        }
    }
}
