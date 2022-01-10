using System.Collections;
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

    [Header("Assigned Model")]
    [SerializeField]
    private AIModel _assignedModel;

    [SerializeField] private bool _useAssignedModel;

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
    [SerializeField] private HumanPoseHandler _modelHandler;

    [SerializeField] public bool _firstValues;


    private AISkeleton PlaybackSkeleton
    {
        get { return Type == RigType.Gen3x2 ? _playbackSkeleton32 : _playbackSkeleton30; }
    }

    private HumanPoseHandler PlaybackHandler
    {
        get { return Type == RigType.Gen3x2 ? _playbackHandler32 : _playbackHandler30; }
    }


    private bool UseAssignedModel
    {
        get { return _useAssignedModel && _assignedModel != null && _assignedModel.enabled; }
    }

    private AIModel[] ActiveModels
    {
        get { return UseAssignedModel ? new AIModel[] { _assignedModel } : _aiModels; }
    }

    private void Awake()
    {

        _playbackHandler30 = new HumanPoseHandler(_playbackSkeleton30.GetComponentInChildren<Animator>().avatar,
            _playbackSkeleton30.transform.GetChild(0));
        _playbackHandler32 = new HumanPoseHandler(_playbackSkeleton32.GetComponentInChildren<Animator>().avatar,
            _playbackSkeleton32.transform.GetChild(0));
        _tPoseHandler32 = new HumanPoseHandler(_tPoseSkeleton32.avatar, _tPoseSkeleton32.transform.GetChild(0));
    }



    void UpdateModelsJoints()
    {
        if (PlaybackHandler == null) return;

        HumanPose pose = new HumanPose();
        if (Type == RigType.Gen3x0)
        {
            _playbackHandler30.GetHumanPose(ref pose);
            _playbackHandler32.SetHumanPose(ref pose);
        }

        _playbackHandler32.GetHumanPose(ref pose);

        for (int i = 0; i < ActiveModels.Length; i++)
        {
            if (!UseAssignedModel)
            {
                _modelHandler = new HumanPoseHandler(ActiveModels[i].avatar, ActiveModels[i].pivot);
                Debug.Log("Assing player for visualization");
            }
            _modelHandler.SetHumanPose(ref pose);
        }
    }

    public void ResetModelValues()
    {
        if (_useRootMotion)
        {
            for (int i = 0; i < ActiveModels.Length; i++)
            {
                ActiveModels[i].ResetToDefault();
            }
        }

        HumanPose pose = new HumanPose();
        _tPoseHandler32.GetHumanPose(ref pose);
        if (UseAssignedModel) _modelHandler.SetHumanPose(ref pose);

        _firstValues = false;
    }

    public void AssignModel(GameObject model)
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
                _useAssignedModel = true;
                _assignedModel = aiModel;
                _modelHandler = new HumanPoseHandler(aiModel.avatar, aiModel.pivot);
            }
        Instance.ResetModelValues();

        }
    }

    public void SampleFromValues(AIFrame aiFrame)
    {

        PlaybackSkeleton.UpdateFromOnPrem(aiFrame, true);
        UpdateModelsJoints();
        _firstValues = true;
    }

}