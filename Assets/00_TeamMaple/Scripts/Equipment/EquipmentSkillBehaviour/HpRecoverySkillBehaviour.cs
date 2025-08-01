using System;
using System.Collections;
using UnityEngine;

// ü�� ȸ�� ��ų
public class HpRecoverySkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    private Coroutine passiveCoroutine;
    private EquipmentSkillManager skillManager;
    
    private int maxHp;
    private ClampedInt hp;

    private void Awake()
    {
        skillManager = EquipmentSkillManager.Instance;
    }

    public void Excute(GameObject caster, EquipmentSkillData skillData)
    {
        maxHp = LocalPlayer.Instance.StatCalculator.Hp;    // ������ Hp �ִ밪
        hp = LocalPlayer.Instance.PlayerStatHolder.Hp;    // ���� ������ Hp ��
        
        switch (skillData.Type)
        {
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] ü�� ȸ�� �нú� �ߵ�");
                if (passiveCoroutine != null)
                {
                    StopCoroutine(passiveCoroutine);
                }
                passiveCoroutine = StartCoroutine(PassiveHpRecovery(skillData.Param));
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] ü�� ȸ�� ��Ƽ�� �ߵ�");
                ActiveHpRegen(skillData.Param);
                break;
        }
    }
    
    private IEnumerator PassiveHpRecovery(float percentPerSec)
    {
        while (true)
        {
            int baseHp = maxHp;
            int healAmount = Mathf.Max(1, Mathf.RoundToInt(baseHp * percentPerSec));    // 10�ʴ� ȸ���� ü��
            hp.Add(healAmount);    // ü�� ����
            skillManager.StartSkillCooldown(0, 10);
            skillManager.effectController.PlaySkillEffect(0);
            
            yield return new WaitForSeconds(10f);
        }
    }

    private void ActiveHpRegen(float percent)
    {
        var addHp = (int)(maxHp * percent);    // ���� �� * ���� �ۼ�Ʈ
        hp.Add(addHp);    // ü�� ����
        skillManager.effectController.PlaySkillEffect(1);
    }
}
