using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_FacilitiesTypewriter : UI_Base
{
    [SerializeField] private Button exitButton;
    [SerializeField] private TextMeshProUGUI infoText;

    private void OnEnable()
    {
        exitButton.onClick.AddListener(CloseSelf);
        SetInfoText();
    }

    private void SetInfoText()
    {
        string text = Localization_Mng.LocalAccess switch
        {
            "kr" => "유니모가 재화 획득을 더 빠르게!\n별꿀 생산 속도를 향상시키는 마법 타자기!",
            "en" => "Make Unimos faster at gaining resources!\nA magical typewriter that boosts Star Honey production speed!",
            "es" => "¡Haz que los Unimos consigan recursos más rápido!\n¡Una máquina de escribir mágica que acelera la producción de miel estelar!",
            _ => ""
        };
        infoText.text = text;
    }

    private void CloseSelf()
    {
        Canvas_Holder.CloseAllPopupUI();
    }
}