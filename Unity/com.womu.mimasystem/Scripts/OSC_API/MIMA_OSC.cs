using System;
using System.CodeDom;
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
        private List<Transform> CamerasInScene = new List<Transform>();

        void Start()
        {
            server = new OscServer(oscPortServer);
            server.MessageDispatcher.AddCallback(String.Empty, (address, data) =>
            {
                // add messages to a queue, because this callback doesn't happen on the Update thread
                var msg = new OSCMsg(address, data);
                MessageQueue.Enqueue(msg);
            });

            client = new OscClient("255.255.255.255", oscPortClient);
            
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

        }

        public override void UpdateUI()
        {
            CamerasInScene.Clear();
            var camPosNames = MIMA_System.Instance.currentScene.cameraPositions;
            for (int i = 0; i < camPosNames.Count; i++)
            {
                var camGO = GameObject.Find(camPosNames[i]);
                if (camGO != null) CamerasInScene.Add(camGO.transform);
                else Debug.LogError($"ERROR - no object found for {camPosNames[i]}");
                    
            }
        }

        private void Update()
        {
            // OSC API DEFINED MOSTLY HERE
            // process the message queue on the update thread, so we can actually do Unity things
            while (MessageQueue.Count > 0)
            {
                
                    var msg = MessageQueue.Dequeue();
                    if (debug) Debug.Log($"Processing OSC: {msg.address} / with {msg.data.GetElementCount()} args");
                    if (debug)
                    {
                        for (int i = 0; i < msg.data.GetElementCount(); i++)
                        {
                            Debug.Log($"arg {i} : {msg.data.GetElementAsString(i)}");
                        }
                    }
                    
                    
                    var parts = msg.address.Split('/');

                    if (debug)
                    {
                        for (int i = 0; i < parts.Length; i++)
                        {
                            Debug.Log($"{i} : {parts[i]}");
                        }
                    }
                    // try
                    // {
                        switch (parts[1])
                        {
                            case "scene":
                                // scene-level stuff here, like loading scenes, setting sources etc

                                switch (parts[2])
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
                                    
                                    case "listEffects":
                                        LogMessageOSC("/scene/numEffects", MIMA_System.Instance.currentScene.effects.Count.ToString());
                                        foreach (var eff in MIMA_System.Instance.currentScene.effects)
                                            LogMessageOSC("/scene/effect", $"{eff.Name}");
                                        break;
                                    
                                    case "blackout":
                                        var toValue = float.Parse(msg.data.GetElementAsString(0));
                                        var overTime = float.Parse(msg.data.GetElementAsString(1));
                                        Debug.Log($"Setting blackout to {toValue} over {overTime}");
                                        if (SetBlackoutOverTime != null) SetBlackoutOverTime.Invoke(toValue, overTime);
                                        break;
                                    case "camera":

                                        var targetCamera = Camera.main.transform;
                                        int camIndex = -1;
                                        if (parts[3] != "main")
                                        {
                                            camIndex = int.Parse(parts[3]);
                                            targetCamera = CamerasInScene[camIndex];
                                        }
                                       

                                        switch (parts[4])
                                        {
                                            case "setRandomMotion":
                                                var randomMotionAmount = float.Parse(msg.data.GetElementAsString(0));
                                                if (debug) Debug.Log($"Setting camera random motion to {randomMotionAmount}");
                                                if (SetCameraRandomMotion != null) SetCameraRandomMotion.Invoke(Camera.main, randomMotionAmount);
                                            break;
                                            case "take":
                                                if (debug) Debug.Log($"Setting main camera settings from camera {camIndex}");
                                                if (GotoCameraPositionOverTime != null) GotoCameraPositionOverTime.Invoke(Camera.main, targetCamera, 0.0f);
                                            break;
                                            case "takeOverTime":
                                                var takeTime = float.Parse(msg.data.GetElementAsString(0));
                                                if (debug) Debug.Log($"Going to cam position {camIndex} over {takeTime}");
                                                if (GotoCameraPositionOverTime != null) GotoCameraPositionOverTime.Invoke(Camera.main, targetCamera, takeTime);
                                            break;
                                            case "move":
                                                var mX = float.Parse(msg.data.GetElementAsString(0));
                                                var mY = float.Parse(msg.data.GetElementAsString(1));
                                                var mZ = float.Parse(msg.data.GetElementAsString(2));
                                                var mVector = new Vector3(mX, mY, mZ);
                                                float moveTime = 0.0f;
                                                if (msg.data.GetElementCount() > 3)
                                                    moveTime = float.Parse(msg.data.GetElementAsString(3));
                                                if (debug) Debug.Log($"Moving camera {camIndex} by {mVector} over {moveTime}");
                                                // rotate direction by camera's 'forward' and move camera
                                                mVector = targetCamera.transform.TransformVector(mVector);
                                                if (MoveTransformByOverTime != null) MoveTransformByOverTime.Invoke(targetCamera, mVector, moveTime);
                                                
                                            break;
                                            case "rotate":
                                                var rX = float.Parse(msg.data.GetElementAsString(0));
                                                var rY = float.Parse(msg.data.GetElementAsString(1));
                                                var rZ = float.Parse(msg.data.GetElementAsString(2));
                                                var rVector = new Vector3(rX, rY, rZ);
                                                float rotateTime = 0.0f;
                                                if (msg.data.GetElementCount() > 3)
                                                    rotateTime = float.Parse(msg.data.GetElementAsString(3));
                                                if (debug) Debug.Log($"Rotating camera {camIndex} by {rVector} over {rotateTime}");
                                                // rotate camera
                                                if (RotateTransformByOverTime != null) RotateTransformByOverTime.Invoke(targetCamera, rVector, rotateTime);
                                                
                                            break;
                                            
                                        }

                                    break;
                                    
                                    case "effect":
                
                                        var effName = parts[3];
                                        var targetEffect = MIMA_System.Instance.currentScene.effects.FirstOrDefault(eff =>
                                            eff.Name == effName);
                                        if (targetEffect != null)
                                        {
                                            switch (parts[4])
                                            {
                                                case "set":
                                                    var paramName = parts[5];
                                                    if (debug) Debug.Log($"Settting param {paramName} on {targetEffect.Name}");
                                                   
                                                    MIMA_Effect.EffectParameter p;
                                                    if (targetEffect.Effect.parameters.TryGetValue(paramName, out p))
                                                    {
                                                        if (p.valueType == typeof(bool)) SetEffectParamBool.Invoke(targetEffect.Effect, p.id, int.Parse(msg.data.GetElementAsString(0)) > 0);
                                                        if (p.valueType == typeof(float)) SetEffectParamFloat.Invoke(targetEffect.Effect, p.id, float.Parse(msg.data.GetElementAsString(0)));
                                                        if (p.valueType == typeof(int)) SetEffectParamInt.Invoke(targetEffect.Effect, p.id, (int)float.Parse(msg.data.GetElementAsString(0)));
                                                        if (p.valueType == typeof(uint)) SetEffectParamUint.Invoke(targetEffect.Effect, p.id, (uint)float.Parse(msg.data.GetElementAsString(0)));
                                                        if (p.valueType == typeof(Vector2))
                                                        {
                                                            var pX = float.Parse(msg.data.GetElementAsString(0));
                                                            var pY = float.Parse(msg.data.GetElementAsString(1));
                                                            SetEffectParamVector2.Invoke(targetEffect.Effect, p.id, new Vector2(pX, pY));
                                                        }
                                                        if (p.valueType == typeof(Vector3))
                                                        {
                                                            var pX = float.Parse(msg.data.GetElementAsString(0));
                                                            var pY = float.Parse(msg.data.GetElementAsString(1));
                                                            var pZ = float.Parse(msg.data.GetElementAsString(2));
                                                            SetEffectParamVector3.Invoke(targetEffect.Effect, p.id, new Vector3(pX, pY, pZ));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Debug.LogError($"ERROR - couldn't find effect parameter for {paramName}");
                                                    }

                                                    
                                                    break;
                                                case "simSpeed":
                                                    float targetSpeed = float.Parse(msg.data.GetElementAsString(0));
                                                    float speedOverTime = 0;
                                                    if (msg.data.GetElementCount() > 1)
                                                    {
                                                        speedOverTime = float.Parse(msg.data.GetElementAsString(1));
                                                    }

                                                    if (speedOverTime > 0)
                                                    {
                                                        SetEffectSimSpeedOverTime.Invoke(targetEffect.Effect, targetSpeed, speedOverTime);
                                                    }
                                                    else
                                                    {
                                                        SetEffectSimSpeed.Invoke(targetEffect.Effect, targetSpeed);
                                                    }
                                                    break;
                                                case "move":
                                                    var mX = float.Parse(msg.data.GetElementAsString(0));
                                                    var mY = float.Parse(msg.data.GetElementAsString(1));
                                                    var mZ = float.Parse(msg.data.GetElementAsString(2));
                                                    var mVector = new Vector3(mX, mY, mZ);
                                                    float moveTime = 0.0f;
                                                    if (msg.data.GetElementCount() > 3)
                                                        moveTime = float.Parse(msg.data.GetElementAsString(3));
                                                    if (debug) Debug.Log($"Moving effect object {targetEffect.Name} by {mVector} over {moveTime}");
                                                    // rotate direction by camera's 'forward' and move camera
                                                    mVector = targetEffect.Effect.transform.TransformVector(mVector);
                                                    if (MoveTransformByOverTime != null) MoveTransformByOverTime.Invoke(targetEffect.Effect.transform, mVector, moveTime); 
                                                    break;
                                                case "rotate":
                                                    var rX = float.Parse(msg.data.GetElementAsString(0));
                                                    var rY = float.Parse(msg.data.GetElementAsString(1));
                                                    var rZ = float.Parse(msg.data.GetElementAsString(2));
                                                    var rVector = new Vector3(rX, rY, rZ);
                                                    float rotateTime = 0.0f;
                                                    if (msg.data.GetElementCount() > 3)
                                                        rotateTime = float.Parse(msg.data.GetElementAsString(3));
                                                    if (debug) Debug.Log($"Rotating effect object {targetEffect.Name} by {rVector} over {rotateTime}");
                                                    // rotate camera
                                                    if (RotateTransformByOverTime != null) RotateTransformByOverTime.Invoke(targetEffect.Effect.transform, rVector, rotateTime);
                                                    break;
                                                case "scale":
                                                    var sX = float.Parse(msg.data.GetElementAsString(0));
                                                    var sY = float.Parse(msg.data.GetElementAsString(1));
                                                    var sZ = float.Parse(msg.data.GetElementAsString(2));
                                                    var sVector = new Vector3(sX, sY, sZ);
                                                    float scaleTime = 0.0f;
                                                    if (msg.data.GetElementCount() > 3)
                                                        scaleTime = float.Parse(msg.data.GetElementAsString(3));
                                                    if (debug) Debug.Log($"Setting scale on {targetEffect.Name} to {sVector} over {scaleTime}");
                                                    // rotate camera
                                                    if (SetTransformScaleOverTime != null) SetTransformScaleOverTime.Invoke(targetEffect.Effect.transform, sVector, scaleTime);
                                                    break;
                                            }
                                            
                                        }
                                        else
                                        {
                                            Debug.LogError($"ERROR - could not find effect {effName}");
                                        }

                                        break;
                                    
                                    case "listMaps":
                                        if (MIMA_System.Instance.currentScene == null) LogMessageOSC("/log/error", "No scene loaded");
                                        else
                                        {
                                            var maps = MIMA_System.Instance.currentScene.textureMaps;
                                            LogMessageOSC("/scene/numTextureMaps", maps.Count.ToString());
                                            string mapList = "";
                                        
                                            for (int i = 0; i < maps.Count; i++)
                                            {
                                                mapList += maps[i].TargetName + "|";
                                            }
                                            LogMessageOSC($"/scene/textureMaps", $"{mapList}");
 
                                        }
                                        break;
                                    case "listSources":
                                        var sources = MIMA_System.Instance.GetExternalSources();
                                        LogMessageOSC("/scene/numExternalSources", sources.Count.ToString());
                                        string sourceList = "";
                                        for (int i = 0; i < sources.Count; i++)
                                        {
                                            sourceList += sources[i] + "|";
                                        }
                                        LogMessageOSC("/scene/externalSources", $"{sourceList}");
                                        break;
                                    
                                    case "map":
                                        if (MIMA_System.Instance.currentScene == null) LogMessageOSC("/log/error", "No scene loaded");
                                        else
                                        {
                                            // a mapped texture in the scene
                                            // find out which one
                                            string mapName = parts[3];
                                            var map = MIMA_System.Instance.currentScene.textureMaps.Where(m =>
                                                m.TargetName == mapName).First();
                                            if (map != null)
                                            {
                                                switch (parts[4])
                                                {
                                                    // what do we actually want to do with this map
                                                    case "setSource":
                                                        var mapNewSourceName = msg.data.GetElementAsString(0);
                                                        map.sourceName = mapNewSourceName;
                                                        if (TextureMapChanged != null) TextureMapChanged.Invoke(map);
                                                        break;
                                                    case "setScale":
                                                        var newScale = float.Parse(msg.data.GetElementAsString(0));
                                                        map.scale = newScale;
                                                        if (TextureMapChanged != null) TextureMapChanged.Invoke(map);
                                                        break;
                                                    case "setOffset":
                                                        var offsetX = float.Parse(msg.data.GetElementAsString(0));
                                                        var offsetY = float.Parse(msg.data.GetElementAsString(1));
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
                                        }

                                        break;
                                    
                                    case "pose":
                                        // incoming pose for character models (or anything else for that matter)
                                        int clientID = int.Parse(parts[3]);
                                        int landscapeID = int.Parse(parts[4]);

                                        float poseX = msg.data.GetElementAsFloat(0);
                                        float poseY = msg.data.GetElementAsFloat(1);
                                        float poseZ = msg.data.GetElementAsFloat(2);

                                        if (SetCharacterIDLandmarkPosition != null)
                                            SetCharacterIDLandmarkPosition(clientID, landscapeID,
                                                new Vector3(poseX, poseY, poseZ));

                                        break;
                                }
                                break;

                            case "system":
                                // System-level stuff here, like rendering quality, window resolution etc

                                switch (parts[2])
                                {
                                    case "setLoggingToOSC":
                                        var param = int.Parse(msg.data.GetElementAsString(0));
                                        AttachToDebugLog = param > 0; 
                                    break;
                                }

                                break;

                        }
                    // }
                    // catch (Exception ex)
                    // {
                    //     Debug.LogError($"ERROR processing message {msg.address}");
                    //     Debug.LogError(ex.Message);
                    //     Debug.LogError(ex.StackTrace);
                    //
                    //     throw ex;
                    // }
                
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

        public void LogMessageOSC(string address, string arg)
        {
            try
            {
                client.Send(address, arg);
            }
            catch (Exception e)
            {
                Debug.LogError($"ERROR sending mesage to address {address} : {arg}");
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