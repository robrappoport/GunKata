using UnityEngine;
using System.Collections.Generic;

public class SmashCamScript : MonoBehaviour {

	public LevelFocus LevelFocus;

	public List<GameObject> Players;

	public float DepthUpdateSpeed = 5f;
	public float AngleUpdateSpeed = 7f;
	public float PositionUpdateSpeed = 5f;

	public float DepthMax = -10f;
	public float DepthMin = -22f;

	public float AngleMax = 11f;
	public float AngleMin = 3f;

	private float CameraEulerZ;
	private Vector3 CameraPos;



	// Use this for initialization
	void Start () {
		Players.Add (LevelFocus.gameObject);
	}
	
	// Update is called once per frame
	void LateUpdate () {
		CalculateCameraLocations ();
		MoveCamera ();
	}

	private void MoveCamera ()
	{
		Vector3 position = gameObject.transform.position;
		if (position != CameraPos) 
		{
			Vector3 targetPosition = Vector3.zero;
			targetPosition.x = Mathf.MoveTowards (position.x, CameraPos.x, PositionUpdateSpeed * Time.deltaTime);
			targetPosition.y = Mathf.MoveTowards (position.y, CameraPos.y, DepthUpdateSpeed * Time.deltaTime);
			targetPosition.z = Mathf.MoveTowards (position.z, CameraPos.z,PositionUpdateSpeed  * Time.deltaTime);
			gameObject.transform.position = targetPosition;
		}

		Vector3 localEulerAngles = gameObject.transform.localEulerAngles;
		if (localEulerAngles.z != CameraEulerZ) 
		{
			Vector3 targetEulerAngles = new Vector3 (localEulerAngles.x, localEulerAngles.y, CameraEulerZ);
			gameObject.transform.localEulerAngles = Vector3.MoveTowards (localEulerAngles, targetEulerAngles, AngleUpdateSpeed * Time.deltaTime);
		}
	}



	private void CalculateCameraLocations()
	{
		Vector3 averageCenter = Vector3.zero;
		Vector3 totalPositions = Vector3.zero;
		Bounds playerBounds = new Bounds ();

		for (int i = 0; i < Players.Count; i++) 
		{
			Vector3 playerPosition = Players [i].transform.position;

			if (!LevelFocus.FocusBounds.Contains (playerPosition)) 
			{
				float playerX = Mathf.Clamp(playerPosition.x, LevelFocus.FocusBounds.min.x, LevelFocus.FocusBounds.max.x);
				float playerY = Mathf.Clamp(playerPosition.y, LevelFocus.FocusBounds.min.y, LevelFocus.FocusBounds.max.y);
				float playerZ = Mathf.Clamp(playerPosition.z, LevelFocus.FocusBounds.min.z, LevelFocus.FocusBounds.max.z);
				playerPosition = new Vector3 (playerX, playerY, playerZ);
			}

			totalPositions += playerPosition;
			playerBounds.Encapsulate (playerPosition);
		}

		averageCenter = (totalPositions / Players.Count);

		float extents = (playerBounds.extents.x + playerBounds.extents.y);
		float lerpPercent = Mathf.InverseLerp (0, (LevelFocus.HalfXBounds + LevelFocus.HalfYBounds) / 2, extents);

		float depth = Mathf.Lerp (DepthMax, DepthMin, lerpPercent);
		float angle = Mathf.Lerp (AngleMax, AngleMin, lerpPercent);

		CameraEulerZ = angle;
		CameraPos = new Vector3 (averageCenter.x,depth , averageCenter.z);
	}
}
