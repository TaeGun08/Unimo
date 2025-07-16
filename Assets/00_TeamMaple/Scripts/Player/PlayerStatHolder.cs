using UnityEngine;

public class PlayerStatHolder
{
    public ClampedInt Hp { get; private set; }
    public ClampedInt Def { get; private set; }
    public ClampedFloat Speed { get; private set; }
    public ClampedInt BloomRange { get; private set; }
    public ClampedFloat BloomSpeed { get; private set; }
    public ClampedFloat FlowerRate { get; private set; }
    public ClampedFloat RareFlowerRate { get; private set; }
    public ClampedFloat Dodge { get; private set; }
    public ClampedFloat StunRecovery { get; private set; }
    public ClampedFloat HpRecovery { get; private set; }
    public ClampedFloat FlowerDropSpeed { get; private set; }
    public ClampedFloat FlowerDropAmount { get; private set; }

    private StatCalculator statCalculator;

    public PlayerStatHolder(StatCalculator calculator)
    {
        statCalculator = calculator;

        // 기본값, 최소값, 최대값으로 ClampedValue 초기화
        // 최소, 최대값 수정 필요
        Hp                = new ClampedInt(statCalculator.Hp, 0, statCalculator.Hp);
        Def               = new ClampedInt(statCalculator.Def, 0, 9999);
        Speed             = new ClampedFloat(statCalculator.Speed * 10f, 0f, 100f);
        BloomRange        = new ClampedInt(statCalculator.BloomRange, 0, 300);
        BloomSpeed        = new ClampedFloat(statCalculator.BloomSpeed, 0f, 100f);
        FlowerRate        = new ClampedFloat(statCalculator.FlowerRate, 0f, 100f);
        RareFlowerRate    = new ClampedFloat(statCalculator.RareFlowerRate, 0f, 1f);
        Dodge             = new ClampedFloat(statCalculator.Dodge, 0f, 1f);
        StunRecovery      = new ClampedFloat(statCalculator.StunRecovery, 0f, 1f);
        HpRecovery        = new ClampedFloat(statCalculator.HpRecovery, 0f, 100f);
        FlowerDropSpeed   = new ClampedFloat(statCalculator.FlowerDropSpeed, 0f, 100f);
        FlowerDropAmount  = new ClampedFloat(statCalculator.FlowerDropAmount, 0f, 100f);
    }
}
