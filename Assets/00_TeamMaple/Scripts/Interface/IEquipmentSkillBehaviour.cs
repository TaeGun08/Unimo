using UnityEngine;

public interface IEquipmentSkillBehaviour
{
    void Excute(GameObject caster, EquipmentSkillType type, float cooldown, float duration, float param);
}
