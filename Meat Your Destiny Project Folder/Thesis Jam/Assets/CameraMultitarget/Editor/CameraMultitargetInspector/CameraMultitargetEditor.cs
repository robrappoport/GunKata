using UnityEngine;
using UnityEditor;
using System.Collections;
/// <summary>
/// Camera multitarget editor, 
/// Custom inspector for the camera multitarget component.
/// </summary>
[CustomEditor( typeof(CameraMultitarget))]
public class CameraMultitargetEditor : Editor {
	
	
	private bool positionParams = true;
	private bool movementParams = true;
	private bool rotationParams = true;
	private bool extraParams = true;
	private bool focusOffset = true;
	
	/// <summary>
	/// Raises the inspector GUI Event.
	/// </summary>
    public override void OnInspectorGUI() {
		CameraMultitarget myTarget = (CameraMultitarget) target;

		myTarget.useScreenSpace = EditorGUILayout.Toggle("Use Scr.Space", myTarget.useScreenSpace);

		// we edit the camera movement parameters.
       	movementParams = EditorGUILayout.Foldout(movementParams, "Movement Settings");
       	if(movementParams) {
			myTarget.minDistanceToTarget = EditorGUILayout.FloatField("Minimum Distance", myTarget.minDistanceToTarget);
			myTarget.maxDistanceToTarget = EditorGUILayout.FloatField("Maximum Distance", myTarget.maxDistanceToTarget);

       	}

		rotationParams = EditorGUILayout.Foldout(rotationParams, "Movement Settings");
		if (rotationParams)
		{
			myTarget.orbitRotation.x = EditorGUILayout.FloatField("Orbit X", myTarget.orbitRotation.x);
			myTarget.orbitRotation.y = EditorGUILayout.FloatField("Orbit Y", myTarget.orbitRotation.y);
			myTarget.orbitRotation.z = EditorGUILayout.FloatField("Orbit Z", myTarget.orbitRotation.z);
		}

		focusOffset = EditorGUILayout.Foldout(focusOffset, "Focus Offset");
		if (focusOffset)
		{
			myTarget.focusOffset.x = EditorGUILayout.FloatField("Focus X", myTarget.focusOffset.x);
			myTarget.focusOffset.y = EditorGUILayout.FloatField("Focus Y", myTarget.focusOffset.y);
			myTarget.focusOffset.z = EditorGUILayout.FloatField("Focus Z", myTarget.focusOffset.z);
		}
		
		// we edit the advanced values for the interpolation and safe area.
		extraParams = EditorGUILayout.Foldout(extraParams, "Advanced Settup");
       	if(extraParams) {
			myTarget.targetInterpolationSpeed = EditorGUILayout.FloatField("Target Interpolation Speed", myTarget.targetInterpolationSpeed);						
			myTarget.positionInterpolationSpeed = EditorGUILayout.FloatField("Position Interpolation Speed", myTarget.positionInterpolationSpeed);						
			EditorGUILayout.PrefixLabel("Screen Safe Area");			
			myTarget.screenSafeArea = EditorGUILayout.Slider(myTarget.screenSafeArea, -100, 100);
			myTarget.additionalZRotation = EditorGUILayout.FloatField("Additional Z Rotation", myTarget.additionalZRotation);

			myTarget.useConstantFocusSpeed = EditorGUILayout.Toggle("Use Constant Focus Speed", myTarget.useConstantFocusSpeed);
			//myTarget.focusSpeed = EditorGUILayout.FloatField("Focus Speed", myTarget.focusSpeed);
       	}

		myTarget.useLateralAlignedView = EditorGUILayout.Toggle("Use Lateral Align View", myTarget.useLateralAlignedView);
		myTarget.keepCameraLookAt = EditorGUILayout.Toggle("Keep Look At", myTarget.keepCameraLookAt);
    }
}
