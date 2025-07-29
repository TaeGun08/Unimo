using System.Collections;
using UnityEngine;

public class PlayerStunState : PlayerState
{
    private Vector3 pushDir;
    private Vector3 moveDir;
    
    public override void StateEnter()
    {
        // StatCalculator�� �ֽ� ���� ȸ���� �� ���
        float stunRecovery = LocalPlayer.PlayerStatHolder.StunRecovery.Value;
        StartCoroutine(StunCoroutine(stunRecovery, LocalPlayer.CombatEvent));
    }

    private IEnumerator StunCoroutine(float duration, CombatEvent e)
    {
        LocalPlayer.IsInvincible = true;
        
        moveDir = (PlayerController.transform.position - e.KnockbackDir).normalized;

        VisualController.StartHitBlink(duration);
        VisualController.SetHitFX(e.Position, duration);
        yield return null;
        StartCoroutine(PushMoveCoroutine(duration, e));
        
        PlayerController.EgineAnim.SetTrigger("stun");
        PlayerController.EgineAnim.SetBool("isstun", true);
        PlayerController.UnimoAnim.SetBool("isstun", true);
        
        yield return new WaitForSeconds(0.8f * duration);
        yield return new WaitForSeconds(0.2f * duration);
        
        PlayerController.ChangeState(IPlayerState.EState.Idle);
    }

    // private IEnumerator StunCoroutine(float duration)
    // {
    //     Debug.Log("PlayerStunState !");
    //
    //     PlayerController.EgineAnim.SetTrigger("stun");
    //     PlayerController.EgineAnim.SetBool("isstun", true);
    //     PlayerController.UnimoAnim.SetBool("isstun", true);
    //
    //     yield return new WaitForSeconds(duration);
    //
    //     PlayerController.ChangeState(IPlayerState.EState.Idle);
    // }

    protected override void StateUpdate()
    {
        PlayerController.transform.position += moveDir * Time.deltaTime + pushDir * 1.5f * Time.deltaTime;
        if (mapSetter.IsInMap(PlayerController.transform.position)) return;
        PlayerController.transform.position = mapSetter.FindNearestPoint(PlayerController.transform.position);
    }

    public override void StateExit()
    {
        PlayerController.EgineAnim.SetBool("isstun", false);
        PlayerController.UnimoAnim.SetBool("isstun", false);
        LocalPlayer.IsInvincible = false;
    }
    
    protected IEnumerator PushMoveCoroutine(float duration, CombatEvent e)
    {
        Vector3 dir = PlayerController.transform.position - e.KnockbackDir;
        dir.y = 0f;
        dir.Normalize();
        Vector2 dirin = new Vector2(dir.x, dir.z);
        duration = Mathf.Min(duration, 1f);
        float lapsetime = 0f;
        while (lapsetime < duration)
        {
            lapsetime += Time.deltaTime;
            float ratio = Mathf.Cos(0.5f * Mathf.PI * lapsetime / duration);
            pushMove(ratio * dirin);
            yield return null;
        }
    }
    
    protected void pushMove(Vector2 direction)
    {
        if (direction.magnitude < 0.01f) 
        {
            pushDir = Vector3.zero;
            return; 
        }
        pushDir = new Vector3(direction.x, 0, direction.y);
    }

    // // �˹� �ӽ� �ڵ�
    // private void ApplyKnockback()
    // {
    //     if (StageLoader.IsBonusStageByIndex(Base_Mng.Data.data.CurrentStage)) return;
    //
    //     float knockbackDistance = 0.5f;
    //
    //     Rigidbody rb = PlayerController.GetComponent<Rigidbody>();
    //     if (rb != null)
    //     {
    //         Vector3 newPosition = rb.position + pushDir * knockbackDistance + new Vector3(0f, 1f, 0f);
    //         rb.MovePosition(newPosition);
    //     }
    // }
}