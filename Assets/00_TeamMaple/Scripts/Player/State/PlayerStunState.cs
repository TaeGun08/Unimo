using System.Collections;
using UnityEngine;

public class PlayerStunState : PlayerState
{
    public override void StateEnter()
    {
        ApplyKnockback();
        StartCoroutine(StunCoroutine(LocalPlayer.Instance.UnimoData.stunDuration(LocalPlayer.Instance.UnimoData.StunRecovery)));
    }
    
    private IEnumerator StunCoroutine(float duration)
    {
        Debug.Log("PlayerStunState !");
        
        PlayerController.EqAnim.SetTrigger("stun");
        PlayerController.EqAnim.SetBool("isstun", true);
        PlayerController.UnimoAnim.SetBool("isstun", true);
        
        yield return new WaitForSeconds(duration);
        
        PlayerController.ChangeState(IPlayerState.EState.Idle);
    }

    protected override void StateUpdate()
    {
    }

    public override void StateExit()
    {
        PlayerController.StartCoroutine(DelayedExitAnimationReset());
    }

    private IEnumerator DelayedExitAnimationReset()
    {
        yield return new WaitForSeconds(0.5f);
        
        PlayerController.EqAnim.SetBool("isstun", false);
        PlayerController.UnimoAnim.SetBool("isstun", false);
    }
    
    // �˹� �ӽ� �ڵ�
    private void ApplyKnockback()
    {
        Vector3 knockbackDir = (PlayerController.transform.position - LocalPlayer.Instance.LastAttackerPos).normalized;
        float knockbackDistance = 0.5f;
        
        Rigidbody rb = PlayerController.GetComponent<Rigidbody>();
        Vector3 newPosition = rb.position + knockbackDir * knockbackDistance;
        rb.MovePosition(newPosition);
    }
}
