using System;
using System.Collections.Generic;

public static class UnimoStatExtensions
{
    private static readonly Dictionary<UnimoStat, string> _ko = new()
    {
        { UnimoStat.None, "" },
        { UnimoStat.All, "모든 스텟 증가" },
        { UnimoStat.Hp, "체력" },
        { UnimoStat.Def, "방어력" },
        { UnimoStat.Speed, "이동속도" },
        { UnimoStat.BloomRange, "개화 범위" },
        { UnimoStat.BloomSpeed, "개화 속도" },
        { UnimoStat.FlowerRate, "꽃 생성 주기" },
        { UnimoStat.RareFlowerRate, "희귀 꽃 확률" },
        { UnimoStat.Dodge, "회피율" },
        { UnimoStat.StunRecovery, "스턴 회복력" },
        { UnimoStat.HpRecovery, "체력 재생" },
        { UnimoStat.FlowerDropSpeed, "꽃 낙하 속도" },
        { UnimoStat.FlowerDropAmount, "낙하량 증가" },
    };

    private static readonly HashSet<UnimoStat> _percentStats = new()
    {
        UnimoStat.BloomSpeed,
        UnimoStat.FlowerRate,
        UnimoStat.RareFlowerRate,
        UnimoStat.Dodge,
        UnimoStat.StunRecovery,
        UnimoStat.HpRecovery,
        UnimoStat.FlowerDropSpeed,
        UnimoStat.FlowerDropAmount
    };

    public static string Ko(this UnimoStat stat)
        => _ko.TryGetValue(stat, out var name) ? name : stat.ToString();

    public static string Format(this UnimoStat stat, float value, int? decimals = null)
    {
        int d = decimals ?? (stat == UnimoStat.Speed ? 2 : (_percentStats.Contains(stat) ? 2 : 0));
        string fmt = d == 0 ? "0" : $"F{d}";
        string suffix = _percentStats.Contains(stat) ? "%" : "";
        return value.ToString(fmt) + suffix;
    }

    public static bool IsPercent(this UnimoStat stat) => _percentStats.Contains(stat);
}