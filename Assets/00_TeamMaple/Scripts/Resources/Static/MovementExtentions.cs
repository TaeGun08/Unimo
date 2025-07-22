using UnityEngine;

public static class MovementExtentions
{
    /// <summary>
    /// 사용자 입력(Horizontal, Vertical)에 따라 이동
    /// </summary>
    /// <returns>X,Z 평면상의 정규화된 이동 방향 벡터</returns>
    public static void GetInputDirectionVector3(this Transform transform)
    {
        Vector3 axis = Vector3.zero;

        axis.x = Input.GetAxis("Horizontal");
        axis.z = Input.GetAxis("Vertical");
        
        transform.Translate(axis * Time.deltaTime, Space.World);
    }

    /// <summary>
    /// 이동이 없는 상태 또는 초기값으로 사용
    /// </summary>
    /// <returns>기본 이동 벡터(Vector3.zero)</returns>
    public static Vector3 GetDefaultMovementVector3()
    {
        return Vector3.zero;
    }
    
    /// <summary>
    /// 이동 방향을 계산하여 이동
    /// </summary>
    /// <returns>이동 방향을 나타내는 Vector3</returns>
    public static void CalculateMovementVector3(this Transform transform, Vector3 dir)
    {
        // TODO: 입력, 방향, 속도 등을 기반으로 계산 로직 추가 예정
        
        transform.Translate(dir * Time.deltaTime, Space.World);
    }
    
    /// <summary>
    /// 입력 받은 X, Z 값의 방향 벡터를 구해 회전 보간을 한 방향으로 이동하는 함수
    /// </summary>
    /// <param name="inputAxis"></param>
    /// <param name="smoothTime"></param>
    public static void ApplyMovementAndRotation(this Transform transform, Vector2 inputVec, float speed, float turnCalmVelocity, float smoothTime)
    {
        if (inputVec.sqrMagnitude < 0.01f) return; // 입력 없으면 처리 안 함

        float targetAngle = Mathf.Atan2(inputVec.x, inputVec.y) * Mathf.Rad2Deg;

        float angle = Mathf.SmoothDampAngle(
            transform.eulerAngles.y,
            targetAngle,
            ref turnCalmVelocity,
            smoothTime
        );

        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        
        transform.Translate(moveDirection.normalized * (speed * Time.deltaTime), Space.World);
    }
}
