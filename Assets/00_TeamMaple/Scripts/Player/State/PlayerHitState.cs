using UnityEngine;

public class PlayerHitState : PlayerState
{
    public override void StateEnter()
    {
        // TODO : ȸ�� Ȯ���� ���� ���� ���·� �Ѿ ���� ����� ��� ��
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
