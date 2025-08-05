using UnityEngine;

[System.Serializable]
public class StageRewardDataRecord : IStageData
{
    public int Id { get; set; } //�������� ���� ���̵�
    public string Star1Y { get; set; } //�������� 1�� �޼� ����
    public string Star2R { get; set; } //�������� 2�� �޼� ����
    public string Star3Y { get; set; } //�������� 3�� �޼� ����
    public string Star3R { get; set; } //�������� 3�� �޼� ����
}

[System.Serializable]
public class StageDataRecord : IStageData
{
    public int Id { get; set; }
    public string StageName { get; set; } //�������� �̸�
    public string PlanetName { get; set; } //�༺ �̸�
}

[System.Serializable]
public class PlanetData
{
    public int ReturnId;
    public Sprite PlanetSprite;
    public Material BG;
    public Material Land;
    public Material Rock;
}

[CreateAssetMenu(fileName = "StageData", menuName = "Scriptable Object/StageData")]
public class StageData : ParsingStageData<StageDataRecord>
{
    [Header("Planet Image")] [SerializeField]
    private PlanetData[] planetData;

    public PlanetData GetPlanetData(int index)
    {
        if (StageLoader.IsBonusStageByIndex(index)) return null;
        switch (GetData(index + 1000).PlanetName)
        {
            case "������":
            case "������_��Ȧ":
                return planetData[0];
            case "�����Ǻ�":
            case "������ ��_��Ȧ":
                return planetData[1];
            case "�ʷϰ��� ��":
            case "�ʷϰ�����_��Ȧ":
                return planetData[2];
            case "��������":
            case "��������_��Ȧ":
                return planetData[3];
            case "�����ϴº�":
            case "�����ϴº�_��Ȧ":
                return planetData[4];
            case "������":
            case "������_��Ȧ":
                return planetData[5];
            case "��ǳ�� ��":
            case "��ǳ�Ǻ�_��Ȧ":
                return planetData[6];
            case "�Ȱ����� ��":
            case "�Ȱ�������_��Ȧ":
                return planetData[7];
            case "�Ҿ����� ��":
            case "�Ҿ����Ѻ�_��Ȧ":
                return planetData[8];
            case "�ð��� ��":
            case "�ð��Ǻ�_��Ȧ":
                return planetData[9];
        }
        
        return planetData[0];
    }
}