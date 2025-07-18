using UnityEngine;

public abstract class VirtualJoystickCtrl : MonoBehaviour
{
    protected VitualStickImager virtualStick;
    protected Vector2 stickCenterPos;
    protected float controlRadius = 140;
    protected float controlWidth = 130;
    protected bool isStopControl = false;
    
    public Vector2 Dir { get; set; }
    
    protected void OnDisable()
    {
        if (virtualStick == null) { return; }
        if (virtualStick.gameObject.activeSelf == true) { virtualStick.gameObject.SetActive(false); }
    }
    
    protected void stopControl()
    {  
        isStopControl = true;
        //mover.SetDirection(Vector2.zero); 
        virtualStick.gameObject.SetActive(false);
    }
    protected void resumeControl()
    {
        isStopControl = false;
    }
}
