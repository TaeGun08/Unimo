using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerGenerator : MonoBehaviour
{
    [HideInInspector] public List<FlowerController> AllFlowers;

    [SerializeField] protected List<GameObject> flowerObjs;
    [SerializeField] protected List<float> appearRatios;
    protected List<float> appearAccProb;

    protected int gatheredFlowers = 0; 

    protected void Awake()
    {
        if (flowerObjs.Count != appearRatios.Count)
        {
            Debug.LogError("FlowerGenerator: flowerObjs and appearRatios counts do not match.");
        }
        
        float totalRatio = 0f;
        appearAccProb = new List<float>();
        foreach (float ratio in appearRatios)
        {
            totalRatio += ratio;
        }
        
        float cumulativeProb = 0f;
        foreach (float ratio in appearRatios)
        {
            cumulativeProb += ratio / totalRatio;
            appearAccProb.Add(cumulativeProb);
        }

        if (appearAccProb.Count <= 0) return;
        appearAccProb[^1] = 1f;
    }

    protected void Start()
    {
        AllFlowers = new List<FlowerController>();
        StartCoroutine(generateCoroutine());
    }
    
    virtual public void GatherFlower()
    {
        ++gatheredFlowers;
    }
    
    virtual protected void generateFlower()
    {
        float rand = Random.Range(0f, 1f);
        int idx = 0;
        
        while (idx < appearAccProb.Count && rand > appearAccProb[idx])
        {
            ++idx;
        }
        
        if (idx >= flowerObjs.Count)
        {
            idx = flowerObjs.Count - 1;
        }
        
        FlowerController flower = Instantiate(flowerObjs[idx], findPosition(), setRotation())
            .GetComponent<FlowerController>();
        flower.InitFlower(this);
    }
    
    virtual protected Vector3 findPosition()
    {
        return Vector3.zero;
    }
    
    virtual protected Quaternion setRotation()
    {
        return Quaternion.identity;
    }
    
    virtual protected IEnumerator generateCoroutine()
    {
        yield break;
    }
}
