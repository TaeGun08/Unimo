using System;
using UnityEngine;
using UnityEngine.UI;

public class SelectStage : MonoBehaviour
{
    private StageManager stageManager;

    [Header("StageButton")] [SerializeField]
    private GameObject prefab;

    private SelectStageButton[] buttons;
    public int stage;

    private void OnEnable()
    {
        if (buttons == null) return;
        ButtonsSetting();
    }

    private void Start()
    {
        stageManager = StageManager.Instance;

        if (buttons != null) return;
        buttons = new SelectStageButton[50];
        CreateButtons();
    }

    private void CreateButtons()
    {
        int index = stage;

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i] = Instantiate(prefab, transform).GetComponent<SelectStageButton>();
            buttons[i].StageName.text = $"Stage - {index}";
            buttons[i].ActiveTrueStars(stageManager.GetStars(index));
            buttons[i].CurrentStage = index;
            if (Base_Mng.Data.data.HighStage < index)
            {
                buttons[i].Interactable(false);
                buttons[i].LockObj.SetActive(true);
            }

            index++;
        }
    }

    private void ButtonsSetting()
    {
        int index = stage;

        foreach (var button in buttons)
        {
            button.StageName.text = $"Stage - {index}";
            button.ActiveTrueStars(stageManager.GetStars(index));
            button.CurrentStage = index;
            if (Base_Mng.Data.data.HighStage <= index)
            {
                button.Interactable(false);
            }

            index++;
        }
    }
}