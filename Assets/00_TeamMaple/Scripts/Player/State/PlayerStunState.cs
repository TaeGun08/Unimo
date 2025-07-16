using System.Collections;
using UnityEngine;

public class PlayerStunState : PlayerState
{
    public override void StateEnter()
    {
        ApplyKnockback();
        
        // StatCalculator의 최신 스턴 회복률 값 사용
        float stunRecovery = LocalPlayer.PlayerStatHolder.StunRecovery.Value;

        StartCoroutine(StunCoroutine(stunRecovery));
    }
    
    private IEnumerator StunCoroutine(float duration)
    {
        Debug.Log("PlayerStunState !");
        
        PlayerController.EgineAnim.SetTrigger("stun");
        PlayerController.EgineAnim.SetBool("isstun", true);
        PlayerController.UnimoAnim.SetBool("isstun", true);

        LocalPlayer.PlayerStatHolder.Hp.Subtract(10);
        
        yield return new WaitForSeconds(duration);

        if (LocalPlayer.PlayerStatHolder.Hp.Value <= 0)
        {
            PlayerController.ChangeState(IPlayerState.EState.Dead);
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
        PlayerController.EgineAnim.SetBool("isstun", false);
        PlayerController.UnimoAnim.SetBool("isstun", false);
    }
    
    // 넉백 임시 코드
    private void ApplyKnockback()
    {
        Vector3 knockbackDir = (PlayerController.transform.position - LocalPlayer.LastAttackerPos).normalized;
        float knockbackDistance = 0.5f;
        
        Rigidbody rb = PlayerController.GetComponent<Rigidbody>();
        Vector3 newPosition = rb.position + knockbackDir * knockbackDistance;
        rb.MovePosition(newPosition);
    }
}
