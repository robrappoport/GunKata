using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class respawnExplosion : MonoBehaviour
{
    public float sphereRad = 10f;
    // Use this for initialization
    void Start()
    {
        Vector3 center = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        float radius = sphereRad;
        bulletExplosion(center, radius);
    }

    void bulletExplosion(Vector3 center, float radius)
    {
        var hitColliders = Physics.OverlapSphere(center, radius);
       

        for (var i = 0; i < hitColliders.Length; i++)
        {
            Debug.Log("something was hit" + hitColliders[i].name);
            if (hitColliders[i].tag == "CannonBall"){
                Debug.Log("i hit something" + hitColliders[i].name);
                Destroy(hitColliders[i].gameObject);
            }
        }
    }
}
