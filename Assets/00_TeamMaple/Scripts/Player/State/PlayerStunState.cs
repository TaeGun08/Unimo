using System.Collections;
using UnityEngine;

public class PlayerStunState : PlayerState
{
    public override void StateEnter()
    {
        // TODO : 업데이트문으로 스턴 시간을 만들거나 또는 코루틴으로 제어해주거나 양자택일
        StartCoroutine(StunCoroutine(0.5f));
    }
    
    private IEnumerator StunCoroutine(float duration)
    {
        // TODO : 스턴 로직
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
