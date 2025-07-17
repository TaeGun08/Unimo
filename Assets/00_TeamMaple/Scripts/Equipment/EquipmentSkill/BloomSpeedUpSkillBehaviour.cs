using UnityEngine;

// 개화 속도 증가 스킬
public class BloomSpeedUpSkillBehaviour : MonoBehaviour,IEquipmentSkillBehaviour
{
    private AuraController auraController;
    
    private void Awake()
    {
        auraController = FindObjectOfType<AuraController>();
    }

    public void Excute(GameObject caster, EquipmentSkillType type, int duration, float param)
    {
        var bloomSpeed = LocalPlayer.Instance.PlayerStatHolder.BloomSpeed;
        var addBloomSpeed = bloomSpeed.Value * param;
        
        switch (type)
        { 
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] 개화 속도 증가 패시브 발동");
                bloomSpeed.Add(addBloomSpeed);
                auraController.InitAura();
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] 개화 속도 증가 액티브 발동");
                break;
        }
    }
}
