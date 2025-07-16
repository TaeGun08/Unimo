using System;

public class ClampedValue<T> where T : IComparable<T>
{
    private T value;
    public T GetValue() => value;
    
    private T min;
    private T max;
    
    public event Action<T> OnValueChanged;
    public event Action<T> OnClampedToMin;
    public event Action<T> OnClampedToMax;

    public ClampedValue(T value, T min, T max)
    {
        this.min = min;
        this.max = max;
        SetValue(value);
    }

    public void SetValue(T newValue)
    {
        if (newValue.CompareTo(min) < 0)
        {
            value = min;
            OnClampedToMin?.Invoke(value);
        }
        else if (newValue.CompareTo(max) > 0)
        {
            value = max;
            OnClampedToMax?.Invoke(value);
        }
        else
        {
            value = newValue;
        }

        OnValueChanged?.Invoke(value);
    }

}