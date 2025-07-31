using System;
using UnityEngine;

// ��ȭ ���� ���� ��ų
public class BloomRangeUpSkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    private AuraController auraController;
    
    private void Awake()
    {
        auraController = FindObjectOfType<AuraController>();
    }
    
    public void Excute(GameObject caster, EquipmentSkillData skillData)
    {
        var range = LocalPlayer.Instance.PlayerStatHolder.BloomRange;    // ���� ��
        var addRange = (int)(range.Value * skillData.Param);    // ���� �� * ���� �ۼ�Ʈ

        switch (skillData.Type)
        {
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] ��ȭ ���� ���� �нú� �ߵ�");
                range.Add(addRange);    // ��ȭ ���� ����
                auraController.InitAura();    // ���� ���� �缳�� (PlayerStatHolder ���� ���)
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] ��ȭ ���� ���� ��Ƽ�� �ߵ�");
                range.Add(addRange);    // ��ȭ ���� �Ͻ� ����
                auraController.ChangeScale(range.Value, skillData.Duration);    // ���� ũ�� ���� �� ����
                range.Subtract(addRange);    // ������ �ٽ� ����
                break;
        }
    }
}
