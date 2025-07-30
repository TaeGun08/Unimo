using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class EngineSkillUI : MonoBehaviour
{
   [SerializeField] private UnimoStatUI unimoStatUI;
   [SerializeField] private GameObject skillUIPrefab;
   [SerializeField] private EquipmentSkillDataSO EQSkillDataSO;

   private EquipmentSkillData skillType1;
   private EquipmentSkillData skillType2;

   [Header("PrefabUISettings")]
   [SerializeField] private RectTransform contentParent;
   // [SerializeField] private TMP_Text Name;
   // [SerializeField] private TMP_Text Type;
   // [SerializeField] private TMP_Text Description;
   
   private void Start()
   {
      StartCoroutine(EngineSkillUISetting(unimoStatUI.equipmentStatData));
      UpdataSkillUISetting();
   }

   private IEnumerator EngineSkillUISetting(EquipmentStatData data)
   {
      yield return new WaitForSeconds(1f);
      
      skillType1 = GetSkillData(data.Skill1, data.Level);
      skillType2 = GetSkillData(data.Skill2, data.Level);
   }
   
   /// <summary>
   /// 스킬 ID와 레벨을 받아서 최종 스킬 데이터를 반환
   /// </summary>
   private EquipmentSkillData GetSkillData(int skillId, int level)
   {
      return skillId != 0 
         ? EQSkillDataSO.GetFinalEquipmentSkillData(skillId, level) 
         : null;
   }

   private void UpdataSkillUISetting()
   {
      SetSkillData(skillType1);
      SetSkillData(skillType2);
   }
   
   /// <summary>
   /// Data UI 처리
   /// </summary>
   private void SetSkillData(EquipmentSkillData data)
   {
      if(data == null) return;
      
      GameObject tmp = Instantiate(skillUIPrefab, contentParent, false);
      TMP_Text[] tmps = tmp.GetComponentsInChildren<TMP_Text>();
      
      tmps[0].text = data.Id.ToString();
      tmps[1].text = data.Name;
      tmps[2].text = data.Type.ToString();
      tmps[3].text = data.Description;
      tmps[4].text = data.Cooldown.ToString();
      tmps[5].text = data.Duration.ToString();
   }
}
