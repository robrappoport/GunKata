using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ControllerManager : MonoBehaviour {
    public static ControllerManager instance;
    public string moveHorizontalAxis;
    public string moveVerticalAxis;
    public string jumpButton;
    public string cameraHorizontalAxis;
    public string cameraVerticalAxis;
    public string fireButton;
    public string fireTrigger;
    public string aimButton;
    public string aimTrigger;
    public string sprintButton;
    public string MenuButton;
    public string BackButton;

    private void Awake() {
        instance = this;

        moveHorizontalAxis = "Horizontal";
        moveVerticalAxis = "Vertical";

        jumpButton = "Jump";

		cameraHorizontalAxis = "RightStickHorizontal";
		cameraVerticalAxis = "RightStickVertical";

        fireButton = "Fire";
        fireTrigger = "Right_Trigger";
        aimButton = "Aim";
        aimTrigger = "Left_Trigger";

        sprintButton = "Sprint";

        MenuButton = "Menu";
        BackButton = "Back";

#if UNITY_EDITOR

#endif

#if UNITY_STANDALONE
        
#endif

#if UNITY_ANDROID
        fireButton = "AndroidFakeFire";
        aimButton = "AndroidFakeAim";
        aimTrigger = "L_2_Aim";
        fireTrigger = "R_2_Fire";
        cameraHorizontalAxis = "Android_R_Stick_H";
        cameraVerticalAxis = "Android_R_Stick_V";
        sprintButton = "Android_Sprint";
        MenuButton = "Android_Menu";
        BackButton = "Android_Back";
#endif

#if UNITY_IOS
        fireButton = "AndroidFakeFire";
        aimButton = "AndroidFakeAim";
#endif
    }

    public Vector3 OnMove() {
        return new Vector3(Input.GetAxis(moveHorizontalAxis), 0, Input.GetAxis(moveVerticalAxis));
    }

    public bool OnJump() {
        return Input.GetButtonDown(jumpButton);
    }

    public bool OnAim() {
        return (Input.GetButton(aimButton) || Input.GetAxis(aimTrigger) > 0.2f);
    }

    public bool OnSprint() {
        return Input.GetButton(sprintButton);
    }

    public bool OnFire() {
        return (Input.GetButton(fireButton) || Input.GetAxis(fireTrigger) > 0.2f);
    }

    public bool OnMenu() {
        return Input.GetButtonUp(MenuButton);
    }

    public bool OnBack() {
        return Input.GetButtonUp(BackButton);
    }
}
