using UnityEngine;

public abstract class InGameEvent : MonoBehaviour
{
    public enum EventType
    {
        Unknown,
        Combat,
    }
    
    public IDamageAble Sender { get; set; }
    public IDamageAble Receiver { get; set; }
    public abstract EventType Type { get; }
}

public class CombatEvent : InGameEvent
{
    public int Damage { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 KnockbackDir;
    
    public override EventType Type => EventType.Combat;
}
