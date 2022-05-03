using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

        public MIMA_CharacterReceiverRadical radicalCharacter;

        public List<MIMA_CharacterPoseControlMediaPipe> dancerControllers =
            new List<MIMA_CharacterPoseControlMediaPipe>();

        public Volume customPostProcessVolume;
        private MIMA_PostProcess postProcess;

        public bool Vsync = true;

        internal MIMA_ExternalSourceManagerBase[] externalTextureSources
        {
            get
            {
                #if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                return new MIMA_ExternalSourceManagerBase[]
                    { MIMA_SpoutSourceManager.Instance, MIMA_NDISourceManager.Instance };
#else
                    return new MIMA_ExternalSourceManagerBase[]
                    { MIMA_SyphonSourceManager.Instance, MIMA_NDISourceManager.Instance };
#endif
            }
        }

        public List<string> GetExternalSources()
        {
            var l = new List<string>();
            foreach (var s in externalTextureSources)
            {
                l.AddRange(s.GetExternalSources());
            }

            return l;
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

            // grab a reference to global custom post process
            var c = customPostProcessVolume.profile.components.Where(v => v is MIMA_PostProcess);
            if (c.Count() > 0) postProcess = c.First() as MIMA_PostProcess;
            if (postProcess == null) Debug.LogError("Error - could not find post process component");
            
            
            // handle controller messages
            
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
                    Texture tex = null;
                    foreach (var s in externalTextureSources)
                    {
                        if (s.GetExternalSources().Contains(map.sourceName)) tex = s.GetTextureForSource(map.sourceName);
                    }
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

                                newDecalMat.EnableKeyword("_COLORMAP");
                                
                                // find projector and replace material, because reasons
                                var projGO = GameObject.Find(map.targetProjectorName);
                                if (projGO != null)
                                {
                                    var proj = projGO.GetComponent<DecalProjector>();
                                    proj.material = newDecalMat;
                                }

                                break;
                            case MIMA_Scene.TEXTURE_TARGET_TYPE.VISUAL_EFFECT:

                                var eff = currentScene.effects.FirstOrDefault(eff => eff.Name == map.targetEffectName);
                                if (eff != null)
                                {
                                    foreach (var texName in map.textureNames)
                                    {
                                        Debug.Log($"Updating effect  {eff.Name} with {map.targetEffectName} at {texName}");
                                        eff.Effect.vfx.SetTexture(texName, tex);
                                    }
                                }
                                else
                                {
                                    Debug.LogError($"ERROR - no effect found from {map.targetEffectName}");
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
                        if (cameraBrownianMotion == null) cameraBrownianMotion = cam.GetComponent<BrownianMotion>();
                        if (cameraBrownianMotion != null)  cameraBrownianMotion.enabled = false;
                        cam.transform.SetPositionAndRotation(target.position, target.rotation);
                        if (cameraBrownianMotion != null)  cameraBrownianMotion.enabled = true;
                    }
                    else
                    {
                        if (cameraBrownianMotion == null) cameraBrownianMotion = cam.GetComponent<BrownianMotion>();
                        if (cameraBrownianMotion != null)  cameraBrownianMotion.enabled = false;
                        Vector3 startPos = cam.transform.position;
                        Quaternion startRot = cam.transform.rotation;
                        if (cameraTween != null) cameraTween.Kill(false);
                        cameraTween = DOVirtual.Float(0.0f, 1.0f, f, v =>
                        {
                            cam.transform.position = Vector3.Lerp(startPos, target.position, v);
                            cam.transform.rotation = Quaternion.Slerp(startRot, target.rotation, v);
                        }).SetEase(Ease.InOutSine).OnComplete(() =>
                        {
                            if (cameraBrownianMotion != null)  cameraBrownianMotion.enabled = true;
                        });
                    }
                    
                };

                controller.MoveTransformByOverTime += (target, moveBy, overTime) =>
                {
                    // check if there's no brownian motion on this object
                    BrownianMotion bm;
                    bm = target.gameObject.GetComponent<BrownianMotion>();
                    if (bm != null) bm.enabled = false;
                    if (overTime > 0.0f)
                    {
                        // tween to new position
                        target.DOKill(false);
                        var endPos = target.position += moveBy;
                        target.DOMove(endPos, overTime).OnComplete(() =>
                            {
                                if (bm != null) bm.enabled = true;
                            });
                    }
                    else
                    {
                        // move directly
                        target.position += moveBy;
                        if (bm != null) bm.enabled = true;
                    }
                };
                
                controller.RotateTransformByOverTime += (target, rotateBy, overTime) =>
                {
                    // check if there's no brownian motion on this object
                    BrownianMotion bm;
                    bm = target.gameObject.GetComponent<BrownianMotion>();
                    if (bm != null) bm.enabled = false;
                    if (overTime > 0.0f)
                    {
                        // tween to new rotation
                        target.DOKill(false);
                        target.DOBlendableRotateBy(rotateBy, overTime).OnComplete(() =>
                        {
                            if (bm != null) bm.enabled = true;
                        });
                    }
                    else
                    {
                        // rotate directly
                        target.Rotate(rotateBy);
                        if (bm != null) bm.enabled = true;
                    }
                };

                controller.SetBlackoutOverTime += (finalValue, overTime) =>
                {
                    if (postProcess == null)
                    {
                        Debug.LogError("ERROR - post process not working");
                    }
                    else
                    {
                        DOVirtual.Float(postProcess.blackoutAmount.GetValue<float>(), Mathf.Clamp01(finalValue),
                            overTime, value =>
                            {
                                postProcess.blackoutAmount.value = value;
                            });
                    }
                    
                };
                
                // effects params
                controller.SetEffectParamBool += (targetEffect, paramId, paramValue) =>
                    targetEffect.vfx.SetBool(paramId, paramValue);
                controller.SetEffectParamFloat += (targetEffect, paramId, paramValue) => 
                    targetEffect.vfx.SetFloat(paramId, paramValue);
                controller.SetEffectParamInt += (targetEffect, paramId, paramValue) =>
                    targetEffect.vfx.SetInt(paramId, paramValue);
                controller.SetEffectParamUint += (targetEffect, paramId, paramValue) =>
                    targetEffect.vfx.SetUInt(paramId, paramValue);
                controller.SetEffectParamVector2 += (targetEffect, paramId, paramValue) =>
                    targetEffect.vfx.SetVector2(paramId, paramValue);
                controller.SetEffectParamVector3 += (targetEffect, paramId, paramValue) =>
                    targetEffect.vfx.SetVector3(paramId, paramValue);
                controller.SetEffectSimSpeed += (targetEffect, speed) =>
                    targetEffect.vfx.playRate = speed;
                controller.SetEffectSimSpeedOverTime += (targetEffect, speed, overTime) =>
                    DOVirtual.Float(targetEffect.vfx.playRate, speed, overTime, val => targetEffect.vfx.playRate = val);

                
                // pose params
                controller.SetDancerLandmarkPositionByClientID += (clientID, landmarkID, position) =>
                {
                    if (currentScene == null) return;
                    var d = dancerControllers.FirstOrDefault(d => d.clientID == clientID);
                    if (d == null && dancerControllers.Count > 0)
                    {
                        // if client id is not found, set the first dancer to respond to this clientID
                        d = dancerControllers[0];
                        d.clientID = clientID;
                    }
                    d.SetLandmarkPosition(landmarkID, position);
                };

                controller.SetDancerObjectClientID += (index, newClientID) =>
                {
                    dancerControllers[index].clientID = newClientID;
                };

                controller.SetDancerPositionScale += (index, scale) =>
                {
                    dancerControllers[index].PosScale = scale;
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
                // destroy effects
                foreach (var eff in currentScene.effects)
                {
                    if (eff.Effect != null) Destroy(eff.Effect.gameObject);
                }
                
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
                if (controller.gameObject.activeSelf) controller.UpdateUI();
            }
            
             
            // reposition Radical character
            if (radicalCharacter == null) radicalCharacter = FindObjectOfType<MIMA_CharacterReceiverRadical>();
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
           
            // instantiate effects
            foreach (var eff in scene.effects)
            {
                Debug.Log($"Instantiating effect {eff.Name}");
                var eGO = Instantiate(eff.Prefab, Vector3.zero, Quaternion.identity, null);
                eff._effect = eGO.GetComponent<MIMA_Effect>();
                if (eff._effect == null)
                {
                    Debug.LogError($"Error - no MIMA_Effect class found for effect prefab {eff.Name}");
                }

                if (eff._effect is MIMA_CharacterPoseControlMediaPipe)
                {
                    dancerControllers.Add(eff._effect as MIMA_CharacterPoseControlMediaPipe);
                }
            }
           

        }

        private void Update()
        {
            
        }
    }
}