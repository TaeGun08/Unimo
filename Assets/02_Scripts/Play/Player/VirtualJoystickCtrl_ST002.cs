using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualJoystickCtrl_ST002 : VirtualJoystickCtrl
{
    private void Start()
    {
        virtualStick = FindAnyObjectByType<VitualStickImager>();
        virtualStick.setStickImgSizes(2f * controlWidth, controlWidth);
        virtualStick.gameObject.SetActive(false);

        PlaySystemRefStorage.playProcessController.SubscribeGameoverAction(stopControl);
        PlaySystemRefStorage.playProcessController.SubscribePauseAction(stopControl);
        PlaySystemRefStorage.playProcessController.SubscribeResumeAction(resumeControl);
    }

    void Update()
    {
        if (isStopControl) { return; }
        if (Input.touchCount == 0)
        {
            if (virtualStick.gameObject.activeSelf)
            {
                virtualStick.gameObject.SetActive(false);
            }
            Dir = Vector2.zero;
            return;
        }
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Ended && touch.phase == TouchPhase.Canceled)
        {
            virtualStick.gameObject.SetActive(false);
        }
        else if (touch.phase == TouchPhase.Began)
        {
            virtualStick.GetComponent<RectTransform>().position= touch.position;
            stickCenterPos = touch.position;
            virtualStick.gameObject.SetActive(true);
        }
        else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
        {
            Vector2 dir = convertToDirection(touch.position);
            dir.y = 0;
            Dir = dir;
            virtualStick.setStickPos(Dir);
        }
    }
    
    private Vector2 convertToDirection(Vector2 inputPos)
    {
        Vector2 diff = inputPos - stickCenterPos;
        diff /= controlWidth;
        diff.x = Mathf.Clamp(diff.x, -1f, 1f);
        diff.y = Mathf.Clamp(diff.y, -1f, 1f);

        return diff;
    }
}