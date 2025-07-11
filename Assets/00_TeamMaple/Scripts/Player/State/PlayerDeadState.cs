using UnityEngine;

public class PlayerDeadState : PlayerState
{
    public override void StateEnter()
    {
        Debug.Log("PlayerDeadState");
        OnPlayerDead();
    }

    protected override void StateUpdate()
    {
    }

    public override void StateExit()
    {
    }
    
    private void OnPlayerDead()
    {
        Debug.Log("플레이어 사망 - 게임 종료 처리");
        PlaySystemRefStorage.playProcessController.TimeUp();
    }
}
