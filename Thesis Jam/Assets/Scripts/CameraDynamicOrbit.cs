using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDynamicOrbit : MonoBehaviour {
    public float cameraMinDistance = 0.1f;
    public float cameraMaxDistance = 3;
    public float cameraAimMaxDistance = 1;
    public float cameraRotationSpeed = 120;
    public float camDampSpeed = 20;
    public float verticalMinAngle = -20;
    public float verticalMaxAngle = 80;

    private float _cameraDistance;
    public float _cameraHorizontalOffset;
	public float _cameraVerticalOffset;
    private Transform _camTrans;

    private float cameraOriginalMaxDistance;
    private float _defaultVerticalOffset;

    private RaycastHit _hit;

    //private ControllerAxis controllerAxis = new ControllerAxis();

    private void Awake() {
        _camTrans = this.transform.Find("PlayerCamera");
    }

    // Use this for initialization
    void Start () {
		this.transform.localPosition = new Vector3(_cameraHorizontalOffset, _cameraVerticalOffset, 0);
        cameraOriginalMaxDistance = cameraMaxDistance;
        _cameraVerticalOffset = 1.6f;
        _defaultVerticalOffset = _cameraVerticalOffset;
    }
	
	// Update is called once per frame
	void Update () {
        //If in Game menu panel is on, camera cannot rotate;
      

        CameraRotate();
        CameraAiming();
        DynamicCameraDistance();
    }

    private void FixedUpdate() {
        _cameraDistance = cameraMaxDistance;

        if (Physics.Raycast(this.transform.position, -this.transform.forward, out _hit)) {
            if(_hit.collider.gameObject.layer == 8) {
                if (_hit.distance < cameraMaxDistance) {
                    _cameraDistance = _hit.distance - 0.5f;
                }
            }
        }
    }

    private void CameraRotate() {
        
            this.transform.Rotate(Input.GetAxis(ControllerManager.instance.cameraVerticalAxis) * cameraRotationSpeed * Time.deltaTime, Input.GetAxis(ControllerManager.instance.cameraHorizontalAxis) * cameraRotationSpeed * Time.deltaTime, 0);
        

        this.transform.localEulerAngles = new Vector3(AngleClamp(this.transform.localEulerAngles.x, verticalMinAngle, verticalMaxAngle), this.transform.localEulerAngles.y, 0);
    }

    private void CameraAiming() {
        if (ControllerManager.instance.OnAim()) {
            _cameraHorizontalOffset = 0.3f;
            _cameraVerticalOffset = 1.7f;
            cameraMaxDistance = cameraAimMaxDistance;
        } else {
            _cameraHorizontalOffset = 0;
            _cameraVerticalOffset = _defaultVerticalOffset;
            cameraMaxDistance = cameraOriginalMaxDistance;
        }
    }

    private void DynamicCameraDistance() {
        _cameraDistance = Mathf.Clamp(_cameraDistance, cameraMinDistance, cameraMaxDistance);
        _camTrans.localPosition = Vector3.Lerp(_camTrans.localPosition, new Vector3(_cameraHorizontalOffset, 0, -_cameraDistance), camDampSpeed * Time.deltaTime);
        this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, new Vector3(0, _cameraVerticalOffset, 0), camDampSpeed * Time.deltaTime);
    }

    private float AngleClamp(float _angle, float _min, float _max) {
        if (_angle >= 0 && _angle <= 90) {
            _angle = Mathf.Clamp(_angle, 0, _max);
        } else if (_angle >= 270 && _angle < 360) {
            _angle = Mathf.Clamp(_angle, 360 + _min, 360);
        }
        return _angle;
    }
}
