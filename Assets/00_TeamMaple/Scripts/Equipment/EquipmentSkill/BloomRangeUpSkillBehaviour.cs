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

    public void Excute(GameObject caster, EquipmentSkillType type, float cooldown, float duration, float param)
    {
        var prevRange = LocalPlayer.Instance.StatCalculator.BloomRange;    // �нú� ����� ������ ���� �ʿ�
        var nextRange = prevRange + prevRange * param;
        
        switch (type)
        {
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] ��ȭ ���� ���� �нú� �ߵ�");
                auraController.InitAura(nextRange);
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] ��ȭ ���� ���� ��Ƽ�� �ߵ�");
                auraController.ChangeScale(prevRange, nextRange, duration);
                break;
        }
    }
}
