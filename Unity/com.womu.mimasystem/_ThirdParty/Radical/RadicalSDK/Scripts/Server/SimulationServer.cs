using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class SimulationServer : MonoBehaviour
{
    //Test Files
    [SerializeField] private InputTemp[] inputTemps;
    [SerializeField] private Vector3 offsetByServerUser;

    [Space]
    [SerializeField] private GameObject _aiCharacterPrefab;

    private float _initTime;

    private List<UserSimulationData> _usersSimulated = new List<UserSimulationData>();

    private static string visualizationSceneName = "_StudioVisualizer";

    private void Awake()
    {
        // SceneManager.LoadScene(visualizationSceneName, LoadSceneMode.Additive);
    }

    private void Start()
    {
        // StartCoroutine(InstanceUsersLocal());
    }

    private void Update()
    {
        for (int i = 0; i < _usersSimulated.Count; i++)
        {
            if(_usersSimulated[i].AiCharacter == null)
            {
                var us = _usersSimulated[i];
                us.AiCharacter = Instantiate(_aiCharacterPrefab, transform);

                us.AiCharacter.name = $"User_{us.IDUser}";

                AIPlayer.Instance.AssignModel(us.IDUser, us.AiCharacter);
                FixTimestamps(us);

                _usersSimulated[i] = us;
            }

            var data = _usersSimulated[i];
            ReadInputInRealTimeSimulate(ref data);
            _usersSimulated[i] = data;
        }
    }

    private string jsonAnimation;
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

    private IEnumerator InstanceUsersLocal()
    {
        string extJson = ".json";
        for (int i = 0; i < inputTemps.Length; i++)
        {
            inputTemps[i].FilesAnimationInStreamingAsset = Path.Combine(Application.streamingAssetsPath, $"{inputTemps[i].FilesAnimationInStreamingAsset}{extJson}");

            yield return StartCoroutine(LoadStreamingAsset(inputTemps[i].FilesAnimationInStreamingAsset));
            UserSimulationData us = new UserSimulationData
            {
                IDUser = inputTemps[i].UserID,
                AiFrames = AnimationReader.ReadAnimationFile(jsonAnimation, inputTemps[i].OffsetPosition, -1, false),
                AiCharacter = Instantiate(_aiCharacterPrefab, transform)
            };
            us.AiCharacter.name = $"User_{us.IDUser}";

            AIPlayer.Instance.AssignModel(us.IDUser, us.AiCharacter);
            FixTimestamps(us);

            _usersSimulated.Add(us);
        }

        _initTime = Time.time;
    }

    public void ReadServerData(string json)
    {
        var jsonData = JsonUtility.FromJson<AnimationServer>(json);

        bool userExist = false;
        UserSimulationData userData = new UserSimulationData();
        foreach (var user in _usersSimulated)
        {
            if (user.IDUser == jsonData.attendeeId)
            {
                userExist = true;
                userData = user;
                break;
            }
        }

        if (userExist)
        {
            var frame = AnimationReader.ReadAnimationFile(ReadAnimationServer.CreateToAnimationSaved(jsonData), userData.OffsetPosition, -1, false);
            userData.AiFrames.AddRange(frame);
        }
        else
        {
            try
            {
                UserSimulationData us = new UserSimulationData
                {
                    IDUser = jsonData.attendeeId,
                    OffsetPosition = offsetByServerUser
                };

                us.AiFrames = AnimationReader.ReadAnimationFile(ReadAnimationServer.CreateToAnimationSaved(jsonData), us.OffsetPosition, -1, false);
                _usersSimulated.Add(us);

                offsetByServerUser += offsetByServerUser;
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
                throw;
            }
        }
    }

    private void ReadInputInRealTimeSimulate(ref UserSimulationData userData)
    {
        if (userData.AiFrames.Count -1 < userData.CurrentFrame)
            return;

        //Debug.Log($"{userData.AiFrames[userData.CurrentFrame].Timestamp}; {Time.time}; {_initTime};");

        if (Time.time - _initTime + userData.AiFrames[0].Timestamp >= userData.AiFrames[userData.CurrentFrame].Timestamp)
        {
            PlaybackSimulate(userData.IDUser, userData.AiFrames[userData.CurrentFrame]);
            userData.CurrentFrame += 1;
        }
    }

    private void PlaybackSimulate(string userID, AIFrame frame)
    {
        AIPlayer.Instance.SampleFromValues(userID, frame);
    }

    private void FixTimestamps(UserSimulationData us)
    {
        for (int i = 0; i < us.AiFrames.Count; i++)
            if (us.AiFrames[i].Timestamp == 0)
                us.AiFrames[i].Timestamp = 1.0f * i / 30;
    }
}

[System.Serializable]
public struct InputTemp
{
    public string UserID;
    public string FilesAnimationInStreamingAsset;
    public Vector3 OffsetPosition;
}

public struct UserSimulationData
{
    public string IDUser;
    public GameObject AiCharacter;
    public List<AIFrame> AiFrames;
    public int CurrentFrame;
    public Vector3 OffsetPosition;
}
