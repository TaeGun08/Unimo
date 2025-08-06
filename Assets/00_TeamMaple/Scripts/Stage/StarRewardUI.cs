using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StarRewardUI : MonoBehaviour
{
    [SerializeField] private Slider stageSlider;
    [SerializeField] private UI_Game uiGame;
    
    private void OnEnable()
    {
        UpdateSlider();
    }

    private void UpdateSlider()
    {
        if (StageManager.Instance == null)
        {
            stageSlider.value = 0f;
            return;
        }

        int starCount = 0;
        int startStage = ((uiGame.StageCount - 1) / 50) * 50 + 1;

        for (int i = 0; i < 50; i++)
        {
            int currentStage = startStage + i;
            starCount += StageManager.Instance.GetStars(currentStage + 1000);
        }

        stageSlider.value = starCount / 135f;
    }
}
