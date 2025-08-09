// ✅ Land.cs 전체 수정: 밸류 일괄 해금/저장 + Start 복구 + 로그
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Land : MonoBehaviour
{
    public static Land instance = null;

    [SerializeField] private LandAnimator[] animators;
    [SerializeField] private GameObject[] objects; // 메인 트리 비주얼(레벨 1~N)
    [SerializeField] private Transform[] landCheck;
    [SerializeField] private FlowerGenerator_Lobby[] Generators;
    public GameObject GlowParticle, StarParticle;
    public GameObject TestUnimo;
    List<GameObject> Unimos = new List<GameObject>();

    int[] value = { 2, 5, 6, 7, 8 };
    public int FlowerValue = 0;
    int CacheValue = 0;
    bool updated = false;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        // 저장된 해금 상태 복구
        if (Base_Mng.Data.data.unlockedLands == null)
            Base_Mng.Data.data.unlockedLands = new List<int>();

        foreach (int index in Base_Mng.Data.data.unlockedLands)
        {
            if (index >= 0 && index < objects.Length)
            {
                var go = objects[index].transform.parent.gameObject;
                go.SetActive(true);
                go.transform.localScale = Vector3.one;
                GetLandAnimation(index); // 애니 실행 보장
            }
        }

        // 최소 1레벨 비주얼 보장
        int level = Base_Mng.Data.data.Level;
        if (level >= 1 && objects.Length > 0)
        {
            var root = objects[0].transform.parent.gameObject;
            root.SetActive(true);
            root.transform.localScale = Vector3.one;
        }

        Generators[0].InitGenFlower();
        GetUnimo();
    }

    // ⭐ 밸류에 따른 일괄 해금 API
    // tier=0 → 1~3, tier=1 → 1~5, tier>=2 → 모두
    public void UnlockLandsByValue(int valueTier)
    {
        var list = Base_Mng.Data.data.unlockedLands;
        if (list == null) Base_Mng.Data.data.unlockedLands = list = new List<int>();

        int maxIdx = (objects != null) ? objects.Length - 1 : 0;
        int targetMax = (valueTier <= 0) ? 2 : (valueTier == 1 ? 4 : maxIdx);

        updated = false;
        for (int i = 0; i <= targetMax && i <= maxIdx; i++)
        {
            if (!list.Contains(i))
            {
                list.Add(i);
                var go = objects[i].transform.parent.gameObject;
                go.SetActive(true);
                go.transform.localScale = Vector3.one;
                GetLandAnimation(i);
                updated = true;
            }
        }

        if (updated)
        {
            Base_Mng.Data.Save();
            Debug.Log($"[🌱 Land] 일괄 해금 저장 완료 (tier={valueTier}) → {string.Join(",", list)}");
        }
    }

    public void GetUnimo()
    {
        for (int i = 0; i < Base_Mng.Data.AltaCount.Length; i++)
            if (i > 0 && Base_Mng.Data.data.Level >= Base_Mng.Data.AltaCount[i - 1])
                CacheValue++;

        for (int i = 0; i < Base_Mng.Data.data.GetCharacterData.Length; i++)
            if (Base_Mng.Data.data.GetCharacterData[i])
                GetCharacter(i);
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
        GlowParticle.SetActive(false);
        StarParticle.SetActive(false);

        yield return new WaitForSeconds(0.5f);
        Canvas_Holder.instance.GetLock(true);
        Camera_Event.instance.ReturnCamera();
        yield return new WaitForSeconds(1.0f);
        Canvas_Holder.instance.Get_Toast("LevelUP02");

        // 다음 두 칸 해금 & 저장
        updated = false;
        if (!Base_Mng.Data.data.unlockedLands.Contains(value))
        {
            Base_Mng.Data.data.unlockedLands.Add(value);
            updated = true;
        }
        if (!Base_Mng.Data.data.unlockedLands.Contains(value + 1))
        {
            Base_Mng.Data.data.unlockedLands.Add(value + 1);
            updated = true;
        }
        if (updated)
        {
            Base_Mng.Data.Save();
            Debug.Log($"[🌱 Land] Save 호출됨. 해금 인덱스: {value},{value + 1} / 리스트: {string.Join(",", Base_Mng.Data.data.unlockedLands)}");
        }

        // 해금된 애니도 실행
        int nextIndex = value * 2;
        if (nextIndex < animators.Length) GetLandAnimation(nextIndex);
        if (nextIndex + 1 < animators.Length) GetLandAnimation(nextIndex + 1);
    }
}
