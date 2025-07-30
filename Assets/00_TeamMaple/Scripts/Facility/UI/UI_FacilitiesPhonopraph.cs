using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_FacilitiesPhonograph : UI_Base
{
    [SerializeField] private Button exitButton;
    [SerializeField] private TextMeshProUGUI infoText;

    public override void Start()
    {
        base.Start();

        exitButton.onClick.AddListener(CloseSelf);

        SetInfoText();
    }

    private void SetInfoText()
    {
        string text = "";

        switch (Localization_Mng.LocalAccess)
        {
            case "kr":
                text = "유니모의 순발력을 끌어올리는\n특별한 장치, 축음기!\n\n이동속도 +1.5% 증가!";
                break;
            case "en":
                text = "A special device that boosts\nUnimo’s reflexes — the Phonograph!\n\nMovement speed +1.5% boost!";
                break;
            case "es":
                text = "Un dispositivo especial que mejora\nlos reflejos de Unimo: ¡el fonógrafo!\n\nVelocidad de movimiento +1.5% aumentada";
                break;
        }

        infoText.text = text;
    }


    private void CloseSelf()
    {
        Canvas_Holder.CloseAllPopupUI();
    }
}