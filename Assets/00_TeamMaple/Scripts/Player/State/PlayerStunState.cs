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
        
        PlayerController.EgineAnim.SetTrigger("stun");
        PlayerController.EgineAnim.SetBool("isstun", true);
        PlayerController.UnimoAnim.SetBool("isstun", true);
        
        yield return new WaitForSeconds(duration);
        
        PlayerController.ChangeState(IPlayerState.EState.Idle);
    }

    protected override void StateUpdate()
    {
    }

    public override void StateExit()
    {
        PlayerController.EgineAnim.SetBool("isstun", false);
        PlayerController.UnimoAnim.SetBool("isstun", false);
    }
    
    // 넉백 임시 코드
    private void ApplyKnockback()
    {
        Vector3 knockbackDir = (PlayerController.transform.position - LocalPlayer.Instance.LastAttackerPos).normalized;
        float knockbackDistance = 0.5f;
        
        Rigidbody rb = PlayerController.GetComponent<Rigidbody>();
        Vector3 newPosition = rb.position + knockbackDir * knockbackDistance;
        rb.MovePosition(newPosition);
    }
}
