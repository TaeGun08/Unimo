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

[CreateAssetMenu(fileName = "StageData", menuName = "Scriptable Object/StageData")]
public class StageData : ParsingStageData<StageDataRecord> { }
