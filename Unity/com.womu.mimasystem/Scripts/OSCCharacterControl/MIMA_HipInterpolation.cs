using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MIMA_HipInterpolation : MonoBehaviour
{
    public Transform LeftHip;
    public Transform RightHip;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // position mid-way between
        transform.position = Vector3.Lerp(LeftHip.position, RightHip.position, 0.5f);
        // calculate forward vector
        transform.LookAt(transform.position + (Vector3.Cross(LeftHip.position - RightHip.position, Vector3.up).normalized * 10.0f));

    }
}
