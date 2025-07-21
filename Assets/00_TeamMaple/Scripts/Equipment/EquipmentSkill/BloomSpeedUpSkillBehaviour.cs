using UnityEngine;

// ��ȭ �ӵ� ���� ��ų
public class BloomSpeedUpSkillBehaviour : MonoBehaviour,IEquipmentSkillBehaviour
{
    private AuraController auraController;
    
    private void Awake()
    {
        auraController = FindObjectOfType<AuraController>();
    }

    public void Excute(GameObject caster, EquipmentSkillData skillData)
    {
        var bloomSpeed = LocalPlayer.Instance.PlayerStatHolder.BloomSpeed;    // ���� ��
        var addBloomSpeed = bloomSpeed.Value * skillData.Param;    // ���� �� * ���� �ۼ�Ʈ
        
        switch (skillData.Type)
        { 
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] ��ȭ �ӵ� ���� �нú� �ߵ�");
                bloomSpeed.Add(addBloomSpeed);    // ��ȭ �ӵ� ����
                auraController.InitAura();    // ���� ���� �缳�� (PlayerStatHolder ���� ���)
                break;
            case EquipmentSkillType.Active:
                break;
        }
    }
}
