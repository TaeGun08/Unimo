using System;
using System.Collections;
using UnityEngine;

public class MapChanger : MonoBehaviour
{
    [Header("Map Materials")] [SerializeField]
    private Renderer bg;
    [SerializeField] private Renderer land;
    [SerializeField] private Renderer rock;
    
    private IEnumerator Start()
    {
        yield return null;
        //bg.materials = StageManager.Instance.StageData.GetData(Base_Mng.Data.data.CurrentStage).
    }
}
