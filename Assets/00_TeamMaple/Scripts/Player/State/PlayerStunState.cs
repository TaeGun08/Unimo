using System.Collections;
using UnityEngine;

public class PlayerStunState : PlayerState
{
    public override void StateEnter()
    {
        ApplyKnockback(LocalPlayer.CombatEvent);
        
        // StatCalculator�� �ֽ� ���� ȸ���� �� ���
        float stunRecovery = LocalPlayer.PlayerStatHolder.StunRecovery.Value;

        StartCoroutine(StunCoroutine(stunRecovery));
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
    
    // �˹� �ӽ� �ڵ�
    private void ApplyKnockback(CombatEvent e)
    {
        if (StageLoader.IsBonusStageByIndex(Base_Mng.Data.data.CurrentStage)) return;

        Vector3 knockbackDir = (LocalPlayer.transform.position - e.KnockbackDir).normalized;

        if (knockbackDir == Vector3.zero)
            knockbackDir = Vector3.back;

        float knockbackDistance = 0.5f;

        Rigidbody rb = LocalPlayer.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 newPosition = rb.position + knockbackDir * knockbackDistance;
            rb.MovePosition(newPosition);
        }
    }
}
