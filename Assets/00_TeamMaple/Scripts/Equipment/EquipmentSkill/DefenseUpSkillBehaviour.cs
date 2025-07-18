using UnityEngine;

// 방어력 증가 스킬
public class DefenseUpSkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    public void Excute(GameObject caster, EquipmentSkillType type, int duration, float param)
    {
        var def = LocalPlayer.Instance.PlayerStatHolder.Def;
        var addDef = (int)(def.Value * param);
        
        switch (type)
        { 
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] 방어력 증가 패시브 발동");
                def.Add(addDef);
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] 방어력 증가 액티브 발동");
                break;
        }
    }
}
