using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;

namespace MIMA
{
    public class MIMA_UI : MIMA_ControlSourceBase
    {
        /*
        public UIDocument mainUI;
        public VisualTreeAsset textureMapVisual;
        public VisualTreeAsset cameraControlVisual;

        private DropdownField field_sceneSelect;
        private Button button_sceneLoad;
        private Label label_currentScene;
        private VisualElement textureMapContainer;
        private VisualElement cameraControlContainer;
        private Label label_log;
        // private string lastLog = "";
        // private int duplcateLogCount = 0;

        public CameraUIController cameraUIController;
        private List<TextureMapUIController> textureUIControllers = new List<TextureMapUIController>();
        
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

        public void Start()
        {
            var root = mainUI.rootVisualElement;
            field_sceneSelect = root.Q<DropdownField>("AvailableScenes");
            button_sceneLoad = root.Q<Button>("LoadSceneButton");
            label_currentScene = root.Q<Label>("CurrentSceneLabel");
            textureMapContainer = root.Q<VisualElement>("TextureMapping");
            cameraControlContainer = root.Q<VisualElement>("CameraControls");
            
            
            
            label_log = root.Q<Label>("LogContainer");

            var logQueue = new List<string>();

            Application.logMessageReceived += (condition, trace, type) =>
            {
                logQueue.Add(condition);
                while (logQueue.Count > 32)
                {
                    logQueue.RemoveAt(0);
                }
                
                label_log.text = String.Join("\n", logQueue);
                
                // if (lastLog != condition)
                // {
                //     
                //     duplcateLogCount = 0;
                // }
                // else
                // {
                //     duplcateLogCount++;
                //     label_log.text = label_log.text.Remove(0, label_log.text.IndexOf("\n"));
                //     label_log.text = $"({duplcateLogCount}) {lastLog} \n" + label_log.text;
                // }
                // lastLog = condition;
                
            };

            externalTextureSource.sourcesChanged += () =>
            {
                var newSources = externalTextureSource.GetExternalSources();
                foreach (var t in textureUIControllers)
                {
                    t.UpdateAvailableSources(newSources);
                }
            };

            button_sceneLoad.clicked += () =>
            {
                if (field_sceneSelect.value != String.Empty)
                {
                    if (LaunchSceneCommand != null) LaunchSceneCommand.Invoke(field_sceneSelect.value);
                }
            };
            
            UpdateUI();
        }

        public override void UpdateUI()
        {
            if (MIMA_System.Instance == null)
            {
                Debug.LogError("ERROR - MIMA system not found");
                return;
            }
            
            var s = MIMA_System.Instance.currentScene;
            if (s == null)
            {
                label_currentScene.text = "Current Scene : none";
                // clear texture maps
                for (int i = 0; i < textureMapContainer.childCount; i++)
                {
                    textureMapContainer.RemoveAt(i);
                }

                textureUIControllers.Clear();
            }
            else
            {
                label_currentScene.text = $"Current Scene : {s.name}";
                
                // clear texture maps
                for (int i = 0; i < textureMapContainer.childCount; i++)
                {
                    textureMapContainer.RemoveAt(i);
                }

                foreach (var map in s.textureMaps)
                {
                    var visual = textureMapVisual.Instantiate();
                    var controller = new TextureMapUIController(visual, map);
                    textureUIControllers.Add(controller);
                    textureMapContainer.Add(visual);
                    controller.UpdateAvailableSources(externalTextureSource.GetExternalSources());

                    controller.SettingsChanged += c =>
                    {
                        if (TextureMapChanged != null) TextureMapChanged.Invoke(c.map);
                        // try to get preview texture
                        c.SetPreviewTexture(externalTextureSource.GetTextureForSource(c.map.sourceName));
                    };
                }
                
                // clear camera control
                for (int i = 0; i < cameraControlContainer.childCount; i++)
                {
                    cameraControlContainer.RemoveAt(i);
                }

                var cameraVisual = cameraControlVisual.Instantiate();
                cameraUIController = new CameraUIController(cameraVisual, s);
                
                cameraControlContainer.Add(cameraVisual);

                cameraUIController.TakeCameraOverTime += (camera1, f) =>
                {
                    GotoCameraPositionOverTime.Invoke(Camera.main, camera1, f);
                };

                cameraUIController.SetCameraOrbitSpeed += f => SetCameraOrbitSensitivity.Invoke(Camera.main, f);
                cameraUIController.SetCameraWobble += f => SetCameraRandomMotion.Invoke(Camera.main, f);
                cameraUIController.SetCameraOrbitEnabled += e => SetCameraOrbitEnabled.Invoke(Camera.main, e);
                


            }

            field_sceneSelect.choices = MIMA_System.Instance.sceneCollection.scenes.ConvertAll(s => s.name);

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                mainUI.rootVisualElement.visible = !mainUI.rootVisualElement.visible;
            }
        }

        private void FixedUpdate()
        {
            if (MIMA_System.Instance != null)
            {
                foreach (var controller in textureUIControllers)
                {
                    controller.UpdateAvailableSources(externalTextureSource.GetExternalSources());
                }
            }
        }


        public class TextureMapUIController
        {
           
            public Action<TextureMapUIController> SettingsChanged;
            
            public MIMA_Scene.ExternalTextureMap map;
            public VisualElement root;
            private DropdownField dropdown_sources;
            private Label label_name;
            private Label label_debug;
            private Slider slider_scale;
            private Slider slider_offsetX;
            private Slider slider_offsetY;
            private Image visual_previewTex;

            public TextureMapUIController(VisualElement r, MIMA_Scene.ExternalTextureMap m)
            {
                map = m;
                root = r;

                dropdown_sources = r.Q<DropdownField>("ExternalSource");
                label_name = r.Q<Label>("MaterialNameLabel");
                slider_scale = r.Q<Slider>("ScaleSlider");
                slider_offsetX = r.Q<Slider>("OffsetXSlider");
                slider_offsetY = r.Q<Slider>("OffsetYSlider");
                visual_previewTex = r.Q<Image>("PreviewTexture");
                label_debug = r.Q<Label>("DebugValues");


                label_name.text = m.TargetName;

                dropdown_sources.RegisterValueChangedCallback((evt => UpdateSettings()));
                slider_scale.RegisterValueChangedCallback((evt => UpdateSettings()));
                slider_offsetX.RegisterValueChangedCallback((evt => UpdateSettings()));
                slider_offsetY.RegisterValueChangedCallback((evt => UpdateSettings()));

            }

            public void UpdateAvailableSources(List<string> sources)
            {
                dropdown_sources.choices = sources;
            }

            public void SetPreviewTexture(Texture tex)
            {
                visual_previewTex.image = tex;
            }

            private void UpdateSettings()
            {
                Debug.Log($"Setting changed on {map.TargetName}");
                map.offset = new Vector2(slider_offsetX.value, slider_offsetY.value);
                map.scale = 1.0f / slider_scale.value;
                map.sourceName = dropdown_sources.value;
                label_debug.text = $"Scale: {map.scale}, X:{map.offset.x}, Y:{map.offset.y}";
                
                if (SettingsChanged != null) SettingsChanged.Invoke(this);
            }
 

        }

        public class CameraUIController
        {
            public Action<Transform, float> TakeCameraOverTime;
            public Action<float> SetCameraWobble;
            public Action<float> SetCameraOrbitSpeed;
            public Action<bool> SetCameraOrbitEnabled;
            
            public VisualElement root;
            private DropdownField dropdown_camera_list;
            
            private Slider slider_transitionTime;
            private Slider slider_wobble;
            private Slider slider_orbit;
            private Toggle toggle_orbit;

            private Button button_take;
            private Button button_set;

            public CameraUIController(VisualElement r, MIMA_Scene s)
            {
                dropdown_camera_list = r.Q<DropdownField>("CameraSelect");
                slider_transitionTime = r.Q<Slider>("TransitionTimeSlider");
                slider_wobble = r.Q<Slider>("CameraWobble");
                slider_orbit = r.Q<Slider>("OrbitSlider");
                button_take = r.Q<Button>("ButtonTake");
                button_set = r.Q<Button>("ButtonSet");
                toggle_orbit = r.Q<Toggle>("OrbitCameraToggle");

                dropdown_camera_list.choices = s.cameraPositions;

                button_take.clicked += () =>
                {

                    var c = GameObject.Find(dropdown_camera_list.value).transform;
                    if (c != null)
                    {
                        Debug.Log($"Going to camera {c.name}");
                        if (TakeCameraOverTime != null) TakeCameraOverTime.Invoke(c, slider_transitionTime.value);    
                    }
                };

                button_set.clicked += () =>
                {
                    var c = GameObject.Find(dropdown_camera_list.value).transform;
                    if (c != null)
                    {
                        Debug.Log($"Updating camera {c.name}");
                        c.transform.SetPositionAndRotation(Camera.main.transform.position, Camera.main.transform.rotation);
                    }
                };

                slider_wobble.RegisterValueChangedCallback(evt =>
                {
                    if (SetCameraWobble != null)
                    {
                        SetCameraWobble.Invoke(slider_wobble.value);
                    }
                });

                slider_orbit.RegisterValueChangedCallback(evt =>
                {
                    if (SetCameraOrbitSpeed != null)
                    {
                        SetCameraOrbitSpeed.Invoke(slider_orbit.highValue - slider_orbit.value);
                    }
                });

                toggle_orbit.RegisterValueChangedCallback(evt =>
                {
                    if (SetCameraOrbitEnabled != null)
                    {
                        SetCameraOrbitEnabled.Invoke(toggle_orbit.value);
                    }
                });
            }
            
            
        }
        */
    }
    
}