using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_FacilitiesFountain : UI_Base
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
            "kr" => "유니모의 방어력과 회피율을 높여\n스테이지 생존력을 강화하는 신비한 분수대!",
            "en" => "A mystical fountain that enhances\nUnimo’s defense and evasion for stage survival!",
            "es" => "¡Una fuente mística que aumenta la defensa\ny evasión de Unimo para sobrevivir mejor en los escenarios!",
            _ => ""
        };
        infoText.text = text;
    }

    private void CloseSelf()
    {
        Canvas_Holder.CloseAllPopupUI();
    }
}