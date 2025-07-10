using System;
using UnityEngine;
using UnityEngine.UI;

public class SkillTestHelper : MonoBehaviour
{
    [SerializeField] private PrefabsTable skillTable;
    
    [SerializeField] private Button skillButton1;
    [SerializeField] private Button skillButton2;
    
    [SerializeField] private GameObject player;

    private void Start()
    {
        // ������ ���� ��ų ���̵� �־��ֱ�
        var skill1ID = 2001;
        var skill2ID = 2002;
        
        // ������ ����
        var skill1Prefab = Instantiate(skillTable.GetPrefabByKey(skill1ID), player.transform);
        var skill2Prefab = Instantiate(skillTable.GetPrefabByKey(skill2ID), player.transform);

        // ��ų �ߵ� ���� ĳ��
        var skill1 = skill1Prefab.GetComponent<IEquipmentSkillBehaviour>();
        var skill2 = skill2Prefab.GetComponent<IEquipmentSkillBehaviour>();
        
        // ��ų ������ ĳ�̵� �ؾ���
        
        // �нú� ���� ��쿡�� ���� ������ �� �ٷ� ��������ִ� ������ �����ؾ� ��
        skillButton1.onClick.AddListener(() =>
        {
            skill1.Excute(player, EquipmentSkillType.Passive, 10f, 5f, 10f, 50f);
        });
        
        // ��Ƽ��� ��ư ������ �ߵ��ǰԲ� �����ؾ� ��
        skillButton2.onClick.AddListener(() =>
        {
            skill2.Excute(player, EquipmentSkillType.Active, 10f, 5f, 10f, 50f);
        });
    }
}
