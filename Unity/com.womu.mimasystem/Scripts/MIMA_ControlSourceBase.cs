﻿using System;
using UnityEngine;

namespace MIMA
{
    public class MIMA_ControlSourceBase : MonoBehaviour
    {
        public Action<string> LaunchSceneCommand;
        public Action<MIMA_Scene.ExternalTextureMap> TextureMapChanged;

        public Action<Camera, float> SetCameraRandomMotion;
        public Action<Camera, float> SetCameraOrbitSensitivity;
        public Action<Camera, bool> SetCameraOrbitEnabled;
        public Action<Camera,Transform,float> GotoCameraPositionOverTime;
        public Action<Transform, Vector3, float> MoveTransformByOverTime;
        public Action<Transform, Vector3, float> RotateTransformByOverTime;
        
        public virtual void UpdateUI()
        {
        }
    }
}