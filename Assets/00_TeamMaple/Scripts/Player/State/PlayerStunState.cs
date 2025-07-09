using System.Collections;
using UnityEngine;

public class PlayerStunState : PlayerState
{
    public override void StateEnter()
    {
        // TODO : ������Ʈ������ ���� �ð��� ����ų� �Ǵ� �ڷ�ƾ���� �������ְų� ��������
        StartCoroutine(StunCoroutine(0.5f));
    }
    
    private IEnumerator StunCoroutine(float duration)
    {
        // TODO : ���� ����
        Debug.Log("PlayerStunState !");
        PlayerController.EqAnim.SetTrigger("stun");
        PlayerController.EqAnim.SetBool("isstun", true);
        PlayerController.UnimoAnim.SetBool("isstun", true);
        yield return new WaitForSeconds(duration);
        PlayerController.EqAnim.SetBool("isstun", false);
        PlayerController.UnimoAnim.SetBool("isstun", false);
        PlayerController.ChangeState(IPlayerState.EState.Idle);
    }

    protected override void StateUpdate()
    {
    }

    public override void StateExit()
    {
    }
}
