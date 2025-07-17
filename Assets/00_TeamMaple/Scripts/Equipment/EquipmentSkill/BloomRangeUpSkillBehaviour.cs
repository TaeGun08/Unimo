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
    
    public void Excute(GameObject caster, EquipmentSkillType type, int duration, float param)
    {
        var range = LocalPlayer.Instance.PlayerStatHolder.BloomRange;
        var addRange = (int)(range.Value * param);

        switch (type)
        {
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] 개화 범위 증가 패시브 발동");
                range.Add(addRange);
                auraController.InitAura();
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] 개화 범위 증가 액티브 발동");
                var prev = range.Value;
                range.Add(addRange);
                auraController.ChangeScale(prev, range.Value, duration);
                range.Subtract(addRange);
                break;
        }
    }
}
