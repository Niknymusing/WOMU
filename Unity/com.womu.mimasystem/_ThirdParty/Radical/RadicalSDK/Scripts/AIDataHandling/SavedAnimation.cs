using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

[System.Serializable]
public struct SavedAnimationFrame
{
    public FrameMeta frame_meta;
    public Dictionary<string, List<float>> frame_data;
}

[System.Serializable]
public class FrameMeta
{
    public float timestamp;
}

public enum eApplicationType
{
    CORE,
    STUDIO,
}

public enum eAIMode
{
    RT,
    SBS,
}

public enum VideoResolution
{
	High,
	Medium,
	Low,
}

public enum AIArchitecture
{
	Full,
	Light,
}

[System.Serializable]
public class InputMetaData
{
    [JsonConverter(typeof(StringEnumConverter))]
    public eApplicationType application;
    [JsonConverter(typeof(StringEnumConverter))]
    public eAIMode mode;
    [JsonConverter(typeof(StringEnumConverter))]
    public VideoResolution video_resolution;
    public string ai_input;
    public int frame_height;
    public int frame_width;
    public int number_of_frames;
    public float video_duration;
    public float avg_FPS;
    public bool has_timestamps;
    public int user_id;
    public string date_created;
}

[System.Serializable]
public class AnimationMetaData : InputMetaData
{
    public AnimationMetaData()
    {

    }
    public void SetAiInputMetaData(InputMetaData inputMetaData)
    {
        application = inputMetaData.application;
        ai_input = inputMetaData.ai_input;
        frame_width = inputMetaData.frame_width;
        frame_height = inputMetaData.frame_height;
        video_duration = inputMetaData.video_duration;
        avg_FPS = inputMetaData.avg_FPS;
        has_timestamps = inputMetaData.has_timestamps;
        user_id = inputMetaData.user_id;
        date_created = inputMetaData.date_created;
        number_of_frames = inputMetaData.number_of_frames;
        mode = inputMetaData.mode;
        video_resolution = inputMetaData.video_resolution;
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public AIArchitecture ai_architecture;
    public string ai_version;
    public string skeleton_type;
}


[System.Serializable]
public class SavedAnimation
{
    public AnimationMetaData meta_data;
    public List<SavedAnimationFrame> animation;
}
