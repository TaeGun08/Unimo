using UnityEngine;

public interface IDamageAble
{
    public GameObject GameObject { get; }

    public void TakeDamage(CombatEvent e);
}
