using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    private static AIPlayer _instance;

    public static AIPlayer Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AIPlayer>();
                if (_instance == null)
                {
                    throw new System.InvalidOperationException(
                        "UGUR!!! AI Player could not be found. Remember to place it on the scene you are using.");
                }
            }

            return _instance;
        }
    }

    [SerializeField] RigType type = RigType.Gen3x2;

    public RigType Type
    {
        get { return type; }
        set
        {
            type = value;
        }
    }

    [SerializeField] public Transform _focusPivot;

    [Header("Models")] 
    [SerializeField] public AIModel _tPoseSkeleton30;

    [SerializeField] private AIModel _tPoseSkeleton32 = default;
    [SerializeField] private AISkeleton _playbackSkeleton30 = default;
    [SerializeField] private AISkeleton _playbackSkeleton32 = default;
    [SerializeField] public AIModel[] _aiModels;

    [SerializeField] private bool _useRootMotion = true;

    [SerializeField] private HumanPoseHandler _playbackHandler30;
    [SerializeField] private HumanPoseHandler _playbackHandler32;
    [SerializeField] private HumanPoseHandler _tPoseHandler32;

    [SerializeField] public bool _firstValues;


    [SerializeField] private Dictionary<string, UserModel> _userData = new Dictionary<string, UserModel>();

    private AISkeleton PlaybackSkeleton
    {
        get { return Type == RigType.Gen3x2 ? _playbackSkeleton32 : _playbackSkeleton30; }
    }

    private HumanPoseHandler PlaybackHandler
    {
        get { return Type == RigType.Gen3x2 ? _playbackHandler32 : _playbackHandler30; }
    }

    private void Awake()
    {

        _playbackHandler30 = new HumanPoseHandler(_playbackSkeleton30.GetComponentInChildren<Animator>().avatar,
            _playbackSkeleton30.transform.GetChild(0));
        _playbackHandler32 = new HumanPoseHandler(_playbackSkeleton32.GetComponentInChildren<Animator>().avatar,
            _playbackSkeleton32.transform.GetChild(0));
        _tPoseHandler32 = new HumanPoseHandler(_tPoseSkeleton32.avatar, _tPoseSkeleton32.transform.GetChild(0));
    }



    void UpdateModelsJoints(string idUser)
    {
        if (PlaybackHandler == null) return;

        HumanPose pose = new HumanPose();
        if (Type == RigType.Gen3x0)
        {
            _playbackHandler30.GetHumanPose(ref pose);
            _playbackHandler32.SetHumanPose(ref pose);
        }

        _playbackHandler32.GetHumanPose(ref pose);

        if (!_userData[idUser].UseAssignedModel)
        {
            var model = _userData[idUser];
            model.ModelHandler = new HumanPoseHandler(_userData[idUser].AssignedModel.avatar, _userData[idUser].AssignedModel.pivot);
            _userData[idUser] = model;

            Debug.Log("Assing player for visualization");
        }
        _userData[idUser].ModelHandler.SetHumanPose(ref pose);
    }

    public void ResetModelValues(string idUser)
    {
        if (_useRootMotion)
        {
            _userData[idUser].AssignedModel.ResetToDefault();
        }

        HumanPose pose = new HumanPose();
        _tPoseHandler32.GetHumanPose(ref pose);
        if (_userData[idUser].UseAssignedModel) _userData[idUser].ModelHandler.SetHumanPose(ref pose);

        _firstValues = false;
    }

    public void AssignModel(string idUser, GameObject model)
    {
        if (model)
        {
            model.transform.parent = _focusPivot;
            model.transform.localPosition = Vector3.zero;
            var aiModel = model.GetComponent<AIModel>();
            if (aiModel == null)
            {
                aiModel = model.AddComponent<AIModel>();
                aiModel.SetupAuto();
            }

            if (aiModel.enabled)
            {
                UserModel um = new UserModel
                {
                    UseAssignedModel = true,
                    AssignedModel = aiModel,
                    ModelHandler = new HumanPoseHandler(aiModel.avatar, aiModel.pivot)
                };

                _userData.Add(idUser, um);
            }

            Instance.ResetModelValues(idUser);
        }
    }

    public void SampleFromValues(string idUser, AIFrame aiFrame)
    {

        PlaybackSkeleton.UpdateFromOnPrem(aiFrame, true);
        UpdateModelsJoints(idUser);
        _firstValues = true;
    }

}

[System.Serializable]
public struct UserModel
{
    public bool UseAssignedModel;
    public AIModel AssignedModel;
    public HumanPoseHandler ModelHandler;
}