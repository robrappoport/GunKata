using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class TwoDGameManager : MonoBehaviour {
    const int maxPlayers = 2;
    public TwoDGunBehavior[] players;
    List<TwoDCharacterController> playerProf = new List<TwoDCharacterController>(maxPlayers);
    // Use this for initialization
    void Start () {
        //InputManager.OnDeviceDetached += OnDeviceDetached;
        players [0].playerNum = 1;
		players [1].playerNum = 2;
	}
	
	// Update is called once per frame
	//void Update () {
 //       var inputDevice = InputManager.ActiveDevice;

 //       if (JoinButtonWasPressedOnDevice(inputDevice))
 //       {
 //           if (ThereIsNoPlayerUsingDevice(inputDevice))
 //           {
 //               CreatePlayer(inputDevice);
 //           }
 //       }
 //   }

 //   bool JoinButtonWasPressedOnDevice(InputDevice inputDevice)
 //   {
 //       return inputDevice.Action1.WasPressed || inputDevice.Action2.WasPressed || inputDevice.Action3.WasPressed || inputDevice.Action4.WasPressed;
 //   }

 //   TwoDCharacterController FindPlayerUsingDevice(InputDevice inputDevice)
 //   {
 //       var playerCount = playerProf.Count;
 //       for (var i = 0; i < playerCount; i++)
 //       {
 //           var player = playerProf[i];
 //           if (player.Device == inputDevice)
 //           {
 //               return player;
 //           }
 //       }

 //       return null;
 //   }

 //   bool ThereIsNoPlayerUsingDevice(InputDevice inputDevice)
 //   {
 //       return FindPlayerUsingDevice(inputDevice) == null;
 //   }


 //   void OnDeviceDetached(InputDevice inputDevice)
 //   {
 //       var player = FindPlayerUsingDevice(inputDevice);
 //       if (player != null)
 //       {
 //           RemovePlayer(player);
 //       }
 //   }

 //   Player CreatePlayer(InputDevice inputDevice)
 //   {
 //       if (playerProf.Count < maxPlayers)
 //       {
 //           // Pop a position off the list. We'll add it back if the player is removed.

 //           player.Device = inputDevice;
 //           playerProf.Add(player);

 //           return player;
 //       }

 //       return null;
 //   }

 //   void RemovePlayer(TwoDCharacterController player)
 //   {
 //       playerProf.Remove(player);
 //       player.Device = null;

 //   }
}
