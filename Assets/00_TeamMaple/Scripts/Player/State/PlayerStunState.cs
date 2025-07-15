using System.Collections;
using UnityEngine;

public class PlayerStunState : PlayerState
{
    public override void StateEnter()
    {
        ApplyKnockback();
        
        // StatCalculator�� �ֽ� ���� ȸ���� �� ���
        float stunRecovery = LocalPlayer.StatCalculator.StunRecovery;

        StartCoroutine(StunCoroutine(stunRecovery));
    }
    
    private IEnumerator StunCoroutine(float duration)
    {
        Debug.Log("PlayerStunState !");
        
        PlayerController.EgineAnim.SetTrigger("stun");
        PlayerController.EgineAnim.SetBool("isstun", true);
        PlayerController.UnimoAnim.SetBool("isstun", true);

        LocalPlayer.CurMaxHp -= 10;    // ���� �ʿ���
        
        yield return new WaitForSeconds(duration);

        if (LocalPlayer.StatCalculator.Hp <= 0)
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
    
    // �˹� �ӽ� �ڵ�
    private void ApplyKnockback()
    {
        Vector3 knockbackDir = (PlayerController.transform.position - LocalPlayer.LastAttackerPos).normalized;
        float knockbackDistance = 0.5f;
        
        Rigidbody rb = PlayerController.GetComponent<Rigidbody>();
        Vector3 newPosition = rb.position + knockbackDir * knockbackDistance;
        rb.MovePosition(newPosition);
    }
}
