using System.Collections;
using UnityEngine;

// 체력 회복 스킬
public class HpRecoverySkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    private Coroutine passiveRegenRoutine;
    
    public void Excute(GameObject caster, EquipmentSkillType type, int duration, float param)
    {
        switch (type)
        {
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] 체력 회복 패시브 발동");
                if (passiveRegenRoutine == null)
                {
                    passiveRegenRoutine = StartCoroutine(PassiveHpRegen(param));
                }
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] 체력 회복 액티브 발동");
                ActiveHpRegen(param);
                break;
        }
    }
    
    private IEnumerator PassiveHpRegen(float percentPerSec)
    {
        var maxHp = LocalPlayer.Instance.StatCalculator.Hp;
        var hp = LocalPlayer.Instance.PlayerStatHolder.Hp;

        while (true)
        {
            int baseHp = maxHp;
            int healAmount = Mathf.Max(1, Mathf.RoundToInt(baseHp * percentPerSec));
            hp.Add(healAmount);
            
            yield return new WaitForSeconds(1f);
        }
    }

    private void ActiveHpRegen(float percent)
    {
        var hp = LocalPlayer.Instance.PlayerStatHolder.Hp;
        var addHp = (int)(hp.Value * percent);
        hp.Add(addHp);
    }
}
