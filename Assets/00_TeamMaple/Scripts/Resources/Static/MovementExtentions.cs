using UnityEngine;

public static class MovementExtentions
{
    /// <summary>
    /// ����� �Է�(Horizontal, Vertical)�� ���� �̵�
    /// </summary>
    /// <returns>X,Z ������ ����ȭ�� �̵� ���� ����</returns>
    public static void GetInputDirectionVector3(this Transform transform)
    {
        Vector3 axis = Vector3.zero;

        axis.x = Input.GetAxis("Horizontal");
        axis.z = Input.GetAxis("Vertical");
        
        transform.Translate(axis * Time.deltaTime, Space.World);
    }

    /// <summary>
    /// �̵��� ���� ���� �Ǵ� �ʱⰪ���� ���
    /// </summary>
    /// <returns>�⺻ �̵� ����(Vector3.zero)</returns>
    public static Vector3 GetDefaultMovementVector3()
    {
        return Vector3.zero;
    }
    
    /// <summary>
    /// �̵� ������ ����Ͽ� �̵�
    /// </summary>
    /// <returns>�̵� ������ ��Ÿ���� Vector3</returns>
    public static void CalculateMovementVector3(this Transform transform, Vector3 dir)
    {
        // TODO: �Է�, ����, �ӵ� ���� ������� ��� ���� �߰� ����
        
        transform.Translate(dir * Time.deltaTime, Space.World);
    }
    
    /// <summary>
    /// �Է� ���� X, Z ���� ���� ���͸� ���� ȸ�� ������ �� �������� �̵��ϴ� �Լ�
    /// </summary>
    /// <param name="inputAxis"></param>
    /// <param name="smoothTime"></param>
    public static void ApplyMovementAndRotation(this Transform transform, Vector2 inputVec, float speed, float turnCalmVelocity, float smoothTime)
    {
        if (inputVec.sqrMagnitude < 0.01f) return; // �Է� ������ ó�� �� ��

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
