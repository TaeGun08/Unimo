using System.Collections;
using UnityEngine;

public class PlayerStunState : PlayerState
{
    public override void StateEnter()
    {
        // TODO : ������Ʈ������ ���� �ð��� ����ų� �Ǵ� �ڷ�ƾ���� �������ְų� ��������
        //StartCoroutine(StartCoroutine());
    }
    
    private IEnumerator StunCoroutine(float duration, Vector3 hitPos)
    {
        // TODO : ���� ����
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
