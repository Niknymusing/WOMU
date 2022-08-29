using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadAnimationServer : MonoBehaviour
{
    public void Read(string json)
    {
        var obj = JsonUtility.FromJson<AnimationServer>(json);

        Debug.Log(JsonUtility.ToJson(obj));
    }

    public static SavedAnimation CreateToAnimationSaved(AnimationServer animServer)
    {
        SavedAnimation sa = new SavedAnimation
        {
            meta_data = new AnimationMetaData
            {
                ai_architecture = AIArchitecture.Full,
                ai_version = "3.2.4",
                skeleton_type = "3.2",
                application = eApplicationType.CORE,
                mode = eAIMode.SBS,
                video_resolution = VideoResolution.High,
                ai_input = "2.1",
                frame_height = 480,
                frame_width = 640,
                video_duration = float.MaxValue,
                avg_FPS = 30f,
                has_timestamps = false,
                user_id = -1,
                number_of_frames = int.MaxValue,
                date_created = "2021-06-03 14:38:01"
            },
            animation = new List<SavedAnimationFrame>
            {
                new SavedAnimationFrame
                {
                    frame_meta = new FrameMeta
                    {
                        timestamp = ((float)animServer.timestamp[0]),
                    },
                    frame_data = new Dictionary<string, List<float>>()
                }
            }
        };

        sa.animation[0].frame_data.Add("root_t", GetListFloats(animServer.root_t));
        sa.animation[0].frame_data.Add("root_r", GetListFloats(animServer.root_r));
        sa.animation[0].frame_data.Add("LeftUpLegDummy_r", GetListFloats(animServer.LeftUpLegDummy_r));
        sa.animation[0].frame_data.Add("LeftUpLeg_r", GetListFloats(animServer.LeftUpLeg_r));
        sa.animation[0].frame_data.Add("LeftLeg_r", GetListFloats(animServer.LeftLeg_r));
        sa.animation[0].frame_data.Add("LeftFoot_r", GetListFloats(animServer.LeftFoot_r));
        sa.animation[0].frame_data.Add("RightUpLegDummy_r", GetListFloats(animServer.RightUpLegDummy_r));
        sa.animation[0].frame_data.Add("RightUpLeg_r", GetListFloats(animServer.RightUpLeg_r));
        sa.animation[0].frame_data.Add("RightLeg_r", GetListFloats(animServer.RightLeg_r));
        sa.animation[0].frame_data.Add("RightFoot_r", GetListFloats(animServer.RightFoot_r));
        sa.animation[0].frame_data.Add("SpineDummy_r", GetListFloats(animServer.SpineDummy_r));
        sa.animation[0].frame_data.Add("Spine_r", GetListFloats(animServer.Spine_r));
        sa.animation[0].frame_data.Add("Spine1_r", GetListFloats(animServer.Spine1_r));
        sa.animation[0].frame_data.Add("Spine2_r", GetListFloats(animServer.Spine2_r));
        sa.animation[0].frame_data.Add("NeckDummy_r", GetListFloats(animServer.NeckDummy_r));
        sa.animation[0].frame_data.Add("Neck_r", GetListFloats(animServer.Neck_r));
        sa.animation[0].frame_data.Add("Head_r", GetListFloats(animServer.Head_r));
        sa.animation[0].frame_data.Add("RightShoulderDummy_r", GetListFloats(animServer.RightShoulderDummy_r));
        sa.animation[0].frame_data.Add("RightShoulder_r", GetListFloats(animServer.RightShoulder_r));
        sa.animation[0].frame_data.Add("RightArm_r", GetListFloats(animServer.RightArm_r));
        sa.animation[0].frame_data.Add("RightForeArm_r", GetListFloats(animServer.RightForeArm_r));
        sa.animation[0].frame_data.Add("RightHand_r", GetListFloats(animServer.RightHand_r));
        sa.animation[0].frame_data.Add("LeftShoulderDummy_r", GetListFloats(animServer.LeftShoulderDummy_r));
        sa.animation[0].frame_data.Add("LeftShoulder_r", GetListFloats(animServer.LeftShoulder_r));
        sa.animation[0].frame_data.Add("LeftArm_r", GetListFloats(animServer.LeftArm_r));
        sa.animation[0].frame_data.Add("LeftForeArm_r", GetListFloats(animServer.LeftForeArm_r));
        sa.animation[0].frame_data.Add("LeftHand_r", GetListFloats(animServer.LeftHand_r));

        return sa;
    }

    private static List<float> GetListFloats(double[] arr)
    {
        List<float> data = new List<float>();
        for (int i = 0; i < arr.Length; i++)
            data.Add((float)arr[i]);

        return data;
    }
}

[System.Serializable]
public struct AnimationServer
{
    public double[] Head_r;
    public double[] LeftArm_r;
    public double[] LeftFoot_r;
    public double[] LeftForeArm_r;
    public double[] LeftHand_r;
    public double[] LeftLeg_r;
    public double[] LeftShoulderDummy_r;
    public double[] LeftShoulder_r;
    public double[] LeftUpLegDummy_r;
    public double[] LeftUpLeg_r;
    public double[] NeckDummy_r;
    public double[] Neck_r;
    public double[] RightArm_r;
    public double[] RightFoot_r;
    public double[] RightForeArm_r;
    public double[] RightHand_r;
    public double[] RightLeg_r;
    public double[] RightShoulderDummy_r;
    public double[] RightShoulder_r;
    public double[] RightUpLegDummy_r;
    public double[] RightUpLeg_r;
    public double[] Spine1_r;
    public double[] Spine2_r;
    public double[] SpineDummy_r;
    public double[] Spine_r;
    public string attendeeId;
    public double[] root_r;
    public double[] root_t;
    public double[] timestamp;

}
