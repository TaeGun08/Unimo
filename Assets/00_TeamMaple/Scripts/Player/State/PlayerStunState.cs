using System.Collections;
using UnityEngine;

public class PlayerStunState : PlayerState
{
    public override void StateEnter()
    {
        // TODO : 업데이트문으로 스턴 시간을 만들거나 또는 코루틴으로 제어해주거나 양자택일
        //StartCoroutine(StartCoroutine());
    }
    
    private IEnumerator StunCoroutine(float duration, Vector3 hitPos)
    {
        // TODO : 스턴 로직
        // playerMover.IsStop = true;
        // auraController.Shrink();
        // isInvincible = true;
        // visualCtrl.StartHitBlink(duration);
        // visualCtrl.SetHitFX(hitPos, duration);
        // yield return null;
        // playerMover.StunPush(duration, hitPos);
        // visualCtrl.SetMovingAnim(false);
        // visualCtrl.SetStunAnim(true);
        // yield return new WaitForSeconds(0.8f * duration);
        // playerMover.IsStop = false;
        // yield return new WaitForSeconds(0.2f * duration);
        // auraController.Resume();
        // visualCtrl.SetStunAnim(false);
        // yield return new WaitForSeconds(invincibleTime);
        // isInvincible = false;
        yield break;
    }

    protected override void StateUpdate()
    {
    }

    public override void StateExit()
    {
    }
}
