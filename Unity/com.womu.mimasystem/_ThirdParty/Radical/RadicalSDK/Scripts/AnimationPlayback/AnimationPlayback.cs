using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationPlayback : MonoBehaviour
{
    public delegate void OnUpdateTimeStamp(int frame);

    List<AIFrame> animationInfo = null;
    Coroutine playbackRoutine;
    private int _currentFrame;
    

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

    public void StartPlayback(int frame)
    {
        if (animationInfo == null)
            return;

        if (playbackRoutine != null)
            StopCoroutine(playbackRoutine);

        playbackRoutine = StartCoroutine(PlaybackRoutine(frame));
    }



    public IEnumerator PlaybackRoutine(int frame)
    {
        float initTime = Time.time;

        int frameInd = frame;
        while (frameInd < animationInfo.Count)
        {
            Debug.Log($"{animationInfo[_currentFrame].Timestamp}; {Time.time}; {initTime};");
            if (Time.time - initTime + animationInfo[_currentFrame].Timestamp >= animationInfo[frameInd].Timestamp)
            {
                AIPlayer.Instance.SampleFromValues("0", animationInfo[frameInd]);
                frameInd+=1;
            }
            yield return null;
        }
        _currentFrame = 0;
    }

    public void StopPlayBack()
    {
        if (playbackRoutine != null)
            StopCoroutine(playbackRoutine);
    }
}
