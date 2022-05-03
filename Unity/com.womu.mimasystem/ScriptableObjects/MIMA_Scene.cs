using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
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
            DECAL_PROJECTOR,
            VISUAL_EFFECT
        }
        
        [Serializable]
        public class ExternalTextureMap
        {
            public string TargetName;
            public TEXTURE_TARGET_TYPE TargetType = TEXTURE_TARGET_TYPE.MATERIAL;
            public Material targetMat;
            public string targetLightName;
            public string targetProjectorName;
            public string targetEffectName;
            public string[] textureNames = {"_MainTex" };
            public Vector2 offset = Vector2.zero;
            public float scale = 1.0f;
            public string sourceName;
        }

        public List<ExternalTextureMap> textureMaps;

        [Serializable]
        public class EffectObject
        {
            public string Name;
            public GameObject Prefab;
            internal MIMA_Effect _effect;
            public MIMA_Effect Effect
            {
                get { return _effect;  }
            }
        }
      

        public List<EffectObject> effects;

        public List<string> cameraPositions;

        public ReflectionProbeRefreshMode reflectionProbeRefreshMode;

        public Vector3 RadicalCharacterStartingPos = Vector3.zero;

        private void OnValidate()
        {
            for (int i = 0; i < textureMaps.Count; i++)
            {
                if (textureMaps[i].TargetName.IndexOf(" ") != -1) Debug.LogError("ERROR - Cannot have spaces in Texture Target Name");
            }
        }

        // internal representation for scene
        [HideInInspector] public string _scenePath;
        [HideInInspector] public string _sceneGUID;
        [HideInInspector] public string _sceneName;
    }
}