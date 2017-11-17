using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraMultitarget : MonoBehaviour {
	
	/// <summary>
	/// The target objects.
	/// </summary>
	public List<GameObject> targetObjects = new List<GameObject>();

	/// <summary>
	/// Used when you want to focus 2 objects in a lateral way, ideal for fighting games.
	/// it will take the two first objects in the tracking list. 
	/// you must make sure the objects are ordered so the ones you want to track are the first two.
	/// </summary>
	public bool useLateralAlignedView;

	/// <summary>
	/// Use this to rotate around when doing free orbit or with lateral view. rotate Z to get the right angle.
	/// </summary>
	public Vector3 orbitRotation;

	/// <summary>
	/// if you want to offset the focus position use this.
	/// </summary>
	public Vector3 focusOffset;

	public bool useScreenSpace;

	/// <summary>
	/// the closest the camera will be, from here the objects won't get framed.
	/// </summary>	
	public float minDistanceToTarget = 10;
	
	/// <summary>
	/// the maximum distance to focus objects. After this distance objects won't get framed.
	/// </summary>	
	public float maxDistanceToTarget = 100;
	

	/// <summary>
	/// The screen safe area to keep your objects in, it's a percent of the fov.
	/// </summary>	
	public float screenSafeArea = 200.0f;
	
	/// <summary>
	/// the easing function that will be used to interpolate positions.
	/// </summary>
	public float positionInterpolationSpeed = 5f;
	
	/// <summary>
	/// the speed it will interpolate to the look at point desired.
	/// </summary>
	public float targetInterpolationSpeed = 2f;

	/// <summary>
	/// The orthographic safe area multiplier to reduce the amount of safe area. More will result in 
	/// less space when adjusting the safe area amount.
	/// </summary>
	public float orthographicSafeAreaMulti = 4f;

	/// <summary>
	/// The additional Z rotation, this makes the camera have an angle facing the objects.
	/// </summary>
	public float additionalZRotation = 0;

	/// <summary>
	/// Allowws you to do linear interpolation, without lerp, this is constant movement towards focus / target.
	/// </summary>
	public bool useConstantFocusSpeed = false;
	/// <summary>
	/// The speed to focus.
	/// </summary>
	public float focusSpeed = 0.1f;

	public Vector3 camPosition;

	public bool keepCameraLookAt = false;
	
	/// <summary>
	/// Private variables for the script functionality.
	/// </summary>
	#region private
	private Vector3 lookAt;
	private Vector3 currentLookAt;
	private Vector3 posAt;
	private Vector3 currentPosAt;
	private Transform trtemp;
	
	private Vector3 scrMin = Vector3.zero;
	private Vector3 scrMax = Vector3.zero;
	private float extraSpeed = 1.0f;
		
	private Vector3 cameraDirection;
	private Bounds currentBounds;


	private Vector3 screenCorrection;
	private Ray correction;

	private Vector3 currentCameraDirection;

	private Vector3 randV;
	private Vector3 shakingVector;
	private float shakeAmount;
	private float shakeTime;
	private float shakeTimer;
	//less value the shake decreases slowly.
	private const float shakeFrequencyInterp = 0.05f;
	// less value the shake is more smooth.
	private const float shakeSmoothing = 0.9f;


	#endregion
	
	// Use this for initialization
	void Start () {
		camPosition = transform.position;
	
		// places the camera at the initial position, relative to the look at vector.
		posAt = camPosition;
		Bounds b = GetElementsBounds();
		
		
		//cameraDirection = b.center - posAt;
		cameraDirection = posAt - b.center;
		cameraDirection = cameraDirection.normalized;
		
		lookAt = b.center;
		currentPosAt = posAt;
		currentLookAt = lookAt;

		//orbitRotation = Quaternion.identity.eulerAngles;
	}
	
	private Bounds GetElementsBounds()
	{
		Bounds bounds = new Bounds(Vector3.zero, Vector3.one);
	
		if (targetObjects.Count > 0)
		{						
			bool inited = false;
			// we get the maximum and minimum bounds of the elements we want to fit in the camera.

			foreach(GameObject tr in targetObjects)
			{				
				if (tr.GetComponent<CameraMultiTargetObjective>().enableTracking)
				{					
					if (!inited)
					{
						bounds = new Bounds (tr.transform.position, tr.GetComponent<Renderer>().bounds.size);;
						inited = true;
					}else
					{
						bounds.Encapsulate(tr.GetComponent<Renderer>().bounds);			
					}
				}				
			}
		}
		
		return bounds;		
	}


	
	void OnDrawGizmos()
	{
		
		// target debug info.
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(currentLookAt, 0.5f);
		
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(lookAt, 0.6f);
				
		Gizmos.DrawLine(currentLookAt, lookAt);

		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(currentCameraDirection + lookAt, lookAt + (currentCameraDirection * 40));
		
		// bounds debug info.
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(currentBounds.center, currentBounds.size);

		Gizmos.color = Color.cyan;
		Gizmos.DrawSphere(currentBounds.max, 0.5f);
		Gizmos.DrawSphere(currentBounds.min, 0.5f);

		Gizmos.DrawSphere(screenCorrection, 1f);
		Gizmos.color = Color.magenta;
		Gizmos.DrawRay(correction);
	}
	
	// Update is called once per frame
	void Update () {	
		if (!useScreenSpace)
		{
			CalculateWithCentralBoundingBoxPosition();
			if (!keepCameraLookAt)
			{
				Camera.main.transform.LookAt(currentLookAt + focusOffset + shakingVector);
			}

		}else
		{
			CalculateWithRelativeScreenSpacePositions();
			if (!keepCameraLookAt)
			{
				Camera.main.transform.LookAt(focusOffset + shakingVector);
			}
		}
		Camera.main.transform.rotation = Camera.main.transform.rotation * Quaternion.Euler(0, 0,additionalZRotation);
	}

	public void CalculateWithRelativeScreenSpacePositions()
	{

		CalculateWithCentralBoundingBoxPosition();
		Camera.main.transform.LookAt(currentLookAt);

		float maxY = -1000000;
		float minY = 1000000;

		if (targetObjects.Count > 0)
		{						
			bool inited = false;
			// we get the maximum and minimum bounds of the elements we want to fit in the camera.			
			foreach(GameObject tr in targetObjects)
			{				
				if (tr.GetComponent<CameraMultiTargetObjective>().enableTracking)
				{
					Vector3 screenPosition = Camera.main.WorldToScreenPoint(tr.transform.position);
					maxY = Mathf.Max(screenPosition.y , maxY);
					minY = Mathf.Min(screenPosition.y , minY);
				}
			}
		}			

		Vector3 newCenter = new Vector3 (Screen.width/2, (Screen.height/2) -  ((maxY - minY)/2), 0);
		correction = Camera.main.ScreenPointToRay(newCenter);
		screenCorrection = correction.GetPoint((Camera.main.transform.position - currentLookAt).magnitude);

		focusOffset = Vector3.MoveTowards(focusOffset, screenCorrection, 0.5f);

	}

	//temp variables.
	private Vector3 pos1;
	private Vector3 pos2;
	private Vector3 perpendicular;

	public void CalculateWithCentralBoundingBoxPosition()
	{
		currentBounds = GetElementsBounds();

		// we stablish our lookAt point to the center of that bounds.
		lookAt = currentBounds.center;

		float boundsSizeSphere = currentBounds.size.magnitude / 2;
		Camera c = GetComponent<Camera>();
		float hFov = Mathf.Atan(Mathf.Tan(c.fieldOfView * Mathf.Deg2Rad / 2f) * c.aspect) * Mathf.Rad2Deg;
		float fov = Mathf.Min (c.fieldOfView, hFov) - ((screenSafeArea / 100) * c.fieldOfView);		
		float distance = boundsSizeSphere / (Mathf.Sin(fov * Mathf.Deg2Rad/2));

		// we get the distance at which we need to position our camera.
		distance = Mathf.Max( minDistanceToTarget, Mathf.Min(distance, maxDistanceToTarget));

		// we interpolate to the new desired positions.	
		currentCameraDirection = Vector3.left;
		if (!useLateralAlignedView || targetObjects.Count < 2)
		{
			currentCameraDirection = Quaternion.Euler(orbitRotation) * cameraDirection;
		}else
		{
			pos1 = targetObjects[0].transform.position;
			pos2 = targetObjects[1].transform.position;

			//Vector3 middlePos = (pos2 - pos1)/2;

			perpendicular = Vector3.Cross((pos2 - pos1), Vector3.up).normalized;

			currentCameraDirection = Quaternion.Euler(orbitRotation) * (perpendicular);
		}

		if (useConstantFocusSpeed)
		{
			currentLookAt = Vector3.MoveTowards(currentLookAt, lookAt, targetInterpolationSpeed * Time.deltaTime);
			posAt = Vector3.MoveTowards(posAt,currentLookAt +( currentCameraDirection * distance), positionInterpolationSpeed * Time.deltaTime);
		}else
		{
			currentLookAt = Vector3.Lerp(currentLookAt, lookAt, targetInterpolationSpeed * Time.deltaTime);
			posAt = Vector3.Lerp(posAt,currentLookAt +( currentCameraDirection * distance), positionInterpolationSpeed * Time.deltaTime);			
		}

		if (c.orthographic)
		{
			c.orthographicSize = boundsSizeSphere + (screenSafeArea/orthographicSafeAreaMulti);
		}

		shakeTimer -= Time.deltaTime;
		randV.x = Random.value - 0.5f; 
		randV.y = Random.value - 0.5f;
		randV.z = Random.value - 0.5f;

		if (shakeTimer < 0)
		{
			shakeAmount = Mathf.Lerp(shakeAmount, 0, shakeFrequencyInterp);
		}

		shakingVector = Vector3.Lerp(shakingVector, randV, shakeSmoothing) * shakeAmount;
		c.transform.position = Vector3.Lerp(c.transform.position , posAt, 0.25f) + shakingVector;
	
	}


	public void Shake(float amount, float time)
	{
		shakeTimer = time;
		shakeAmount = amount;
	}
}
