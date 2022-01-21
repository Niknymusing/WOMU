﻿using System;
using System.Collections;
using System.Collections.Generic;
using Klak.Spout;
using Klak.Syphon;
using MIMA;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;

namespace MIMA
{
    public class MIMA_System : MonoBehaviour
    {
        public static MIMA_System Instance
        {
            get { return _instance;  }
        }

        private static MIMA_System _instance;
        
        public MIMA_SceneCollection sceneCollection;

        public MIMA_Scene currentScene = null;

        public List<MIMA_ControlSourceBase> controlSources = new List<MIMA_ControlSourceBase>();

        public Material defaultDecalMaterial;

        public MIMA_ExternalSourceManagerBase externalTextureSource
        {
            get
            {
                #if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                    return MIMA_SpoutSourceManager.Instance;
                #else
                    return MIMA_SyphonSourceManager.Instance;
                #endif
            }
        }

        private void Awake()
        {
            if (_instance != null)
            {
                Debug.LogError("ERROR - there should only ever be one instance of this class");
            }

            _instance = this;
        }
      

        void Start()
        {
            foreach (var controller in controlSources)
            {
                controller.LaunchSceneCommand += sceneName =>
                {
                    var sceneObj = sceneCollection.scenes.FindLast(s => s.name == sceneName);
                    if (sceneObj != null)
                    {
                        LoadScene(sceneObj);    
                    }
                    else
                    {
                        Debug.LogError($"ERROR - scene object not found for {sceneName}");
                    }
                    
                };
                controller.TextureMapChanged += map =>
                {
                    // get material, assign texture
                    var tex = externalTextureSource.GetTextureForSource(map.sourceName);
                    if (tex == null) Debug.LogError($"ERROR - no texture found for source {map.sourceName}");
                    else
                    {
                        switch (map.TargetType)
                        {
                            case MIMA_Scene.TEXTURE_TARGET_TYPE.MATERIAL:
                                foreach (var texName in map.textureNames)
                                {
                                    Debug.Log($"Updating material {map.targetMat.name} with {map.sourceName} at {texName}");
                                    map.targetMat.SetTexture(texName, tex);
                                    map.targetMat.SetTextureOffset(texName, map.offset);
                                    map.targetMat.SetTextureScale(texName, new Vector2(map.scale, map.scale));
                                }
                                break;
                            case MIMA_Scene.TEXTURE_TARGET_TYPE.LIGHT_COOKIE:
                                var targetLightGO = GameObject.Find(map.targetLightName);
                                if (targetLightGO != null)
                                {
                                    Debug.Log($"Updating cookie on light {map.targetLightName} with {map.sourceName}");
                                    var lxData = targetLightGO.GetComponent<HDAdditionalLightData>();
                                    lxData.SetCookie(tex);
                                }
                                
                                break;
                            case MIMA_Scene.TEXTURE_TARGET_TYPE.DECAL_PROJECTOR:

                                var newDecalMat = Instantiate(defaultDecalMaterial);
                                newDecalMat.name = map.TargetName + "_DecalMaterial";
                                
                                foreach (var texName in map.textureNames)
                                {
                                    Debug.Log($"Updating material {map.targetMat.name} with {map.sourceName} at {texName}");
                                    newDecalMat.SetTexture(texName, tex);
                                    newDecalMat.SetTextureOffset(texName, map.offset);
                                    newDecalMat.SetTextureScale(texName, new Vector2(map.scale, map.scale));
                                }
                                
                                // find projector and replace material, because reasons
                                var projGO = GameObject.Find(map.targetProjectorName);
                                if (projGO != null)
                                {
                                    var proj = projGO.GetComponent<DecalProjector>();
                                    proj.material = newDecalMat;
                                }

                                break;
                        }
                        
                        
                    }
                };

                controller.SetCameraRandomMotion += f =>
                {
                    // todo - camera random motion
                };

                controller.GotoCameraPositionOverTime += (i, f) =>
                {
                    // todo - camera position over time
                };
            }
        }

        void LoadScene(MIMA_Scene scene)
        {
            StartCoroutine(LoadSceneRoutine(scene));
        }

        IEnumerator LoadSceneRoutine(MIMA_Scene scene)
        {
            // TODO - fade in / out
            
            if (currentScene != null)
            {
                Debug.Log($"Unloading scene {currentScene._sceneName}");
                // unload this scene first
                var asyncUnload = SceneManager.UnloadSceneAsync(currentScene._sceneName);
                yield return new WaitUntil(() => asyncUnload.isDone);
                Debug.Log($"Unloaded scene {currentScene._sceneName}");
            }
            
            Debug.Log($"Loading scene {scene._sceneName}");
            var asyncLoad = SceneManager.LoadSceneAsync(scene._sceneName, LoadSceneMode.Additive);
            asyncLoad.allowSceneActivation = true;

            yield return new WaitUntil(()=> asyncLoad.isDone);
            
            Debug.Log($"Loaded scene {scene.name}");

            currentScene = scene;

            foreach (var controller in controlSources)
            {
                controller.UpdateUI();
            }
            
            // append syphon or spout behaviour to main camera
            Debug.Log($"Searching for main camera to append Spout/Syphon behaviour");
            while (Camera.main == null)
            {
                yield return null;
            }
            
            #if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                var sender = Camera.main.gameObject.AddComponent<SpoutSender>();
                sender.captureMethod = CaptureMethod.Camera;
                sender.sourceCamera = Camera.main;
            #else
                var sender = Camera.main.gameObject.AddComponent<SyphonServer>();
            #endif


        }

        private void Update()
        {
            
        }
    }
}