using System;

public abstract class ClampedValue<T> where T : IComparable<T>
{
    public T Value { get; protected set; }
    protected T min;
    protected T max;

    public event Action<T> OnValueChanged;

    protected ClampedValue(T value, T min, T max)
    {
        this.min = min;
        this.max = max;
        Value = Clamp(value);
    }

    public void Add(T amount)
    {
        Value = Clamp(AddValues(Value, amount));
        OnValueChanged?.Invoke(Value);
    }

    public void Subtract(T amount)
    {
        Value = Clamp(SubtractValues(Value, amount));
        OnValueChanged?.Invoke(Value);
    }

    private T Clamp(T val)
    {
        if (val.CompareTo(max) > 0) return max;
        if (val.CompareTo(min) < 0) return min;
        return val;
    }

    protected abstract T AddValues(T a, T b);
    protected abstract T SubtractValues(T a, T b);
}