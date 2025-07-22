using UnityEngine;

// 방어력 증가 스킬
public class DefenseUpSkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    public void Excute(GameObject caster, EquipmentSkillData skillData)
    {
        var def = LocalPlayer.Instance.PlayerStatHolder.Def;    // 기존 값
        var addDef = (int)(def.Value * skillData.Param);    // 기존 값 * 증가 퍼센트
        
        switch (skillData.Type)
        { 
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] 방어력 증가 패시브 발동");
                def.Add(addDef);    // 방어력 증가
                break;
            case EquipmentSkillType.Active:
                break;
        }
    }
}
