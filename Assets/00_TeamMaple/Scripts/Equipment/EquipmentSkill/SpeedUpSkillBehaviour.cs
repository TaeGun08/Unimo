using UnityEngine;

// 이동 속도 증가 스킬
public class SpeedUpSkillBehaviour : MonoBehaviour,IEquipmentSkillBehaviour
{
    public void Excute(GameObject caster, EquipmentSkillType type, int duration, float param)
    {
        var speed = LocalPlayer.Instance.PlayerStatHolder.Speed;
        var addSpeed = speed.Value * param;
        
        switch (type)
        { 
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] 이동 속도 증가 패시브 발동");
                speed.Add(addSpeed);
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] 이동 속도 증가 액티브 발동");
                break;
        }
    }
}
