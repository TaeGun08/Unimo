using UnityEngine;

// 스턴 회복률 증가 스킬
public class StunRecoverySkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    public void Excute(GameObject caster, EquipmentSkillData skillData)
    {
        var stunRecovery = LocalPlayer.Instance.PlayerStatHolder.StunRecovery;    // 기존 값
        var addStunRecovery = stunRecovery.Value * skillData.Param;    // 기존 값 * 증가 퍼센트
        
        switch (skillData.Type)
        { 
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] 스턴 회복률 증가 패시브 발동");
                stunRecovery.Add(addStunRecovery);    // 스턴 회복률 증가
                break;
            case EquipmentSkillType.Active:
                break;
        }
    }
}
