using UnityEngine;

public class PlayerMoveState : PlayerState
{
    public override void StateEnter()
    {
    }

    protected override void StateUpdate()
    {
        // TODO : 이동로직
        // PlayerController.transform.position += moveSpeed * Time.deltaTime * moveDir + pushSpeed * Time.deltaTime * pushDir;
        // moveAudio.volume = moveSoundMax * Mathf.Clamp01(moveDir.magnitude);
        // moveAudio.volume = Sound_Manager.instance._audioSources[1].volume;
        //
        // if (mapSetter.IsInMap(playerTransform.position) == false)
        // {
        //     playerTransform.position = mapSetter.FindNearestPoint(playerTransform.position);
        // }
        // auraCtrl.transform.position = playerTransform.position + new Vector3(0f, auraOffset, 0f);
        // if (pushDir.magnitude < 0.01f)
        // {
        //     changeRotation(moveDir);
        // }
    }

    public override void StateExit()
    {
    }
}
