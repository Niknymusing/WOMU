using System;
using System.Collections;
using System.Collections.Generic;
using Klak.Spout;
using Klak.Syphon;

using MIMA;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Klak.Motion;
using Unity.Mathematics;
using UnityEngine.Rendering;

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
        public Vector3 defaultCameraMotionPosition;
        public Vector3 defaultCameraMotionRotation;
        public float defaultCameraMotionFrequency = 0.05f;
        public float defaultCameraPositionLerpTime = 0.1f;
        public float defaultCameraRotationLerpTime = 0.1f;
        

        private BrownianMotion cameraBrownianMotion;
        private float lastCameraBrownianAmount = 1.0f;
        private SimpleCameraController cameraOrbitMotion;
        private Tween cameraTween;

        public MIMA_CharacterReceiver radicalCharacter;

        public bool Vsync = true;

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
            if (Vsync)
            {
                QualitySettings.vSyncCount = 1;
            }
            
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
                    
                    // update reflection probes
                    var probes = FindObjectsOfType<ReflectionProbe>();
                    foreach (var p in probes)
                    {
                        if (p.refreshMode != ReflectionProbeRefreshMode.EveryFrame) p.RenderProbe();
                    }
                };

                controller.SetCameraOrbitEnabled += (c, e) =>
                {
                    if (e)
                    {
                        DOVirtual.Float(1.0f, 0.0f, 2.0f, v =>
                        {
                            cameraBrownianMotion.positionAmount = defaultCameraMotionPosition * v * lastCameraBrownianAmount;
                            cameraBrownianMotion.rotationAmount = defaultCameraMotionRotation * v * lastCameraBrownianAmount;
                        }).OnComplete(() =>
                        {
                            cameraBrownianMotion.enabled = false;
                            cameraOrbitMotion.enabled = true;
                        });
                    }
                    else
                    {
                        cameraOrbitMotion.enabled = false;
                        cameraBrownianMotion.enabled = true;
                        cameraBrownianMotion.positionAmount = float3.zero;
                        cameraBrownianMotion.rotationAmount = float3.zero;
                        DOVirtual.Float(0.0f, 1.0f, 2.0f, v =>
                        {
                            cameraBrownianMotion.positionAmount = defaultCameraMotionPosition * v * lastCameraBrownianAmount;
                            cameraBrownianMotion.rotationAmount = defaultCameraMotionRotation * v * lastCameraBrownianAmount;
                        }).OnComplete(() =>
                        {
                            
                            
                        });
                    }

                };

                controller.SetCameraOrbitSensitivity += (c, f) =>
                {
                    cameraOrbitMotion = c.GetComponent<SimpleCameraController>();
                    cameraOrbitMotion.positionLerpTime = f * defaultCameraPositionLerpTime;
                    cameraOrbitMotion.rotationLerpTime = f * defaultCameraRotationLerpTime;
                };
                
                controller.SetCameraRandomMotion += (c, f) =>
                {
                    if (cameraBrownianMotion == null) cameraBrownianMotion = c.GetComponent<BrownianMotion>();
                    cameraBrownianMotion.positionAmount = defaultCameraMotionPosition * f;
                    cameraBrownianMotion.rotationAmount = defaultCameraMotionRotation * f;
                    lastCameraBrownianAmount = f;
                };

                controller.GotoCameraPositionOverTime += (cam, target, f) =>
                {
                    Debug.Log($"Moving camera to {target.position} / {target.rotation} over {f} seconds");
                    if (f == 0.0f)
                    {
                        cameraBrownianMotion.enabled = false;
                        cam.transform.SetPositionAndRotation(target.position, target.rotation);
                        cameraBrownianMotion.enabled = true;
                    }
                    else
                    {
                        cameraBrownianMotion.enabled = false;
                        Vector3 startPos = cam.transform.position;
                        Quaternion startRot = cam.transform.rotation;
                        if (cameraTween != null) cameraTween.Kill(false);
                        cameraTween = DOVirtual.Float(0.0f, 1.0f, f, v =>
                        {
                            cam.transform.position = Vector3.Lerp(startPos, target.position, v);
                            cam.transform.rotation = Quaternion.Slerp(startRot, target.rotation, v);
                        }).SetEase(Ease.InOutSine).OnComplete(() =>
                        {
                            cameraBrownianMotion.enabled = true;
                        });
                    }
                    
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
            
             
            // reposition Radical character
            if (radicalCharacter == null) radicalCharacter = FindObjectOfType<MIMA_CharacterReceiver>();
            if (radicalCharacter != null)
            {
                radicalCharacter.transform.position = scene.RadicalCharacterStartingPos;
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

            // append random motion
            cameraBrownianMotion = Camera.main.gameObject.GetComponent<BrownianMotion>();
            if (cameraBrownianMotion == null) cameraBrownianMotion = Camera.main.gameObject.AddComponent<BrownianMotion>();
            
            cameraBrownianMotion.frequency = defaultCameraMotionFrequency;
            cameraBrownianMotion.positionAmount = defaultCameraMotionPosition;
            cameraBrownianMotion.rotationAmount = defaultCameraMotionRotation;
            
            // apply settings to Reflection Probes
            var probes = FindObjectsOfType<ReflectionProbe>();
            foreach (var p in probes)
            {
                Debug.Log($"Setting {p.name} to {scene.reflectionProbeRefreshMode}");
                p.refreshMode = scene.reflectionProbeRefreshMode;
                var rData = p.GetComponent<HDAdditionalReflectionData>();
                switch (scene.reflectionProbeRefreshMode)
                {
                    case ReflectionProbeRefreshMode.OnAwake:
                        rData.realtimeMode = ProbeSettings.RealtimeMode.OnEnable;
                        break;
                    case ReflectionProbeRefreshMode.EveryFrame:
                        rData.realtimeMode = ProbeSettings.RealtimeMode.EveryFrame;
                        break;
                    case ReflectionProbeRefreshMode.ViaScripting:
                        rData.realtimeMode = ProbeSettings.RealtimeMode.OnDemand;
                        break;
                }
            }
            
            // enable camera controller
            cameraOrbitMotion = Camera.main.GetComponent<SimpleCameraController>();
            if (cameraOrbitMotion == null) cameraOrbitMotion = Camera.main.gameObject.AddComponent<SimpleCameraController>();
            cameraOrbitMotion.enabled = false;
           

        }

        private void Update()
        {
            
        }
    }
}