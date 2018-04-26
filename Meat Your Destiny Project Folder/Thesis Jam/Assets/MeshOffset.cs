using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshOffset : MonoBehaviour {

    public Vector2 scrollVelocity;

    private Renderer _renderer;

    public bool useUnscaledtime = true;
	// Use this for initialization
	void Start () {
        _renderer = GetComponent<Renderer>();
		
	}
	
	// Update is called once per frame
	void Update () {
        if (useUnscaledtime)
        {
            _renderer.sharedMaterial.mainTextureOffset += scrollVelocity * Time.unscaledDeltaTime;
        }
        else
        {
            _renderer.sharedMaterial.mainTextureOffset += scrollVelocity * Time.deltaTime;
        }

		
	}
}
