using UnityEngine;

public class PlayerHitState : PlayerState
{
    public override void StateEnter()
    {
        // TODO : 회피 확률에 따라 스턴 상태로 넘어갈 건지 만들어 줘야 함
        Debug.Log("PlayerHitState !");
        float hit = Random.Range(0f, 100f);
        if (hit <= 70f)
        {
            PlayerController.ChangeState(IPlayerState.EState.Stun);
        }
        else
        {
            PlayerController.ChangeState(IPlayerState.EState.Idle);
        }
    }

    protected override void StateUpdate()
    {
    }

    public override void StateExit()
    {
    }
}
