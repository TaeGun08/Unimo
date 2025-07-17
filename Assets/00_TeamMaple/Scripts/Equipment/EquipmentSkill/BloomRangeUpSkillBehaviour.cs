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
    
    public void Excute(GameObject caster, EquipmentSkillType type, int duration, float param)
    {
        var range = LocalPlayer.Instance.PlayerStatHolder.BloomRange;
        var addRange = (int)(range.Value * param);

        switch (type)
        {
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] ��ȭ ���� ���� �нú� �ߵ�");
                range.Add(addRange);
                auraController.InitAura();
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] ��ȭ ���� ���� ��Ƽ�� �ߵ�");
                var prev = range.Value;
                range.Add(addRange);
                auraController.ChangeScale(prev, range.Value, duration);
                range.Subtract(addRange);
                break;
        }
    }
}
