using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;

namespace MIMA
{
    public class MIMA_UI : MIMA_ControlSourceBase
    {
        public UIDocument mainUI;
        public VisualTreeAsset textureMapVisual;

        private DropdownField field_sceneSelect;
        private Button button_sceneLoad;
        private Label label_currentScene;
        private VisualElement textureMapContainer;
        private Label label_log;
        // private string lastLog = "";
        // private int duplcateLogCount = 0;

        private List<TextureMapUIController> textureUIControllers = new List<TextureMapUIController>();

        public void Start()
        {
            var root = mainUI.rootVisualElement;
            field_sceneSelect = root.Q<DropdownField>("AvailableScenes");
            button_sceneLoad = root.Q<Button>("LoadSceneButton");
            label_currentScene = root.Q<Label>("CurrentSceneLabel");
            textureMapContainer = root.Q<VisualElement>("TextureMapping");
            label_log = root.Q<Label>("LogContainer");

            Application.logMessageReceived += (condition, trace, type) =>
            {
                label_log.text = condition + "\n" + label_log.text;
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

            MIMA_SpoutManager.Instance.sourcesChanged += () =>
            {
                var newSources = MIMA_SpoutManager.GetExternalSources();
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
                    controller.UpdateAvailableSources(MIMA_SpoutManager.GetExternalSources());

                    controller.SettingsChanged += c =>
                    {
                        if (TextureMapChanged != null) TextureMapChanged.Invoke(c.map);
                        // try to get preview texture
                        c.SetPreviewTexture(MIMA_SpoutManager.Instance.GetTextureForSource(c.map.sourceName));
                    };
                }
                
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
                    controller.UpdateAvailableSources(MIMA_SpoutManager.GetExternalSources());
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

                label_name.text = m.targetMat.name;

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
                Debug.Log($"Setting changed on {map.targetMat.name}");
                map.offset = new Vector2(slider_offsetX.value, slider_offsetY.value);
                map.scale = 1.0f / slider_scale.value;
                map.sourceName = dropdown_sources.value;
                label_debug.text = $"Scale: {map.scale}, X:{map.offset.x}, Y:{map.offset.y}";
                
                if (SettingsChanged != null) SettingsChanged.Invoke(this);
            }
 

        }
    }
}