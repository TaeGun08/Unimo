using System;
using UnityEngine;
using UnityEngine.UI;

public class SkillTestHelper : MonoBehaviour
{
    [SerializeField] private PrefabsTable skillTable;
    [SerializeField] private Button skillButton;
    [SerializeField] private GameObject player;
    [SerializeField] private EquipmentSkillDataSO skillDataSO;

    private void Start()
    {
        SetEngineSkills();
    }

    private void SetEngineSkills()
    {
        // 엔진이 가진 스킬 아이디 넣어주기 (엔진 데이터에서 갖고오기)
        var skillId1 = 2001;
        var skillId2 = 2002;
        
        // 프리팹 생성
        var skillPrefab1 = Instantiate(skillTable.GetPrefabByKey(skillId1), player.transform);
        var skillPrefab2 = Instantiate(skillTable.GetPrefabByKey(skillId2), player.transform);

        // 스킬 발동 위해 캐싱
        var skillExcutor1 = skillPrefab1.GetComponent<IEquipmentSkillBehaviour>();
        var skillExcutor2 = skillPrefab2.GetComponent<IEquipmentSkillBehaviour>();
        
        // 스킬 데이터 캐싱
        var skillData1 = skillDataSO.GetEquipmentSkillData(skillId1);
        var skillData2 = skillDataSO.GetEquipmentSkillData(skillId2);

        // 패시브 적용
        skillExcutor1.Excute(player, skillData1.Type, skillData1.Cooldown, skillData1.Duration, skillData1.Param);
        Debug.Log($"[Skill1] " +
                  $"Id: {skillId1} / " +
                  $"Type: {skillData1.Type} / " +
                  $"Cooldown: {skillData1.Cooldown} / " +
                  $"Duration: {skillData1.Duration} / " +
                  $"Param: {skillData1.Param}");

        // 액티브 등록
        skillButton.onClick.AddListener(() =>
        {
            skillExcutor2.Excute(player, skillData2.Type, skillData2.Cooldown, skillData2.Duration, skillData2.Param);
            Debug.Log($"[Skill2] " +
                      $"Id: {skillId2} / " +
                      $"Type: {skillData2.Type} / " +
                      $"Cooldown: {skillData2.Cooldown} / " +
                      $"Duration: {skillData2.Duration} / " +
                      $"Param: {skillData2.Param}");
        });
    }
}
