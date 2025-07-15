using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class UI_Game : UI_Base
{
    public TextMeshProUGUI GameOneBest, GameTwoBest;
    private int stageCount;
    
    [SerializeField] private TMP_Text stageText;
    
    public override void Start()
    {
        Camera_Event.instance.GetCameraEvent(CameraMoveState.InGame);
        GameOneBest.text = "Best Score\n" + StringMethod.ToCurrencyString(Base_Mng.Data.data.BestScoreGameOne);
        GameTwoBest.text = "Best Score\n" + StringMethod.ToCurrencyString(Base_Mng.Data.data.BestScoreGameTwo);
        base.Start();

        stageCount = JsonDataLoader.LoadServerData().CurrentStage;
    }

    public override void DisableOBJ()
    {
        base.DisableOBJ();
    }

    public void GoGameScene(int value)
    {
        WholeSceneController.Instance.ReadyNextScene(value);
        Base_Mng.Data.data.GamePlay++;
        Pinous_Flower_Holder.FlowerHolder.Clear();
        Base_Mng.Data.data.CurrentStage = stageCount;
        //Base_Mng.ADS._interstitialCallback = () =>
        //{
           
        //};
        //Base_Mng.ADS.ShowInterstitialAds();
    }

    private void SetStageText()
    {
        stageText.text = $"Stage-{stageCount}";
    }

    public void StageCountUp()
    {
        stageCount++;
        if (StageLoader.HighStageChecker(stageCount))
        {
            stageCount--;
            return;
        }
        
        SetStageText();
    }

    public void StageCountDown()
    {
        stageCount--;
        if (0 >= stageCount)
        {
            stageCount = 1;
            return;
        }
        
        SetStageText();
    }
}
