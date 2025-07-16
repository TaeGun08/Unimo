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
        // ������ ���� ��ų ���̵� �־��ֱ� (���� �����Ϳ��� ��������)
        var skillId1 = 2001;
        var skillId2 = 2020;
        
        // ������ ����
        var skillPrefab1 = Instantiate(skillTable.GetPrefabByKey(skillId1), player.transform);
        var skillPrefab2 = Instantiate(skillTable.GetPrefabByKey(skillId2), player.transform);

        // ��ų �ߵ� ���� ĳ��
        var skillExcutor1 = skillPrefab1.GetComponent<IEquipmentSkillBehaviour>();
        var skillExcutor2 = skillPrefab2.GetComponent<IEquipmentSkillBehaviour>();
        
        // ��ų ������ ĳ��
        var skillData1 = skillDataSO.GetEquipmentSkillData(skillId1);
        var skillData2 = skillDataSO.GetEquipmentSkillData(skillId2);

        // �нú� ����
        skillExcutor1.Excute(player, skillData1.Type, skillData1.Duration, skillData1.Param);
        Debug.Log($"[Skill1] " +
                  $"Id: {skillId1} / " +
                  $"Type: {skillData1.Type} / " +
                  $"Cooldown: {skillData1.Cooldown} / " +
                  $"Duration: {skillData1.Duration} / " +
                  $"Param: {skillData1.Param}");

        // ��Ƽ�� ���
        skillButton.onClick.AddListener(() =>
        {
            skillExcutor2.Excute(player, skillData2.Type, skillData2.Duration, skillData2.Param);
            Debug.Log($"[Skill2] " +
                      $"Id: {skillId2} / " +
                      $"Type: {skillData2.Type} / " +
                      $"Cooldown: {skillData2.Cooldown} / " +
                      $"Duration: {skillData2.Duration} / " +
                      $"Param: {skillData2.Param}");
        });
    }
}
