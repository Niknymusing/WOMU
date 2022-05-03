using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using UnityEngine;

public class MIMA_CharacterPoseControlMediaPipe : MonoBehaviour
{
    // GHUM3D Landmarks
    public Transform Nose;
    public Transform LeftEye;
    public Transform RightEye;
    public Transform LeftShoulder;
    public Transform RightShoulder;
    public Transform LeftElbow;
    public Transform RightElbow;
    public Transform LeftWrist;
    public Transform RightWrist;
    public Transform LeftPinky;
    public Transform RightPinky;
    public Transform LeftIndex;
    public Transform RightIndex;
    public Transform LeftThumb;
    public Transform RightThumb;
    public Transform LeftHip;
    public Transform RightHip;
    public Transform LeftKnee;
    public Transform RightKnee;
    public Transform LeftAnkle;
    public Transform RightAnkle;
    public Transform LeftHeel;
    public Transform RightHeel;
    public Transform LeftFootIndex;
    public Transform RightFootIndex;

    public float PosScale = 1.0f;

    

    public bool debug = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLandmarkPosition(int index, Vector3 pos)
    {
        pos *= PosScale;

        if (debug) Debug.Log($"Setting landmark {index} to {pos}");
        
        switch (index)
        {
            case 0:
                Nose.localPosition = pos;
                break;
            case 1:
                
                break;
            case 2:
                LeftEye.localPosition = pos;
                break;
            case 3:
                
                break;
            case 4:
            
                break;
            case 5:
                RightEye.localPosition = pos;
                break;
            case 6:
            
                break;
            case 7:
            
                break;
            case 8:
            
                break;
            case 9:
            
                break;
            case 10:
            
                break;
            case 11:
                LeftShoulder.localPosition = pos;
                break;
            case 12:
                RightShoulder.localPosition = pos;
                break;
            case 13:
                LeftElbow.localPosition = pos;
                break;
            case 14:
                RightElbow.localPosition = pos;
                break;
            case 15:
                LeftWrist.localPosition = pos;
                break;
            case 16:
                RightWrist.localPosition = pos;
                break;
            case 17:
                LeftPinky.localPosition = pos;
                break;
            case 18:
                RightPinky.localPosition = pos;
                break;
            case 19:
                LeftIndex.localPosition = pos;
                break;
            case 20:
                RightIndex.localPosition = pos;
                break;
            case 21:
                LeftThumb.localPosition = pos;
                break;
            case 22:
                RightThumb.localPosition = pos;
                break;
            case 23:
                LeftHip.localPosition = pos;
                break;
            case 24:
                RightHip.localPosition = pos;
                break;
            case 25:
                LeftKnee.localPosition = pos;
                break;
            case 26:
                RightKnee.localPosition = pos;
                break;
            case 27:
                LeftAnkle.localPosition = pos;
                break;
            case 28:
                RightAnkle.localPosition = pos;
                break;
            case 29:
                LeftHeel.localPosition = pos;
                break;
            case 30:
                RightHeel.localPosition = pos;
                break;
            case 31:
                LeftFootIndex.localPosition = pos;
                break;
            case 32:
                RightFootIndex.localPosition = pos;
                break;
                
        }
        
        
    }
}
