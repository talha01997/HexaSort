using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCam : MonoBehaviour
{
    void Start()

    {
        //transform.Rotate( 180,0,0 );
    }

    void Update()
    {
        Vector3 v = Camera.main.transform.position - transform.position;

        v.x = v.z = 0.0f;

        transform.LookAt(Camera.main.transform.position - v);

        transform.rotation = (Camera.main.transform.rotation);
    }
}
