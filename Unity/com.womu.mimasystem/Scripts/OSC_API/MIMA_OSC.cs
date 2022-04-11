using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using UnityEngine.UIElements;
using OscJack;
using UnityEngine.SceneManagement;

namespace MIMA
{
    
    public class MIMA_OSC : MIMA_ControlSourceBase
    {
        public OscServer server;
        public OscClient client;
        public int oscPortServer = 8888;
        public int oscPortClient = 8889;

        public bool debug = false;
        public bool AttachToDebugLog = true;
        
        public struct OSCMsg
        {
            public string address;
            public OscDataHandle data;

            public OSCMsg(string addr, OscDataHandle d)
            {
                address = addr;
                data = d;
            }
        }

        public struct LogMsg
        {
            public string address;
            public string msg;
            public LogMsg(string addr, string m)
            {
                address = addr;
                msg = m;
            }
        }
    
        Queue<OSCMsg> MessageQueue = new Queue<OSCMsg>();
        Queue<LogMsg> LogToOSCQueue = new Queue<LogMsg>();
        private List<Camera> CamerasInScene = new List<Camera>();

        void Start()
        {
            server = new OscServer(oscPortServer);
            server.MessageDispatcher.AddCallback(String.Empty, (address, data) =>
            {
                // add messages to a queue, because this callback doesn't happen on the Update thread
                var msg = new OSCMsg(address, data);
                MessageQueue.Enqueue(msg);
            });

            client = new OscClient("0.0.0.0", oscPortClient);
            
            // reroute all Unity error messages to OSC
            Application.logMessageReceivedThreaded += (condition, trace, type) =>
            {
                if (AttachToDebugLog)
                {
                    switch (type)
                    {
                        case LogType.Error:
                            LogToOSCQueue.Enqueue(new LogMsg("/log/error", condition));
                            break;
                        case LogType.Assert:
                            LogToOSCQueue.Enqueue(new LogMsg("/log/assert", condition));
                            break;
                        case LogType.Warning:
                            LogToOSCQueue.Enqueue(new LogMsg("/log/warning", condition));
                            break;
                        case LogType.Log:
                            LogToOSCQueue.Enqueue(new LogMsg("/log/info", condition));
                            break;
                        case LogType.Exception:
                            LogToOSCQueue.Enqueue(new LogMsg("/log/exception", condition));
                            break;
                    }
                }
                
            };
            
            // Cache all cameras when scene is loaded
            SceneManager.sceneLoaded += (scene, mode) =>
            {
                // we don't care about the Main scene
                if (!scene.name.Contains("Main"))
                {
                    CamerasInScene.Clear();
                    CamerasInScene.AddRange(GameObject.FindObjectsOfType<Camera>());
                    Debug.Log("Loaded new scene, found " + CamerasInScene.Count + " cameras");
                }
            };
        }
        
        private void Update()
        {
            // OSC API DEFINED MOSTLY HERE
            // process the message queue on the update thread, so we can actually do Unity things
            while (MessageQueue.Count > 0)
            {
                
                    var msg = MessageQueue.Dequeue();
                    if (debug) Debug.Log("Processing OSC: " + msg);
                    var parts = msg.address.Split('/');
                    
                    try
                    {
                        switch (parts[0])
                        {
                            case "scene":
                                // scene-level stuff here, like loading scenes, setting sources etc

                                switch (parts[1])
                                {
                                    case "load":
                                        var targetScene = msg.data.GetElementAsString(0);
                                        if (debug) Debug.Log($"Loading scene {targetScene}");
                                        if (LaunchSceneCommand != null) LaunchSceneCommand.Invoke(targetScene);
                                        break;
                                    
                                    case "listCameras":
                                        LogMessageOSC("/scene/numCameras", CamerasInScene.Count.ToString());
                                        for (int i=0; i < CamerasInScene.Count; i++)
                                            LogMessageOSC("/scene/cameraName", $"{i} : {CamerasInScene[i].name}");
                                        
                                        break;
                                    case "camera":

                                        var targetCamera = Camera.main;
                                        if (parts[2] != "main")
                                        {
                                            int camIndex = int.Parse(parts[2]);
                                            targetCamera = CamerasInScene[camIndex];
                                            
                                            switch (parts[3])
                                            {
                                                case "setRandomMotion":
                                                    var randomMotionAmount = msg.data.GetElementAsFloat(0);
                                                    if (debug) Debug.Log($"Setting camera random motion to {randomMotionAmount}");
                                                    if (SetCameraRandomMotion != null) SetCameraRandomMotion.Invoke(targetCamera, randomMotionAmount);
                                                break;
                                                case "take":
                                                    if (debug) Debug.Log($"Setting main camera settings from camera {camIndex}");
                                                    CopyCameraSettings(Camera.main, targetCamera);
                                                break;
                                                case "takeOverTime":
                                                    var takeTime = msg.data.GetElementAsFloat(0);
                                                    if (debug) Debug.Log($"Going to cam position {camIndex} over {takeTime}");
                                                    if (GotoCameraPositionOverTime != null) GotoCameraPositionOverTime.Invoke(Camera.main, targetCamera.transform, takeTime);
                                                break;
                                                case "move":
                                                    var mX = msg.data.GetElementAsFloat(0);
                                                    var mY = msg.data.GetElementAsFloat(1);
                                                    var mZ = msg.data.GetElementAsFloat(2);
                                                    var mVector = new Vector3(mX, mY, mZ);
                                                    if (debug) Debug.Log($"Moving camera {camIndex} by {mVector}");
                                                    // rotate direction by camera's 'forward' and move camera
                                                    mVector = targetCamera.transform.TransformVector(mVector);
                                                    targetCamera.transform.position += mVector;
                                                break;
                                                case "rotate":
                                                    var rX = msg.data.GetElementAsFloat(0);
                                                    var rY = msg.data.GetElementAsFloat(1);
                                                    var rZ = msg.data.GetElementAsFloat(2);
                                                    var rVector = new Vector3(rX, rY, rZ);
                                                    if (debug) Debug.Log($"Rotating camera {camIndex} by {rVector}");
                                                    // rotate camera
                                                    targetCamera.transform.Rotate(rVector);
                                                break;
                                            }
                                        }

                                    break;
                                    
                                    case "listMaps":
                                        var maps = MIMA_System.Instance.currentScene.textureMaps;
                                        LogMessageOSC("/scene/numTextureMaps", maps.Count.ToString());
                                        for (int i = 0; i < maps.Count; i++)
                                        {
                                            switch (maps[i].TargetType)
                                            {
                                                case MIMA_Scene.TEXTURE_TARGET_TYPE.MATERIAL:
                                                    LogMessageOSC($"/scene/textureMap", $"{maps[i].TargetName} mapped to material {maps[i].targetMat.name}");
                                                    break;
                                                case MIMA_Scene.TEXTURE_TARGET_TYPE.LIGHT_COOKIE:
                                                    LogMessageOSC($"/scene/textureMap", $"{maps[i].TargetName} mapped to light {maps[i].targetLightName}");
                                                    break;
                                                case MIMA_Scene.TEXTURE_TARGET_TYPE.DECAL_PROJECTOR:
                                                    LogMessageOSC($"/scene/textureMap", $"{maps[i].TargetName} mapped to projector {maps[i].targetProjectorName}");
                                                    break;
                                            }
                                        }

                                        break;
                                    
                                    case "map":
                                        // a mapped texture in the scene
                                        // find out which one
                                        string mapName = parts[2];
                                        var map = MIMA_System.Instance.currentScene.textureMaps.Where(m =>
                                            m.TargetName == mapName).First();
                                        if (map != null)
                                        {
                                            switch (parts[3])
                                            {
                                                // what do we actually want to do with this map
                                                case "setSource":
                                                    var mapNewSourceName = msg.data.GetElementAsString(0);
                                                    map.sourceName = mapNewSourceName;
                                                    if (TextureMapChanged != null) TextureMapChanged.Invoke(map);
                                                break;
                                                case "setScale":
                                                    var newScale = msg.data.GetElementAsFloat(0);
                                                    map.scale = newScale;
                                                    if (TextureMapChanged != null) TextureMapChanged.Invoke(map);
                                                    break;
                                                case "setOffset":
                                                    var offsetX = msg.data.GetElementAsFloat(0);
                                                    var offsetY = msg.data.GetElementAsFloat(1);
                                                    Vector2 offset = new Vector2(offsetX, offsetY);
                                                    map.offset = offset;
                                                    if (TextureMapChanged != null) TextureMapChanged.Invoke(map);
                                                    break;
                                            }
                                            
                                        }
                                        else
                                        {
                                            Debug.LogError($"ERROR - no texture map found for {mapName}");
                                        }

                                    break;
                                }
                                break;

                            case "system":
                                // System-level stuff here, like rendering quality, window resolution etc

                                switch (parts[1])
                                {
                                    case "setLoggingToOSC":
                                        var param = msg.data.GetElementAsInt(0);
                                        AttachToDebugLog = param > 0; 
                                    break;
                                }

                                break;

                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"ERROR processing message {msg.address}");
                        Debug.LogError(ex.Message);
                        Debug.LogError(ex.StackTrace);
                    }
                
            }

            while (LogToOSCQueue.Count > 0)
            {
                var msg = LogToOSCQueue.Dequeue();
                LogMessageOSC(msg.address, msg.msg);
            }
        }

        public void CopyCameraSettings(Camera from, Camera to)
        {
            to.transform.position = from.transform.position;
            to.transform.rotation = from.transform.rotation;
            to.fieldOfView = from.fieldOfView;
            to.cameraType = from.cameraType;
            to.nearClipPlane = from.nearClipPlane;
            to.farClipPlane = from.farClipPlane;
        }

        public void LogMessageOSC(string address, string msg)
        {
            try
            {
                client.Send(address, msg);
            }
            catch (Exception e)
            {
                Debug.LogError($"ERROR sending mesage to address {address} : {msg}");
            }
        }

        private void OnDestroy()
        {    
            server.Dispose();
            client.Dispose();
        }
        
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
    }
    
}