using System;
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
        public Action<Transform, Vector3, float> SetTransformScaleOverTime;

        public Action<float, float> SetBlackoutOverTime;

        public Action<MIMA_Effect, int, bool> SetEffectParamBool;
        public Action<MIMA_Effect, int, float> SetEffectParamFloat;
        public Action<MIMA_Effect, int, int> SetEffectParamInt;
        public Action<MIMA_Effect, int, uint> SetEffectParamUint;
        public Action<MIMA_Effect, int, Vector2> SetEffectParamVector2;
        public Action<MIMA_Effect, int, Vector3> SetEffectParamVector3;
        public Action<MIMA_Effect, float> SetEffectSimSpeed;
        public Action<MIMA_Effect, float, float> SetEffectSimSpeedOverTime;

        public Action<int, int, Vector3> SetDancerLandmarkPositionByClientID;
        public Action<int, int> SetDancerObjectClientID;
        public Action<int, float> SetDancerPositionScale;

        public virtual void UpdateUI()
        {
        }
    }
}