using UnityEngine;

public class PlayerHitState : PlayerState
{
    public override void StateEnter()
    {
        // TODO : ȸ�� Ȯ���� ���� ���� ���·� �Ѿ ���� ����� ��� ��
        Debug.Log("PlayerHitState !");
        PlayerController.ChangeState(IPlayerState.EState.Idle);
    }

    protected override void StateUpdate()
    {
    }

    public override void StateExit()
    {
    }
}
