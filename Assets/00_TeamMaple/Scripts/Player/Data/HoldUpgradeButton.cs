using UnityEngine;
using UnityEngine.EventSystems;

public class HoldUpgradeButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum UpgradeType { Unimo, Engine }

    [SerializeField] private UpgradeType upgradeType;       // 유니모 or 엔진 선택
    [SerializeField] private UnimoStatUI targetUI;          // 연결할 UI 클래스
    [SerializeField] private float repeatInterval = 0.2f;   // 반복 호출 간격

    public void OnPointerDown(PointerEventData eventData)
    {
        InvokeRepeating(nameof(PerformUpgrade), 0f, repeatInterval);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        CancelInvoke(nameof(PerformUpgrade));
    }

    private void PerformUpgrade()
    {
        switch (upgradeType)
        {
            case UpgradeType.Unimo:
                targetUI.UpgradeUnimoAndStatUI();
                break;
            case UpgradeType.Engine:
                targetUI.UpgradeEngineAndStatUI();
                break;
        }
    }
}