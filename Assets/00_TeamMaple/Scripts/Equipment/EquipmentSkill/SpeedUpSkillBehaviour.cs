using UnityEngine;

// 이동 속도 증가 스킬
public class SpeedUpSkillBehaviour : MonoBehaviour,IEquipmentSkillBehaviour
{
    public void Excute(GameObject caster, EquipmentSkillData skillData)
    {
        var speed = LocalPlayer.Instance.PlayerStatHolder.Speed;    // 기존 값
        var addSpeed = speed.Value * skillData.Param;    // 기존 값 * 증가 퍼센트
        
        switch (skillData.Type)
        { 
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] 이동 속도 증가 패시브 발동");
                speed.Add(addSpeed);    // 이동 속도 증가
                break;
            case EquipmentSkillType.Active:
                break;
        }
    }
}
