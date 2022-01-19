using System;
using System.Collections;
using System.Collections.Generic;
using MIMA;
using UnityEngine;
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
                    var tex = MIMA_SpoutManager.Instance.GetTextureForSource(map.sourceName);
                    if (tex == null) Debug.LogError($"ERROR - no texture found for source {map.sourceName}");
                    else
                    {
                        foreach (var texName in map.textureNames)
                        {
                            Debug.Log($"Updating material {map.targetMat.name} with {map.sourceName} at {texName}");
                            map.targetMat.SetTexture(texName, tex);
                            map.targetMat.SetTextureOffset(texName, map.offset);
                            map.targetMat.SetTextureScale(texName, new Vector2(map.scale, map.scale));
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

        }

        private void Update()
        {
            
        }
    }
}