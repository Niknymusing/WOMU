using System;
using System.Collections.Generic;
using UnityEngine;

namespace MIMA
{
    [UnityEngine.CreateAssetMenu(fileName = "MIMAScene", menuName = "MIMA/New MIMA Scene", order = 0)]
    public class MIMA_Scene : UnityEngine.ScriptableObject
    {
        [Serializable]
        public class ExternalTextureMap
        {
            public Material targetMat;
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