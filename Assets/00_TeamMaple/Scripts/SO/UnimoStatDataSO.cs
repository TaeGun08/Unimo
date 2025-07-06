using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System.IO;
using CsvHelper;

[System.Serializable]
public class UnimoData
{
    public int unimo_id { get; set; }
    public string name { get; set; }
    public int hp { get; set; }
    public int def { get; set; }
    public int speed { get; set; }
    public int bloom_range { get; set; }
    public float bloom_spd { get; set; }
    public float flower_rate { get; set; }
    public float rare_flower_rate { get; set; }
    public float dodge { get; set; }
    public float stun_recovery { get; set; }
    public float hp_recovery { get; set; }
    public float drop_speed { get; set; }
    public float drop_count { get; set; }
}

[CreateAssetMenu(fileName = "UnimoStatDataSO", menuName = "Scriptable Object/UnimoStatDataSO")]
public class UnimoStatDataSO : ScriptableObject
{
    [Header("UnimoDataCsv")]
    [SerializeField] private TextAsset unimoDataCsv;

    /// <summary>
    /// �Է� ���� ���̵� ���� ������ ��ȯ
    /// </summary>
    public UnimoData GetUnimoData(int unimoID)
    {
        if (unimoDataCsv == null)
        {
            Debug.LogError("unimoDataCsv is null");
            return null;
        }

        using (StringReader reader = new StringReader(unimoDataCsv.text))
        using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Read(); // "���ϸ� ID", "�̸�", ...
            csv.Read(); // unimo_id, name, hp, ...
            csv.ReadHeader();   // ��� ���

            csv.Read(); //������ Ÿ��
            
            IEnumerable<UnimoData> records = csv.GetRecords<UnimoData>();

            foreach (UnimoData record in records)
            {
                if (record.unimo_id == unimoID)
                    return record;
            }
        }

        Debug.LogWarning($"UnimoData with ID {unimoID} not found.");
        return null;
    }
}