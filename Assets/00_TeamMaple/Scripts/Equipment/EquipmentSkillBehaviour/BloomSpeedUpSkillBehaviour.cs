using UnityEngine;

// 개화 속도 증가 스킬
public class BloomSpeedUpSkillBehaviour : MonoBehaviour,IEquipmentSkillBehaviour
{
    private AuraController auraController;
    
    private void Awake()
    {
        auraController = FindObjectOfType<AuraController>();
    }

    public void Excute(GameObject caster, EquipmentSkillData skillData)
    {
        var bloomSpeed = LocalPlayer.Instance.PlayerStatHolder.BloomSpeed;    // 기존 값
        var addBloomSpeed = bloomSpeed.Value * skillData.Param;    // 기존 값 * 증가 퍼센트
        
        switch (skillData.Type)
        { 
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] 개화 속도 증가 패시브 발동");
                bloomSpeed.Add(addBloomSpeed);    // 개화 속도 증가
                auraController.InitAura();    // 오라 세팅 재설정 (PlayerStatHolder 스탯 기반)
                break;
            case EquipmentSkillType.Active:
                break;
        }
    }
}
