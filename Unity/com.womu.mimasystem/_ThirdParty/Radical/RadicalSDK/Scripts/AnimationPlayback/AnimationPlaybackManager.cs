using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Radical;
using System.Text.Json;

public class AnimationPlaybackManager : MonoBehaviour
{

    private static string visualizationSceneName = "_StudioVisualizer";

    public GameObject aiCharacter;
    public AnimationPlayback animationPlayback;
    public string fileName = "299539.json";
    public bool useLive = false;
    public string roomName = "test";
    public string socketServerURL = "";

    private string animationPath;
    private RadicalLiveInterface radSdk;

    public void Awake()
    {
        // SceneManager.LoadScene(visualizationSceneName, LoadSceneMode.Additive);
    }

    List<AIFrame> aiFrames;

    void Start()
    {
        animationPath = Path.Combine(Application.streamingAssetsPath, fileName);

        aiCharacter = Instantiate(aiCharacter);
        AIPlayer.Instance.AssignModel(aiCharacter);

        if (useLive)
        {
            animationPlayback.StartPlaybackLive(0);
            ConnectToWSS();
        }
        else
        {
            aiFrames = AnimationReader.ReadAnimationFile(animationPath, -1, false);
            animationPlayback.AssignAnimation(aiFrames);
            animationPlayback.StartPlayback(0);
        }
    }

    void ConnectToWSS()
    {
        Debug.Log("Connecting...");
        radSdk = new RadicalLiveInterface(socketServerURL, roomName);
        radSdk.OnConnected(ConnectedToRoom);
        radSdk.OnData(result => GettingData(result));
        radSdk.Connect();
    }

    void GettingData(IList<JsonElement> data)
    {
        // Debug.Log("getting frame data");
        animationPlayback.playLiveFrame(data);
    }

    void ConnectedToRoom()
    {
        // Debug.Log("Connected to room");
    }
}