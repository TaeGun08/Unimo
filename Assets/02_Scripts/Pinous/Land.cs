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

    private List<GameObject> Unimos = new List<GameObject>();
    public int FlowerValue = 0;
    private int[] landIndexTable = { 2, 5, 6, 7, 8 }; // 랜드 위치 랜덤 참조용
    private int CacheValue = 0;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        // 🌱 기본 꽃 생성기 활성화
        Generators[0].InitGenFlower();

        // 🌱 껐다 켜도 해금 랜드 복구
        foreach (int index in Base_Mng.Data.data.unlockedLands)
        {
            if (index >= 0 && index < animators.Length)
            {
                animators[index].gameObject.SetActive(true);
                animators[index].DefaultInitalize();
            }
        }

        // 🌲 별나무 외형 복원
        UpdateMainTreeVisual(Base_Mng.Data.data.Level);

        // 🧍‍♀️ 유니모 등장 처리
        GetUnimo();
    }

    public void UpdateMainTreeVisual(int level)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].SetActive(i == level - 1);
        }
    }

    public void UnlockLand(int index)
    {
        if (index < 0 || index >= animators.Length) return;

        if (!Base_Mng.Data.data.unlockedLands.Contains(index))
            Base_Mng.Data.data.unlockedLands.Add(index);

        StartCoroutine(LevelUpAlta(index));
    }

    public void UnlockMultipleLands(int[] indices)
    {
        foreach (int i in indices)
        {
            UnlockLand(i);
        }
    }

    IEnumerator LevelUpAlta(int index)
    {
        if (index < 0 || index >= animators.Length)
            yield break;

        Canvas_Holder.instance.GetLock(false);
        Camera_Event.instance.GetCameraEvent(CameraMoveState.Alta_LevelUP);

        yield return new WaitForSeconds(0.3f);
        Canvas_Holder.instance.Get_Toast("LevelUP01");

        var animator = animators[index];
        animator.gameObject.SetActive(true);
        animator.Initalize();

        StarParticle.SetActive(true);
        Sound_Manager.instance.Play(Sound.Effect, "effect_1001");

        yield return new WaitForSeconds(1.0f);
        StarParticle.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        Canvas_Holder.instance.GetLock(true);
        Camera_Event.instance.ReturnCamera();
        yield return new WaitForSeconds(1.0f);
        Canvas_Holder.instance.Get_Toast("LevelUP02");
    }

    public void GetUnimo()
    {
        CacheValue = 0;
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
        int value02 = value01 <= 0 ? 0 : value01;
        var land = landCheck[Random.Range(0, landIndexTable[value02])];
        Debug.Log(landIndexTable[value02]);

        var go = Instantiate(TestUnimo, new Vector3(
            land.position.x + Random.Range(-3.0f, 3.0f),
            land.position.y + 8.0f,
            land.position.z + Random.Range(-1.5f, 1.5f)),
            Quaternion.identity, transform).GetComponent<TempUnimoSetter>();

        go.GetComponent<AI_Move>().Name = i.ToString();
        go.currentChar = i + 1;
        go.currentEq = i + 1;
    }

    public void GetLandAnimation(int value)
    {
        animators[value].gameObject.SetActive(true);
        animators[value].Initalize();
    }

    public void GetLevelUpAlta(int value)
    {
        StartCoroutine(LevelUpAlta(value));
    }
}
