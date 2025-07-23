using System.Collections;
using UnityEngine;

// ü�� ȸ�� ��ų
public class HpRecoverySkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    private Coroutine passiveCoroutine;
    
    public void Excute(GameObject caster, EquipmentSkillData skillData)
    {
        switch (skillData.Type)
        {
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] ü�� ȸ�� �нú� �ߵ�");
                if (passiveCoroutine == null)
                {
                    passiveCoroutine = StartCoroutine(PassiveHpRecovery(skillData.Param));
                }
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] ü�� ȸ�� ��Ƽ�� �ߵ�");
                ActiveHpRegen(skillData.Param);
                break;
        }
    }
    
    private IEnumerator PassiveHpRecovery(float percentPerSec)
    {
        var maxHp = LocalPlayer.Instance.StatCalculator.Hp;    // ������ Hp �ִ밪
        var hp = LocalPlayer.Instance.PlayerStatHolder.Hp;    // ���� ������ Hp ��

        while (true)
        {
            int baseHp = maxHp;
            int healAmount = Mathf.Max(1, Mathf.RoundToInt(baseHp * percentPerSec));    // 10�ʴ� ȸ���� ü��
            hp.Add(healAmount);    // ü�� ����
            
            yield return new WaitForSeconds(10f);
        }
    }

    private void ActiveHpRegen(float percent)
    {
        var hp = LocalPlayer.Instance.PlayerStatHolder.Hp;    // ���� ��
        var addHp = (int)(hp.Value * percent);    // ���� �� * ���� �ۼ�Ʈ
        hp.Add(addHp);    // ü�� ����
    }
}
