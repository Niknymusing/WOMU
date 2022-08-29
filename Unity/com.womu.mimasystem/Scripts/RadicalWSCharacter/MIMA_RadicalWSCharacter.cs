using System;
using System.Collections;
using System.Collections.Generic;
using MIMA;
using UnityEngine;
using UnityEngine.PlayerLoop;
using System.Runtime.InteropServices;
using PlasticPipe.Server;
using UnityEngine.XR;

public class MIMA_RadicalWSCharacter : MIMA_CharacterPoseControlBase
{
    public enum CONNECTION_STATUS
    {
        DISCONNECTED,
        CONNECTING,
        CONNECTED
    }

    public CONNECTION_STATUS connStatus = CONNECTION_STATUS.DISCONNECTED;
    
    public SkinnedMeshRenderer smr;
   

    public Transform smrRootBone;

    public string uriServer = "wss://room-handler.live-prod.live.k8s.getrad.co";
    public string userToken = "a49e6c47-1a64-47b8-a831-69edaae5ef91";
    
    /// <summary>
    /// Here we use current Client ID in place of RoomID
    /// </summary>
    private string _currentClientID;
    public string roomId
    {
        get { return _currentClientID;  }
    }

    
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void MyCallbackDelegate(string msg);
    private MyCallbackDelegate callback;

    public List<UserSimulationData> CurrentUsers = new List<UserSimulationData>();

    public bool ApplyPositionOffsetMultipleClients = false;
    public Vector3 ClientPositionOffset;

    public GameObject RadicalCharacterPrefab;

    public float _initTime;

    public Queue<string> ServerMessages = new Queue<string>();
    

    void Start()
    {
        base.Start();

        _initTime = Time.time;

        StartCoroutine(FindSkinnedMeshRenderer());

    }
    
    new void Update()
    {
        base.Update();

        // Changing the client ID (room ID in this case) triggers a disconnect & reconnect to the server
        if (clientID != _currentClientID)
        {
            _currentClientID = clientID;

            if (clientID != "none")
            {
                ConnectToServer();
            }
        }
        
        
        // go through server queue messages
        while (ServerMessages.Count > 0)
        {
            var msg = ServerMessages.Dequeue();
            
            if (msg.Contains("Connected"))
            {
                Debug.Log("Connected to Radical WS Server!");
                connStatus = CONNECTION_STATUS.CONNECTED;
            } else if (msg.Contains("Disconnected"))
            {
                Debug.Log("Disconnected from Radical WS Server");
                connStatus = CONNECTION_STATUS.DISCONNECTED;
            }
            else
            {
                // it's a bit of frame data - decode JSON and handle
                var jsonData = JsonUtility.FromJson<AnimationServer>(msg);
                
                // check if we have this user already
                var userData = new UserSimulationData();
                bool isNewUser = true;
                foreach (var u in CurrentUsers)
                {
                    if (u.IDUser == jsonData.attendeeId)
                    {
                        isNewUser = false;
                        userData = u;
                        break;
                    }
                }

                if (isNewUser)
                {
                    Debug.Log($"New attendee joined {userData.IDUser}");
                    
                    // create new user
                    UserSimulationData newUser = new UserSimulationData
                    {
                        IDUser = jsonData.attendeeId,
                        OffsetPosition = ApplyPositionOffsetMultipleClients ? ClientPositionOffset : Vector3.zero
                    };

                    newUser.AiFrames = AnimationReader.ReadAnimationFile(
                        ReadAnimationServer.CreateToAnimationSaved(jsonData), newUser.OffsetPosition, -1, false);

                    CurrentUsers.Add(newUser);
                    
                    // apply offset
                    if (ApplyPositionOffsetMultipleClients)
                        ClientPositionOffset += ClientPositionOffset;
                    

                }
                else
                {
                    // append frames to existing user
                    userData.AiFrames.AddRange(AnimationReader.ReadAnimationFile(
                        ReadAnimationServer.CreateToAnimationSaved(jsonData), userData.OffsetPosition, -1, false));
                    
                }

            }
            
        }
        
        
        // iterate through current users, ensure there's a instantiated prefab for each + simulate update
        for (int i=0; i < CurrentUsers.Count; i++)
        {
            var u = CurrentUsers[i];
            if (u.AiCharacter == null)
            {
                Debug.Log($"Creating new character gameobject for {u.IDUser}");
                // instantiate a new prefab for this user & reset position
                u.AiCharacter = Instantiate(RadicalCharacterPrefab, transform);
                u.AiCharacter.transform.localPosition = Vector3.zero;
                u.AiCharacter.transform.localRotation = Quaternion.identity;
                u.AiCharacter.name = $"User_{u.IDUser}";
                
                // make link between user ID and character
                AIPlayer.Instance.AssignModel(u.IDUser, u.AiCharacter);
                FixTimestamps(u);

               
            }
            
            // update simulation if it's the right time
            if (u.AiFrames.Count - 1 < u.CurrentFrame) continue;
            if (Time.time - _initTime + u.AiFrames[0].Timestamp >= u.AiFrames[u.CurrentFrame].Timestamp)
            {
                AIPlayer.Instance.SampleFromValues(u.IDUser, u.AiFrames[u.CurrentFrame]);
                u.CurrentFrame++;
            }
            
            // put it back because structs
            CurrentUsers[i] = u;

        }
        
    }


    IEnumerator FindSkinnedMeshRenderer()
    {
        // This looks for a SMR on child objects - which we will then drive with our data from Radical server
        while (smr == null)
        {
            smr = GetComponentInChildren<SkinnedMeshRenderer>();
            yield return null;
        }

        Debug.Log("Found SMR " + smr.name + " on character object");
        vfx.SetSkinnedMeshRenderer("targetSkinnedMeshRenderer", smr);
        int numVerts = smr.sharedMesh.vertexCount;
        Debug.Log("Number of verts : " + numVerts);
        vfx.SetInt("targetVertexCount", numVerts);
        
        vfx.transform.SetParent(smr.rootBone);
        vfx.transform.localPosition = Vector3.zero;
        vfx.transform.localRotation = Quaternion.identity;
        vfx.transform.localScale = Vector3.one;
        
    }

    [ContextMenu("Connect To Server")]
    public void ConnectToServer()
    {
        if (connStatus != CONNECTION_STATUS.DISCONNECTED)
        {
            Disconnect();
            connStatus = CONNECTION_STATUS.DISCONNECTED;
        }
        
        if (callback == null)
        {
            callback = ServerCallbackDelegate;
            DataReceivedCallback(callback);
        }

        Debug.Log($"Connecting to room {roomId} on {uriServer}");
        connStatus = CONNECTION_STATUS.CONNECTING;
        Connect(uriServer, userToken, roomId);

    }

    void ServerCallbackDelegate(string msg)
    {
        try
        {
            ServerMessages.Enqueue(msg);

        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            Debug.Log(ex);
        }
        
    }
    
    

    
    private void FixTimestamps(UserSimulationData us)
    {
        for (int i = 0; i < us.AiFrames.Count; i++)
            if (us.AiFrames[i].Timestamp == 0)
                us.AiFrames[i].Timestamp = 1.0f * i / 30;
    }
    


    private void OnApplicationQuit()
    {
        if (connStatus != CONNECTION_STATUS.DISCONNECTED)
        {
            Disconnect();
        }
    }


    // Radical DLL interface
    [DllImport("Wrapper")]
    public static extern void Connect(string uri, string userToken, string room);

    [DllImport("Wrapper")]
    public static extern void Disconnect();

    [DllImport("Wrapper")]
    public static extern void DataReceivedCallback(MyCallbackDelegate callbackMsg);


}
