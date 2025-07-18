using System.Collections;
using UnityEngine;

// �ǰ� ��ȿȭ ��ų
public class HitInvalidationSkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    public float passiveCooldown;
    private Coroutine passiveRegenRoutine;
    
    private void Start()
    {
        // ��ȿȭ �����ø��� ��Ÿ�� ����!
        LocalPlayer.Instance.PlayerStatHolder.OnOnceInvalidUsed += StartPassiveCooldown;
    }

    private void OnDestroy()
    {
        LocalPlayer.Instance.PlayerStatHolder.OnOnceInvalidUsed -= StartPassiveCooldown;
    }

    public void Excute(GameObject caster, EquipmentSkillType type, int duration, float param)
    {
        switch (type)
        { 
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] �ǰ� ��ȿȭ �нú� �ߵ�");
                // ���ʿ��� 1ȸ ��ȿȭ �ο�
                LocalPlayer.Instance.PlayerStatHolder.GiveOnceInvalid();
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] ���� ��Ƽ�� �ߵ�");
                break;
        }
    }
    
    private void StartPassiveCooldown()
    {
        if (passiveRegenRoutine != null)
        {
            StopCoroutine(passiveRegenRoutine);
        }
        passiveRegenRoutine = StartCoroutine(PassiveCooldownRoutine());
    }

    private IEnumerator PassiveCooldownRoutine()
    {
        yield return new WaitForSeconds(passiveCooldown);
        LocalPlayer.Instance.PlayerStatHolder.GiveOnceInvalid();
        passiveRegenRoutine = null;
    }
}
