using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSkillUIController : MonoBehaviour
{
    // 스킬 Sprite 테이블
    [SerializeField] private SpriteTable skillSpriteTable;
    
    [SerializeField] private Image[] skillSprites;
    [SerializeField] private Image[] skillCooldownFills;
    [SerializeField] private TMP_Text[] skillCooldownTexts;
    
    public void SetSkillSprite(int idx, int skillId)
    {
        Sprite sprite = skillSpriteTable.GetSpriteByKey(skillId);
        skillSprites[idx].sprite = sprite;
        skillSprites[idx].gameObject.SetActive(true);
    }

    public void ResetSkillSprite(int idx)
    {
        skillSprites[idx].gameObject.SetActive(false);
    }

    public void OnCooldownUI(int idx, float cooldown)
    {
        skillCooldownFills[idx].fillAmount = 1f;
        skillCooldownTexts[idx].text = Mathf.Ceil(cooldown).ToString("F0");
        skillCooldownFills[idx].gameObject.SetActive(true);
        skillCooldownTexts[idx].gameObject.SetActive(true);
    }

    public void UpdateCooldown(int idx, float fill, float text)
    {
        skillCooldownFills[idx].fillAmount = fill;
        skillCooldownTexts[idx].text = text.ToString("F0");
    }

    public void OffCooldownUI(int idx)
    {
        skillCooldownFills[idx].gameObject.SetActive(false);
        skillCooldownTexts[idx].gameObject.SetActive(false);
        skillCooldownFills[idx].fillAmount = 0f;
        skillCooldownTexts[idx].text = "";
    }
}
