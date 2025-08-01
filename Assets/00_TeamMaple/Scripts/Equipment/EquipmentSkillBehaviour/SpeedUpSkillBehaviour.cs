using System.Collections;
using UnityEngine;

// �̵� �ӵ� ���� ��ų
public class SpeedUpSkillBehaviour : MonoBehaviour,IEquipmentSkillBehaviour
{
    private Coroutine activeCoroutine;
    private EquipmentSkillManager skillManager;
    
    private void Awake()
    {
        skillManager = EquipmentSkillManager.Instance;
    }
    
    public void Excute(GameObject caster, EquipmentSkillData skillData)
    {
        var speed = LocalPlayer.Instance.PlayerStatHolder.Speed;    // ���� ��
        var addSpeed = speed.Value * skillData.Param;    // ���� �� * ���� �ۼ�Ʈ
        
        switch (skillData.Type)
        { 
            case EquipmentSkillType.Passive:
                Debug.Log("[Passive] �̵� �ӵ� ���� �нú� �ߵ�");
                speed.Add(addSpeed);    // �̵� �ӵ� ����
                break;
            case EquipmentSkillType.Active:
                Debug.Log("[Active] �̵� �ӵ� ���� ��Ƽ�� �ߵ�");
                if (activeCoroutine != null)
                {
                    StopCoroutine(activeCoroutine);
                }
                activeCoroutine = StartCoroutine(ActiveSpeedUp(speed, skillData.Duration, addSpeed));
                break;
        }
    }
    
    private IEnumerator ActiveSpeedUp(ClampedFloat speed, float duration, float addSpeed)
    {
        speed.Add(addSpeed);    // �̵� �ӵ� �Ͻ� ����
        skillManager.effectController.PlaySkillEffect(1);
        yield return new WaitForSeconds(duration);    // duration�� ���
        speed.Subtract(addSpeed);    // ������ �ٽ� ����
        skillManager.effectController.StopSkillEffect(1);
        activeCoroutine = null;
    }
}
