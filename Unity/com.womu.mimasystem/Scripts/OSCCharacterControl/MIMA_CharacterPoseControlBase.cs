using System;
using UnityEngine;

namespace MIMA
{
    public class MIMA_CharacterPoseControlBase : MIMA_Effect
    {
        [Header("The pose 'channel' we'll be listening to")]
        public string clientID = "none";
        
        [Header("Scale up/down character model")]
        public float PosScale = 1.0f;
        
        public enum CONTROL_METHOD
        {
            MEDIAPIPE_LANDMARK_POSITION,
            RADICAL_BONE_ROTATION
        }

        public CONTROL_METHOD controlMethod = CONTROL_METHOD.MEDIAPIPE_LANDMARK_POSITION;

        public virtual void SetPosePosition(int index, Vector3 pos)
        {
            throw new NotImplementedException();
        }

        public virtual void SetJointRotation(string joint, Quaternion rot)
        {
            throw new NotImplementedException();
        }
        
    }
}