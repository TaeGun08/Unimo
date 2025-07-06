using UnityEngine;

public interface IPlayerState
{
    public enum EState
    {
        Idle,
        Move,
        Skill,
        Hit,
        Stun,
        Dead,
    }
    
    public EState State { get;}
}
