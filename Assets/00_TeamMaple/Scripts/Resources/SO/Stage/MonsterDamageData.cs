using System;
using UnityEngine;

[System.Serializable]
public class MonsterDamageDataRecord : IStageData
{
    public int Id { get; set; }
    public string Monster_1Damage { get; set; }
    public string Monster_2Damage { get; set; }
    public string Monster_3Damage { get; set; }
    public string Monster_4Damage { get; set; }
    public string Monster_5Damage { get; set; }
}

[CreateAssetMenu(fileName = "MonsterDamageData", menuName = "Scriptable Object/MonsterDamageData")]
public class MonsterDamageData : ParsingStageData<MonsterDamageDataRecord>
{
    public float GetMonsterDamage(int id, Monstertype type)
    {
        switch (type)
        {
            case Monstertype.Monster1:
                return float.Parse(GetData(id + 1000).Monster_1Damage);
            case Monstertype.Monster2:
                return float.Parse(GetData(id + 1000).Monster_2Damage);
            case Monstertype.Monster3:
                return float.Parse(GetData(id + 1000).Monster_3Damage);
            case Monstertype.Monster4:
                return float.Parse(GetData(id + 1000).Monster_4Damage);
            case Monstertype.Monster5:
                return float.Parse(GetData(id + 1000).Monster_5Damage);
        }

        return 0;
    }
}
