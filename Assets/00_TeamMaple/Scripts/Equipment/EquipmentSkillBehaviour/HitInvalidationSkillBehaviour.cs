using System.Collections;
using UnityEngine;

// �ǰ� ��ȿȭ ��ų
public class HitInvalidationSkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    private float passiveCooldown;
    
    private Coroutine passiveCoroutine;
    private Coroutine activeCoroutine;
    private PlayerStatHolder statHolder;
    
    private void Awake()
    {
        statHolder = LocalPlayer.Instance.PlayerStatHolder;
    }

    private void OnDestroy()
    {
        if (passiveCoroutine != null)
        {
            StopCoroutine(passiveCoroutine);
        }
        LocalPlayer.Instance.PlayerStatHolder.OnOnceInvalidUsed -= StartPassiveCooldown;
    }

    public void Excute(GameObject caster, EquipmentSkillData skillData)
    {
        switch (skillData.Type)
        { 
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] �ǰ� ��ȿȭ �нú� �ߵ�");
                // OnOnceInvalidUsed �̺�Ʈ�� �нú� ��Ÿ�� �ڷ�ƾ ����
                LocalPlayer.Instance.PlayerStatHolder.OnOnceInvalidUsed += StartPassiveCooldown;
                passiveCooldown = skillData.Cooldown;
                statHolder.GiveOnceInvalid();    // 1ȸ �ǰ� ��ȿ �ο�
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] ���� ��Ƽ�� �ߵ�");
                if (activeCoroutine != null)
                {
                    StopCoroutine(activeCoroutine);
                }
                activeCoroutine = StartCoroutine(ActiveInvincible(skillData.Duration));
                break;
        }
    }
    
    // �нú� ��Ÿ�� ��ŸƮ (1ȸ �ǰ� ��ȿ �ο� ���¿��� �ǰ� �ÿ� ȣ��)
    private void StartPassiveCooldown()
    {
        if (passiveCoroutine != null)
        {
            StopCoroutine(passiveCoroutine);
        }
        passiveCoroutine = StartCoroutine(PassiveCooldown());
        EquipmentSkillManager.Instance.StartSkillCooldown(0, passiveCooldown);
    }

    private IEnumerator PassiveCooldown()
    {
        yield return new WaitForSeconds(passiveCooldown);    // �нú� ��Ÿ�� ���
        statHolder.GiveOnceInvalid();    // �ٽ� 1ȸ �ǰ� ��ȿ �ο�
        passiveCoroutine = null;
    }
    
    private IEnumerator ActiveInvincible(float duration)
    {
        statHolder.GiveInvincible();    // ���� �ο�
        yield return new WaitForSeconds(duration);    // ��Ƽ�� ���ӽð� ���
        statHolder.RemoveInvincible();    // ���� ����
    }
}
