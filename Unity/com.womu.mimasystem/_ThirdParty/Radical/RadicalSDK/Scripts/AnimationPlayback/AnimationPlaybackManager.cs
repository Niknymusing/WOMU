using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class AnimationPlaybackManager : MonoBehaviour
{

    private static string visualizationSceneName = "_StudioVisualizer";

    public GameObject aiCharacter;
    public AnimationPlayback animationPlayback;

    private string animationPath = Path.Combine(Application.streamingAssetsPath, "299539.json");


    public void Awake()
    {
        SceneManager.LoadScene(visualizationSceneName, LoadSceneMode.Additive);
    }

    private string jsonAnimation;
    List<AIFrame> aiFrames;
    private IEnumerator Start()
    {
        yield return StartCoroutine(LoadStreamingAsset(animationPath));

        aiFrames = AnimationReader.ReadAnimationFile(jsonAnimation, -1, false);
        aiCharacter = Instantiate(aiCharacter);
        AIPlayer.Instance.AssignModel("0", aiCharacter);
        animationPlayback.AssignAnimation(aiFrames);
        animationPlayback.StartPlayback(0);
    }

    //Temp
    private IEnumerator LoadStreamingAsset(string filePath)
    {
        if (filePath.Contains("://") || filePath.Contains(":///"))
        {
            UnityWebRequest www = new UnityWebRequest(filePath);
            yield return www.SendWebRequest();
            jsonAnimation = www.downloadHandler.text;
        }
        else
        {
            jsonAnimation = File.ReadAllText(filePath);
        }
    }
}
