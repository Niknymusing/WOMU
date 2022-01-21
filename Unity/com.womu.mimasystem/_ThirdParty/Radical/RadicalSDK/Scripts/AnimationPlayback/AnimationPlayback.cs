using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;


public class AnimationPlayback : MonoBehaviour
{
    public delegate void OnUpdateTimeStamp(int frame);

    List<AIFrame> animationInfo = null;
    List<AIFrame> liveAnimationInfo = new List<AIFrame>();
    AIFrame currentLiveFrame = null;
    Coroutine playbackRoutine;
    Coroutine livePlaybackRoutine;
    private int _currentFrame;
    public int activeCurrentFrame;
    
    // 

    private void Awake()
    {
        //PlaybackController.OnPauseHandler += OnPauseHandler;
        //PlaybackController.OnPlayHandler += OnPlayHandler;
    }

    private void OnPlayHandler(int frame)
    {
        StartPlayback(frame);
    }

    private void OnPauseHandler(int frame)
    {
        _currentFrame = frame;
        StopAllCoroutines();
    }

    public int GetFrameCount(){
        return animationInfo?.Count ?? 0;
    }

    public void AssignAnimation(List<AIFrame> animation)
    {
        this.animationInfo = animation;
        FixTimestamps();
    }
    private void FixTimestamps()
    {
        for (int i = 0; i < animationInfo.Count; i++)
            if (animationInfo[i].Timestamp == 0)
                animationInfo[i].Timestamp = 1.0f * i / 30;
    }

    public void StartPlaybackLive(int frame)
    {
        if (livePlaybackRoutine != null)
            StopCoroutine(livePlaybackRoutine);

        livePlaybackRoutine = StartCoroutine(LivePlaybackRoutine(frame));
    }

    public void StartPlayback(int frame)
    {
       if (animationInfo == null)
          return;

       if (playbackRoutine != null)
          StopCoroutine(playbackRoutine);

        playbackRoutine = StartCoroutine(PlaybackRoutine(frame));
    }

    public void playLiveFrame(IList<JsonElement> jsonData)
    {
        AIFrame aiframe = AnimationReader.ReadAnimationData(jsonData);
        if (aiframe != null)
            currentLiveFrame = aiframe;
    }

    public IEnumerator PlaybackRoutine(int frame)
    {
        float initTime = Time.time;

        int frameInd = frame;
        while (frameInd < animationInfo.Count)
        {
            if (Time.time - initTime + animationInfo[_currentFrame].Timestamp >= animationInfo[frameInd].Timestamp)
            {
                AIPlayer.Instance.SampleFromValues(animationInfo[frameInd]);
                frameInd+=1;
                activeCurrentFrame = frameInd;
            }
            yield return null;
        }
        _currentFrame = 0;
    }

    public IEnumerator LivePlaybackRoutine(int frame)
    {
        while (true)
        {
            if (currentLiveFrame != null) AIPlayer.Instance.SampleFromValues(currentLiveFrame);
            yield return null;
        }
    }

    public void StopPlayBack()
    {
        if (playbackRoutine != null)
            StopCoroutine(playbackRoutine);
    }
}
