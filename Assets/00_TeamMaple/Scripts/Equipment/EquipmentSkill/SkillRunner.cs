using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SkillRunner : MonoBehaviour
{
    [SerializeField] private PrefabsTable skillTable;
    [SerializeField] private Button skillButton;
    [SerializeField] private GameObject player;
    
    [SerializeField] private EquipmentStatDataSO equipmentStatDataSo;
    [SerializeField] private EquipmentSkillDataSO skillDataSo;

    private bool isSkill2OnCooldown = false;    // 쿨타임 체크 변수
    private Coroutine skill2CooldownCoroutine;
    
    private void Start()
    {
        SetEngineSkills();
    }

    private void SetEngineSkills()
    {
        var engineStatData = equipmentStatDataSo.GetEquipmentStatData(Base_Mng.Data.data.EQCount);

        // 스킬 아이디 확인 (0이면 스킬 없음)
        var skillId1 = engineStatData.Skill1;
        var skillId2 = engineStatData.Skill2;

        // 스킬1
        if (skillId1 != 0)
        {
            var skillPrefab1 = Instantiate(skillTable.GetPrefabByKey(skillId1), player.transform);
            var skillExcutor1 = skillPrefab1.GetComponent<IEquipmentSkillBehaviour>();
            var skillData1 = skillDataSo.GetEquipmentSkillData(skillId1);

            skillExcutor1.Excute(player, skillData1);
            Debug.Log($"[Skill1] Id: {skillId1} / Type: {skillData1.Type} / Cooldown: {skillData1.Cooldown} / Duration: {skillData1.Duration} / Param: {skillData1.Param}");
        }
        else
        {
            Debug.Log("[Skill1] 스킬 없음 (ID=0)");
        }

        // 스킬2
        if (skillId2 != 0)
        {
            var skillPrefab2 = Instantiate(skillTable.GetPrefabByKey(skillId2), player.transform);
            var skillExcutor2 = skillPrefab2.GetComponent<IEquipmentSkillBehaviour>();
            var skillData2 = skillDataSo.GetEquipmentSkillData(skillId2);

            skillButton.onClick.AddListener(() =>
            {
                if (isSkill2OnCooldown)
                {
                    Debug.Log("[Skill2] 쿨타임 중!");    // 이미 쿨타임이면 무반응 또는 안내
                    return;
                }

                skillExcutor2.Excute(player, skillData2);
                Debug.Log($"[Skill2] Id: {skillId2} / Type: {skillData2.Type} / Cooldown: {skillData2.Cooldown} / Duration: {skillData2.Duration} / Param: {skillData2.Param}");

                // 쿨타임 시작!
                if (skill2CooldownCoroutine != null)
                {
                    StopCoroutine(skill2CooldownCoroutine);
                }
                skill2CooldownCoroutine = StartCoroutine(Skill2Cooldown(skillData2.Cooldown));
            });
        }
        else
        {
            Debug.Log("[Skill2] 스킬 없음 (ID=0)");
            skillButton.interactable = false;
        }
    }
    
    private IEnumerator Skill2Cooldown(float cooldown)
    {
        isSkill2OnCooldown = true;
        skillButton.interactable = false;
        Debug.Log($"[Skill2] 쿨타임 시작 ({cooldown}초)");
        
        yield return new WaitForSeconds(cooldown);
        
        isSkill2OnCooldown = false;
        skillButton.interactable = true;
        Debug.Log("[Skill2] 쿨타임 종료, 버튼 활성화");
    }
}
