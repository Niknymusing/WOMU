using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;


namespace MIMA
{
    [UnityEngine.CreateAssetMenu(fileName = "MIMAScene", menuName = "MIMA/New MIMA Scene", order = 0)]
    public class MIMA_Scene : UnityEngine.ScriptableObject
    {
        public enum TEXTURE_TARGET_TYPE
        {
            MATERIAL,
            LIGHT_COOKIE,
            DECAL_PROJECTOR
        }
        
        [Serializable]
        public class ExternalTextureMap
        {
            public string TargetName;
            public TEXTURE_TARGET_TYPE TargetType = TEXTURE_TARGET_TYPE.MATERIAL;
            public Material targetMat;
            public string targetLightName;
            public string targetProjectorName;
            public string[] textureNames = {"_MainTex" };
            public Vector2 offset = Vector2.zero;
            public float scale = 1.0f;
            public string sourceName;
        }

        public List<ExternalTextureMap> textureMaps;
        
        // internal representation for scene
        [HideInInspector] public string _scenePath;
        [HideInInspector] public string _sceneGUID;
        [HideInInspector] public string _sceneName;
    }
}