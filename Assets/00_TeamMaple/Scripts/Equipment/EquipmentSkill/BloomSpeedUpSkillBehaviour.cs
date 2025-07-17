using UnityEngine;

public class BloomSpeedUpSkillBehaviour : MonoBehaviour,IEquipmentSkillBehaviour
{
    private AuraController auraController;
    
    private void Awake()
    {
        auraController = FindObjectOfType<AuraController>();
    }

    public void Excute(GameObject caster, EquipmentSkillType type, int duration, float param)
    {
        var range = LocalPlayer.Instance.PlayerStatHolder.BloomSpeed;
        var addRange = (int)(range.Value * param);
        
        throw new System.NotImplementedException();
    }
}
