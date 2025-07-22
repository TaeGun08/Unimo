using System.Collections;
using UnityEngine;

// ��ȭ ���� �� �� ��� ��ȭ ��ų   
public class ImmediateBloomSkillBehaviour : MonoBehaviour, IEquipmentSkillBehaviour
{
    private AuraController auraController;
    
    private Coroutine activeCoroutine;
    
    private void Awake()
    {
        auraController = FindObjectOfType<AuraController>();
    }

    public void Excute(GameObject caster, EquipmentSkillData skillData)
    {
        var bloomSpeed = LocalPlayer.Instance.PlayerStatHolder.BloomSpeed;    // ���� ��
        var duration = 0.1f;    // ��� ��ȭ
        var addBloomSpeed = 999f;    // ��ȭ �ӵ� �ִ��
        
        switch (skillData.Type)
        { 
            case EquipmentSkillType.Passive:
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] ��ȭ ���� �� �� ��� ��ȭ ��Ƽ�� �ߵ�");
                if (activeCoroutine != null)
                {
                    StopCoroutine(activeCoroutine);
                }
                activeCoroutine = StartCoroutine(ActiveImmediateBloom(bloomSpeed, duration, addBloomSpeed));
                break;
        }
    }
    
    private IEnumerator ActiveImmediateBloom(ClampedFloat bloomSpeed, float duration, float addBloomSpeed)
    {
        var subtractValue = addBloomSpeed - bloomSpeed.Value;
        
        bloomSpeed.Add(addBloomSpeed);    // ��ȭ �ӵ� �Ͻ� ����
        auraController.InitAura();    // ���� ���� �缳�� (PlayerStatHolder ���� ���)
        yield return new WaitForSeconds(duration);    // duration�� ���
        bloomSpeed.Subtract(subtractValue);    // ������ �ٽ� ����
        auraController.InitAura();    // ���� ���� �缳�� (PlayerStatHolder ���� ���)
        activeCoroutine = null;
    }
}
