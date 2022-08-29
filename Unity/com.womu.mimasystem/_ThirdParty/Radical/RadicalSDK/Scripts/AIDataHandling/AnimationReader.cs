using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimationReader
{
    public static readonly float s_defalutAvgFPS = 28.86f;
    public static List<AIFrame> ReadAnimationFile(string animationPath,float videoFrameRate = -1, bool decryptFile = true)
    {
        SavedAnimation savedAnimation;
        try
        {
            string jsonStr = System.IO.File.ReadAllText(animationPath);
            savedAnimation = Newtonsoft.Json.JsonConvert.DeserializeObject<SavedAnimation>(jsonStr);
        }
        catch
        {
            throw new Exception();
        }
        if(videoFrameRate > 0)
        {
            savedAnimation.meta_data.avg_FPS = videoFrameRate;
        }
        else if (savedAnimation.meta_data.avg_FPS <= 0)
        {
            savedAnimation.meta_data.avg_FPS = s_defalutAvgFPS;
        }
        List<AIFrame> res = new List<AIFrame>(savedAnimation.animation.Count);
        float timestamp = 0.0f;
        foreach (var frameInfo in savedAnimation.animation)
        {
            List<float> rawData = new List<float>();
            foreach (var arr in frameInfo.frame_data)
            {
                foreach (var val in arr.Value)
                {
                    rawData.Add(val);
                }
            }
            if (savedAnimation.meta_data.has_timestamps)
            {
                timestamp = frameInfo.frame_meta.timestamp;
            }
            else
            {
                timestamp += 1.0f / savedAnimation.meta_data.avg_FPS;
            }
            res.Add(AIDataHandlingTools.CreateAIFrameFromRawData(rawData.ToArray(), timestamp));
        }
        return res;
    }

    public static List<AIFrame> ReadAnimationFile(string jsonStr, Vector3 rootOffset, float videoFrameRate = -1, bool decryptFile = true)
    {
        SavedAnimation savedAnimation;
        try
        {
            savedAnimation = Newtonsoft.Json.JsonConvert.DeserializeObject<SavedAnimation>(jsonStr);
        }
        catch(Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }
        if (videoFrameRate > 0)
        {
            savedAnimation.meta_data.avg_FPS = videoFrameRate;
        }
        else if (savedAnimation.meta_data.avg_FPS <= 0)
        {
            savedAnimation.meta_data.avg_FPS = s_defalutAvgFPS;
        }
        List<AIFrame> res = new List<AIFrame>(savedAnimation.animation.Count);
        float timestamp = 0.0f;
        foreach (var frameInfo in savedAnimation.animation)
        {
            List<float> rawData = new List<float>();
            foreach (var arr in frameInfo.frame_data)
            {
                foreach (var val in arr.Value)
                {
                    rawData.Add(val);
                }
            }
            if (savedAnimation.meta_data.has_timestamps)
            {
                timestamp = frameInfo.frame_meta.timestamp;
            }
            else
            {
                timestamp += 1.0f / savedAnimation.meta_data.avg_FPS;
            }
            res.Add(AIDataHandlingTools.CreateAIFrameFromRawData(rawData.ToArray(), timestamp));

            for (int i = 0; i < res.Count; i++)
            {
                res[i].rootPosition = rootOffset;
            }
        }
        return res;
    }

    public static List<AIFrame> ReadAnimationFile(SavedAnimation savedAnimation, Vector3 rootOffset, float videoFrameRate = -1, bool decryptFile = true)
    {
        try
        {
            if (videoFrameRate > 0)
            {
                savedAnimation.meta_data.avg_FPS = videoFrameRate;
            }
            else if (savedAnimation.meta_data.avg_FPS <= 0)
            {
                savedAnimation.meta_data.avg_FPS = s_defalutAvgFPS;
            }
            List<AIFrame> res = new List<AIFrame>(savedAnimation.animation.Count);
            float timestamp = 0.0f;
            foreach (var frameInfo in savedAnimation.animation)
            {
                List<float> rawData = new List<float>();
                foreach (var arr in frameInfo.frame_data)
                {
                    foreach (var val in arr.Value)
                    {
                        rawData.Add(val);
                    }
                }
                if (savedAnimation.meta_data.has_timestamps)
                {
                    timestamp = frameInfo.frame_meta.timestamp;
                }
                else
                {
                    timestamp += 1.0f / savedAnimation.meta_data.avg_FPS;
                }
                res.Add(AIDataHandlingTools.CreateAIFrameFromRawData(rawData.ToArray(), timestamp));

                for (int i = 0; i < res.Count; i++)
                {
                    res[i].rootPosition += rootOffset;
                }
            }

            return res;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
            throw;
        }

    }
}