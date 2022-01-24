using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MIMA_JointVelocityTracker : MonoBehaviour
{
    public Action<string, float> VelocityFrame;
    
    public string JointOSCName = "TestJoint";
    
    public List<Vector3> velocityHistory;
    
    private Vector3 lastPosition = Vector3.zero;

    public float Velocity = 0.0f;

    public float MaxVelocity = 1.0f;

    public float updateFPS = 30.0f;
    
    public float NormalisedVelocity
    {
        get { return (Velocity / MaxVelocity); }
    }

    private void Start()
    {
        lastPosition = transform.position;
        StartCoroutine(UpdateRoutine());
    }

    IEnumerator UpdateRoutine()
    {
        while (true)
        {
            if (VelocityFrame != null) VelocityFrame.Invoke(JointOSCName, NormalisedVelocity);

            yield return new WaitForSeconds(1.0f / updateFPS);
        }
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var vel = lastPosition - transform.position;
        velocityHistory.Add(vel);
        if (velocityHistory.Count > 5)
        {
            velocityHistory.RemoveAt(0);
        }

        float mag = 0.0f;
        foreach (var v in velocityHistory)
        {
            mag += v.magnitude;
        }

        mag /= (float) velocityHistory.Count;
        
        Velocity = mag;
    }
}
