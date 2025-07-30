using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectStageButton : MonoBehaviour
{
    [Header("Button Settings")]
    [SerializeField] private Button button; 
    [SerializeField] private TMP_Text stageName;
    public TMP_Text StageName => stageName;
    [SerializeField] private GameObject[] stars;
    [SerializeField] private GameObject lockObj;
    public GameObject LockObj => lockObj;
    public int CurrentStage { get; set; }


    public void Interactable(bool value)
    {
        button.interactable = value;
    }
    
    public void ActiveTrueStars(int starCount)
    {
        int count = 0;
        foreach (var starTrs in stars)
        {
            if (starCount == count) break;
            starTrs.gameObject.SetActive(true);
            count++;
        }
    }
    
    public void GoGameScene()
    {
        WholeSceneController.Instance.ReadyNextScene(CurrentStage);
        Base_Mng.Data.data.GamePlay++;
        Pinous_Flower_Holder.FlowerHolder.Clear();
        Base_Mng.Data.data.CurrentStage = CurrentStage;
        JsonDataLoader.SaveServerData(Base_Mng.Data.data);
    }
}
