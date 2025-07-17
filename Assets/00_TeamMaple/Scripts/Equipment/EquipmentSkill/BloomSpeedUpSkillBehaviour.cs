using UnityEngine;

// ��ȭ �ӵ� ���� ��ų
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
                Debug.Log("[Passive] ��ȭ �ӵ� ���� �нú� �ߵ�");
                bloomSpeed.Add(addBloomSpeed);
                auraController.InitAura();
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] ��ȭ �ӵ� ���� ��Ƽ�� �ߵ�");
                break;
        }
    }
}
