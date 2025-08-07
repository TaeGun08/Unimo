using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Land : MonoBehaviour
{
    public static Land instance = null;

   
    [SerializeField] private LandAnimator[] animators;
    [SerializeField] private GameObject[] objects; // MainTree 비주얼용 (레벨 1~5)
    [SerializeField] private Transform[] landCheck;
    [SerializeField] private FlowerGenerator_Lobby[] Generators;
    public GameObject GlowParticle, StarParticle;
    public GameObject TestUnimo;
    List<GameObject> Unimos = new List<GameObject>();

    int[] value = { 2, 5, 6, 7, 8 };
    public int FlowerValue = 0;
    int CacheValue = 0;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        ResetAllLands();

        int level = Base_Mng.Data.data.Level;

        if (level >= 1 && objects.Length > 0)
        {
            objects[0].transform.parent.gameObject.SetActive(true);
            objects[0].transform.parent.localScale = Vector3.one;
        }

        for (int i = 0; i < Base_Mng.Data.AltaCount.Length; i++)
        {
            if (level >= Base_Mng.Data.AltaCount[i])
            {
                var go = objects[i + 1].transform.parent.gameObject;
                go.SetActive(true);
                go.transform.localScale = Vector3.one;
            }
        }

        Generators[0].InitGenFlower();
        GetUnimo();
    }

    private void ResetAllLands()
    {
        foreach (var anim in animators)
        {
            anim.gameObject.SetActive(false);
        }
    }

    public void GetUnimo()
    {
        for (int i = 0; i < Base_Mng.Data.AltaCount.Length; i++)
        {
            if (i > 0 && Base_Mng.Data.data.Level >= Base_Mng.Data.AltaCount[i - 1])
            {
                CacheValue++;
            }
        }
        for (int i = 0; i < Base_Mng.Data.data.GetCharacterData.Length; i++)
        {
            if (Base_Mng.Data.data.GetCharacterData[i])
            {
                GetCharacter(i);
            }
        }
    }

    public void GetCharacter(int i)
    {
        int value01 = CacheValue - 1;
        int value02 = Mathf.Max(0, value01);
        var land = landCheck[Random.Range(0, value[value02])];
        var go = Instantiate(TestUnimo, new Vector3(land.position.x + Random.Range(-3.0f, 3.0f),
            land.position.y + 8.0f, land.position.z + Random.Range(-1.5f, 1.5f)), Quaternion.identity, transform).GetComponent<TempUnimoSetter>();
        go.GetComponent<AI_Move>().Name = i.ToString();
        go.currentChar = i + 1;
        go.currentEq = i + 1;
    }

    public void GetLandAnimation(int index)
    {
        if (index < animators.Length)
        {
            animators[index].gameObject.SetActive(true);
            animators[index].Initalize();
        }
    }

    public void GetLevelUpAlta(int value)
    {
        StartCoroutine(LevelUpAlta(value));
    }

    IEnumerator LevelUpAlta(int value)
    {
        Canvas_Holder.instance.GetLock(false);
        Camera_Event.instance.GetCameraEvent(CameraMoveState.Alta_LevelUP);

        Base_Mng.Data.data.TreeLevelUp++;
        
        yield return new WaitForSeconds(0.3f);
        Canvas_Holder.instance.Get_Toast("LevelUP01");

        var alta = objects[value].transform.parent.gameObject;
        float current = 0, percent = 0;
        GlowParticle.SetActive(true);

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / 1.0f;

            Color color = Color.Lerp(Color.black, Color.white, percent);
            for (int i = 0; i < objects[value].transform.childCount; i++)
            {
                var obj = objects[value].transform.GetChild(i).GetComponent<Renderer>();
                obj.material.SetColor("_EmissionColor", color);
            }
            yield return null;
        }

        current = percent = 0f;
        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / 0.2f;
            alta.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, percent);
            yield return null;
        }
        alta.SetActive(false);

        if (value + 1 < objects.Length)
        {
            var next = objects[value + 1].transform.parent.gameObject;
            next.SetActive(true);
            next.transform.localScale = Vector3.zero;
            current = percent = 0f;
            while (percent < 1f)
            {
                current += Time.deltaTime;
                percent = current / 0.2f;
                next.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, percent);
                yield return null;
            }
        }

        StarParticle.SetActive(true);
        Sound_Manager.instance.Play(Sound.Effect, "effect_1001");

        yield return new WaitForSeconds(1.0f);
        StarParticle.SetActive(false);

        yield return new WaitForSeconds(0.5f);
        Canvas_Holder.instance.GetLock(true);
        Camera_Event.instance.ReturnCamera();
        yield return new WaitForSeconds(1.0f);
        Canvas_Holder.instance.Get_Toast("LevelUP02");

        // 랜드 해금 처리
        int nextIndex = value * 2;
        if (nextIndex < animators.Length) GetLandAnimation(nextIndex);
        if (nextIndex + 1 < animators.Length) GetLandAnimation(nextIndex + 1);

        if (value == 2 && !Base_Mng.Data.data.GetVane)
        {
            yield return new WaitForSeconds(0.5f);
            Base_Mng.Data.data.GetVane = true;
            Canvas_Holder.instance.GetUI("##Vane");
        }
    }
}