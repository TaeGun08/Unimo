using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class VirtualJoystickCtrl_ST001 : VirtualJoystickCtrl
{
    private void Start()
    {
        //mover = GetComponent<PlayerMover_ST001>();
        virtualStick = FindAnyObjectByType<VitualStickImager>();
        virtualStick.setStickImgSizes(2f * controlRadius, 2f * controlRadius);
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
            Dir = convertToDirection(touch.position);
            virtualStick.setStickPos(Dir);
        }
    }
    private Vector2 convertToDirection(Vector2 inputPos)
    {
        Vector2 diff = inputPos - stickCenterPos;
        diff /= controlRadius;
        float radius = Mathf.Clamp01(diff.magnitude);
        float angle = diff.AngleInXZ();
        Vector2 dir = radius * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        return dir;
    }
}
