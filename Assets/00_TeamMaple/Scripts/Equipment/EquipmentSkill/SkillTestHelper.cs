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
        // 엔진이 가진 스킬 아이디 넣어주기
        var skill1ID = 2001;
        var skill2ID = 2002;
        
        // 프리팹 생성
        var skill1Prefab = Instantiate(skillTable.GetPrefabByKey(skill1ID), player.transform);
        var skill2Prefab = Instantiate(skillTable.GetPrefabByKey(skill2ID), player.transform);

        // 스킬 발동 위해 캐싱
        var skill1 = skill1Prefab.GetComponent<IEquipmentSkillBehaviour>();
        var skill2 = skill2Prefab.GetComponent<IEquipmentSkillBehaviour>();
        
        // 스킬 데이터 캐싱도 해야함
        
        // 패시브 같은 경우에는 게임 시작할 때 바로 실행시켜주는 식으로 진행해야 함
        skillButton1.onClick.AddListener(() =>
        {
            skill1.Excute(player, EquipmentSkillType.Passive, 10f, 5f, 10f, 50f);
        });
        
        // 액티브는 버튼 누르면 발동되게끔 진행해야 함
        skillButton2.onClick.AddListener(() =>
        {
            skill2.Excute(player, EquipmentSkillType.Active, 10f, 5f, 10f, 50f);
        });
    }
}
