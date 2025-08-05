using System.Collections;
using UnityEngine;

/// <summary>
/// 일정 시간 동안 이동/조작을 제한하는 간단한 스턴 제어 컴포넌트
/// ex) 낙뢰 기믹에서 사용
/// </summary>
public class StunRestrictor : MonoBehaviour
{
    private bool isStunned = false;
    public bool IsStunned => isStunned;

    private Coroutine currentRoutine;
    private Vector3 lockedPos;

    /// <summary>
    /// 지정한 시간 동안 스턴 적용
    /// </summary>
    public void ApplyStun(float duration)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        lockedPos = transform.position;
        currentRoutine = StartCoroutine(StunCoroutine(duration));
    }

    private IEnumerator StunCoroutine(float duration)
    {
        isStunned = true;
        yield return new WaitForSeconds(duration);
        isStunned = false;
        currentRoutine = null;
    }

    private void LateUpdate()
    {
        if (isStunned)
        {
            transform.position = lockedPos;
        }
    }
}