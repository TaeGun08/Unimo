using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SelectStage : MonoBehaviour
{
    private StageManager stageManager;

    [Header("StageButton")] [SerializeField]
    private GameObject prefab;

    private SelectStageButton[] buttons;
    public int Stage { get; set; }

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
        int index = Stage;

        for (int i = 0; i < buttons.Length; i++)
        {
            if (!StageLoader.IsBonusStageByIndex(index))
            {
                buttons[i] = Instantiate(prefab, transform).GetComponent<SelectStageButton>();
                buttons[i].StageName.text = $"Stage - {index}";
                buttons[i].ActiveTrueStars(stageManager.GetStars(index + 1000));
                buttons[i].CurrentStage = index;
                buttons[i].Interactable(true);
                buttons[i].LockObj.SetActive(false);
                if (Base_Mng.Data.data.HighStage < index)
                {
                    buttons[i].Interactable(false);
                    buttons[i].LockObj.SetActive(true);
                }
            }

            index++;
        }
    }

    private void ButtonsSetting()
    {
        int index = Stage;

        foreach (var button in buttons)
        {
            if (!StageLoader.IsBonusStageByIndex(index))
            {
                button.StageName.text = $"Stage - {index}";
                button.ActiveTrueStars(stageManager.GetStars(index + 1000));
                button.CurrentStage = index;
                button.Interactable(true);
                button.LockObj.SetActive(false);
                if (Base_Mng.Data.data.HighStage < index)
                {
                    button.Interactable(false);
                    button.LockObj.SetActive(true);
                }
            }

            index++;
        }
    }
}