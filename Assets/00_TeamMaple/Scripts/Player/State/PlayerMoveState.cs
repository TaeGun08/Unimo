using System;
using UnityEngine;

public class PlayerMoveState : PlayerState
{
    private readonly float rotateThreshold = 0.05f;
    private float rotateTime = 0.3f;
    
    private float turnCalmVelocity;
    protected MapRangeSetter mapSetter;

    private void Start()
    {
        mapSetter = PlaySystemRefStorage.mapSetter;
    }

    public override void StateEnter()
    {
    }

    protected override void StateUpdate()
    {
        if (mapSetter.IsInMap(PlayerController.transform.position) == false)
        {
            PlayerController.transform.position = mapSetter.FindNearestPoint(PlayerController.transform.position);
        }
        
        UpdateMoveAndRotation(PlayerController.VirtualJoystickCtrl.Dir, 0.1f);

        if (PlayerController.VirtualJoystickCtrl.Dir.magnitude < 0.01f)
        {
            PlayerController.ChangeState(IPlayerState.EState.Idle);
        }
    }

    public override void StateExit()
    {
    }
    
    /// <summary>
    /// 이동 및 회전 처리
    /// </summary>
    /// <param name="inputAxis"></param>
    /// <param name="smoothTime"></param>
    private void UpdateMoveAndRotation(Vector2 inputAxis, float smoothTime)
    {
        if (inputAxis.sqrMagnitude < 0.01f) return; // 입력 없으면 처리 안 함

        float targetAngle = Mathf.Atan2(inputAxis.x, inputAxis.y) * Mathf.Rad2Deg;

        float angle = Mathf.SmoothDampAngle(
            PlayerController.transform.eulerAngles.y,
            targetAngle,
            ref turnCalmVelocity,
            smoothTime
        );

        PlayerController.transform.rotation = Quaternion.Euler(0f, angle, 0f);

        Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        
        float speed = LocalPlayer.Instance.UnimoData.Speed;
        PlayerController.transform.position += moveDirection.normalized * (speed * Time.deltaTime);
    }
}
