using UnityEngine;

[System.Serializable]
public class StageGenerateFlowerDataRecord : IStageData
{
    public int Id { get; set; }
    public float YGeneration { get; set; } //����� ���� Ȯ��
    public float RGeneration { get; set; } //������ ���� Ȯ��
    public float BGeneration { get; set; } //�Ķ��� ���� Ȯ��
}

[CreateAssetMenu(fileName = "StageGenerateFlowerData", menuName = "Scriptable Object/StageGenerateFlowerData")]
public class StageGenerateFlowerData : ParsingStageData<StageGenerateFlowerDataRecord> { }
