public class ClampedInt : ClampedValue<int>
{
    public ClampedInt(int value, int min, int max) : base(value, min, max) { }

    protected override int AddValues(int a, int b) => a + b;
    protected override int SubtractValues(int a, int b) => a - b;
}