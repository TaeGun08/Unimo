using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_FacilitiesKettle : UI_Base
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
            "kr" => "교환소의 핵심 동력, 주전자!\n파랑 별꿀을 노랑/주황 별꿀로 바꾸는 데 필수!",
            "en" => "The core of the exchange booth — the Kettle!\nEssential for converting Blue Star Honey into Yellow or Orange ones!",
            "es" => "El núcleo del puesto de intercambio: ¡la Tetera!\n¡Esencial para convertir miel azul en amarilla o naranja!",
            _ => ""
        };
        infoText.text = text;
    }

    private void CloseSelf()
    {
        Canvas_Holder.CloseAllPopupUI();
    }
}
