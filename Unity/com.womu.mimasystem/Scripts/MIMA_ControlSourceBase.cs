using System;
using UnityEngine;

namespace MIMA
{
    public class MIMA_ControlSourceBase : MonoBehaviour
    {
        public Action<string> LaunchSceneCommand;
        public Action<MIMA_Scene.ExternalTextureMap> TextureMapChanged;

        public Action<float> SetCameraRandomMotion;
        public Action<int,float> GotoCameraPositionOverTime;

        public virtual void UpdateUI()
        {
        }
    }
}