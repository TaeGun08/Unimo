public class ClampedFloat : ClampedValue<float>
{
    public ClampedFloat(float value, float min, float max) : base(value, min, max) { }

    protected override float AddValues(float a, float b) => a + b;
    protected override float SubtractValues(float a, float b) => a - b;
}