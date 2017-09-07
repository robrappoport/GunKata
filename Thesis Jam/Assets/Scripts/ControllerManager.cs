using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;


public class ControllerManager : MonoBehaviour {
    public static ControllerManager instance;
 
    private InputDevice myController;

    private void Awake() {
        instance = this;
        myController = InputManager.ActiveDevice;




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

    void Update()
    {
        myController = InputManager.ActiveDevice;
    }


    public Vector3 OnRotate() {
        return new Vector3(myController.RightStickX, -myController.RightStickY, 0);
        
    }

    public Vector3 OnMove() {
        return new Vector3(myController.LeftStickX, 0, myController.LeftStickY);
    }

    public bool OnDodge() {
        return myController.Action2.WasReleased;
    }

    public bool onLock() {
		return myController.LeftBumper.IsPressed;
    }
    public bool Unlock()
    {
		return myController.LeftBumper.WasReleased;
    }

    public bool OnSprint() {
        return myController.Action2.WasPressed;
    }

    public bool onShoot() {
        return (myController.Action4.WasReleased);
    }

    public bool onMelee()
    {
        return (myController.Action3.WasReleased);
    }

    public bool OnMenu() {
        return (myController.Command);
    }

    // FOR LATER return (Input.GetButton(aimButton) || Input.GetAxis(aimTrigger) > 0.2f)
}

