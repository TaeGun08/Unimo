using System.Collections;
using UnityEngine;

public class PlayerStunState : PlayerState
{
    public override void StateEnter()
    {
        ApplyKnockback();
        
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
    private void ApplyKnockback()
    {
        if (StageLoader.IsBonusStageByIndex(Base_Mng.Data.data.CurrentStage)) return;
        
        Vector3 knockbackDir = (PlayerController.transform.position - LocalPlayer.LastAttackerPos).normalized;
        float knockbackDistance = 0.5f;
        
        if (knockbackDir == Vector3.zero)
            knockbackDir = Vector3.back;
        
        Rigidbody rb = PlayerController.GetComponent<Rigidbody>();
        Vector3 newPosition = rb.position + knockbackDir * knockbackDistance;
        // rb.interpolation = RigidbodyInterpolation.Interpolate;
        // rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        
        Debug.Log($"PlayerPos: {PlayerController.transform.position}, AttackerPos: {LocalPlayer.LastAttackerPos}");
        Debug.Log($"KnockbackDir: {knockbackDir}, FinalPos: {newPosition}");
        
        rb.MovePosition(newPosition);
    }
}
