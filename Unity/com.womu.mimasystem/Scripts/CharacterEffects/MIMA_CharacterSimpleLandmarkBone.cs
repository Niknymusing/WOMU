using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MIMA_CharacterSimpleLandmarkBone : MonoBehaviour
{
    public Transform tStart;
    public Transform tEnd;

    public GameObject boneObj;
    public float XYScale = 1.0f;

    // Update is called once per frame
    void Update()
    {
        transform.position = tStart.position;
        transform.LookAt(tEnd.position);

        float scaleAmt = Vector3.Distance(tStart.position, tEnd.position);
        transform.localScale = new Vector3(XYScale, XYScale, scaleAmt);
    }
}
