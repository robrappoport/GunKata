using UnityEngine;
using System.Collections;
using InControl;

[RequireComponent(typeof(CharacterController))]
public class testMoveScript : MonoBehaviour {
    private InputDevice testController;
	// Use this for initialization
	void Awake () {
      
	}
	
	// Update is called once per frame
	void Update () {
        var testController = InputManager.ActiveDevice;
        transform.position += new Vector3(testController.LeftStickX, 0, testController.LeftStickY);
	}
}
