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
    private Quaternion lockedRotation;

    /// <summary>
    /// 지정한 시간 동안 스턴 적용 (Hit 상태 무시하고 항상 갱신)
    /// </summary>
    public void ApplyStun(float duration)
    {
        Debug.Log($"[StunRestrictor] 스턴 적용: {duration}s");

        lockedPos = transform.position;
        lockedRotation = transform.rotation;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(StunCoroutine(duration));
    }

    private IEnumerator StunCoroutine(float duration)
    {
        isStunned = true;
        Debug.Log("[StunRestrictor] 스턴 시작");
        yield return new WaitForSeconds(duration);
        isStunned = false;
        Debug.Log("[StunRestrictor] 스턴 종료");
        currentRoutine = null;
    }

    /// <summary>
    /// 외부 피격 시 스턴 일시 해제 + 자동 복구
    /// </summary>
    public void TemporarilyDisable(float delay)
    {
        if (!isStunned) return;

        Debug.Log($"[StunRestrictor] 스턴 일시 해제: {delay}s");

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        isStunned = false;
        currentRoutine = StartCoroutine(ResumeAfter(delay));
    }

    private IEnumerator ResumeAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        isStunned = true;
        Debug.Log("[StunRestrictor] 스턴 복귀됨");
    }

    private void LateUpdate()
    {
        if (isStunned)
        {
            transform.position = lockedPos;
            transform.rotation = lockedRotation;
        }
    }

    /// 외부에서 강제로 스턴 다시 설정 가능하도록 공개 래퍼
    public void ResumeAfterDelay(float delay)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(ResumeAfter(delay));
    }
}
