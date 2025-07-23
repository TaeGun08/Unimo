using System.Collections;
using UnityEngine;

// 체력 회복 스킬
public class HpRecoverySkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    private Coroutine passiveCoroutine;
    
    public void Excute(GameObject caster, EquipmentSkillData skillData)
    {
        switch (skillData.Type)
        {
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] 체력 회복 패시브 발동");
                if (passiveCoroutine == null)
                {
                    passiveCoroutine = StartCoroutine(PassiveHpRecovery(skillData.Param));
                }
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] 체력 회복 액티브 발동");
                ActiveHpRegen(skillData.Param);
                break;
        }
    }
    
    private IEnumerator PassiveHpRecovery(float percentPerSec)
    {
        var maxHp = LocalPlayer.Instance.StatCalculator.Hp;    // 정해진 Hp 최대값
        var hp = LocalPlayer.Instance.PlayerStatHolder.Hp;    // 변경 가능한 Hp 값

        while (true)
        {
            int baseHp = maxHp;
            int healAmount = Mathf.Max(1, Mathf.RoundToInt(baseHp * percentPerSec));    // 10초당 회복할 체력
            hp.Add(healAmount);    // 체력 증가
            
            yield return new WaitForSeconds(10f);
        }
    }

    private void ActiveHpRegen(float percent)
    {
        var hp = LocalPlayer.Instance.PlayerStatHolder.Hp;    // 기존 값
        var addHp = (int)(hp.Value * percent);    // 기존 값 * 증가 퍼센트
        hp.Add(addHp);    // 체력 증가
    }
}
