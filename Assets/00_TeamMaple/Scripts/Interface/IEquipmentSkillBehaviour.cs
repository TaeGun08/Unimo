using UnityEngine;

public interface IEquipmentSkillBehaviour
{
    void Excute(GameObject caster, EquipmentSkillType type, int duration, float param);
}
