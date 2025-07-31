using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EngineSkillUI : MonoBehaviour
{
   [SerializeField] private UnimoStatUI unimoStatUI;
   [SerializeField] private GameObject skillUIPrefab;
   [SerializeField] private EquipmentSkillDataSO EQSkillDataSO;
   [SerializeField] private SpriteTable spriteTable;
   
   private EquipmentSkillData skillType1;
   private EquipmentSkillData skillType2;

   [Header("PrefabUISettings")]
   [SerializeField] private RectTransform contentParent;
   private float currentY = 0;

   private void OnEnable()
   {
      StartCoroutine(EngineSkillUISetting(unimoStatUI.equipmentStatData));
   }

   private IEnumerator EngineSkillUISetting(EquipmentStatData data)
   {
      yield return new WaitForSeconds(0.1f);
      
      skillType1 = GetSkillData(data.Skill1, data.Level);
      skillType2 = GetSkillData(data.Skill2, data.Level);

      UpdataSkillUISetting();
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
      // 기존 UI 초기화
      foreach (Transform child in contentParent)
      {
         Destroy(child.gameObject);
      }
      currentY = 0;
      
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
      Image img = tmp.GetComponentsInChildren<Image>(true)[1]; 
      img.sprite = spriteTable.GetSpriteByKey(data.Id);
      
      TMP_Text[] tmps = tmp.GetComponentsInChildren<TMP_Text>();
      RectTransform rt = tmp.GetComponent<RectTransform>();
      
      float startY = 130f;
      float offsetY = startY - (330f * currentY);
      rt.anchoredPosition = new Vector2(0f, offsetY);
      currentY++;
      
      tmps[0].text = data.Id.ToString();
      tmps[1].text = data.Name;
      tmps[2].text = data.Type.ToString();
      tmps[3].text = data.Description;
      tmps[4].text = data.Cooldown.ToString();
      tmps[5].text = data.Duration.ToString();
   }
}
