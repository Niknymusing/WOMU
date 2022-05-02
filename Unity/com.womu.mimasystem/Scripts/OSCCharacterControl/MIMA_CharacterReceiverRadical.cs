using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;

public class MIMA_CharacterReceiverRadical : MonoBehaviour
{
    public MIMA_OSCManager OscManager;

    public int inPort;

    public int numMessagesReceived = 0;
    public int numFramesProcessed = 0;

    public bool debug = false;

    private bool HasNewFrame = false;
    
    public MIMA_CharacterPoseControlRadical characterController;

    // save this, and every time we get a root position, we update everything
    private Dictionary<string, object[]> oscFrameData = new Dictionary<string, object[]>();

    // Start is called before the first frame update
    void Start()
    {
        OscManager.StartOSCReceiving(inPort);
        OscManager.OnMessage += (s, objects) =>
        {
            if (debug) Debug.Log(s);
            
            numMessagesReceived++;
            if (oscFrameData.ContainsKey(s))
            {
                oscFrameData[s] = objects;
            }
            else
            {
                oscFrameData.Add(s, objects);
            }

            if (s == MIMA_CharacterPoseControlRadical.FRAME_KEY)
            {
                HasNewFrame = true;
            }
        };
    }

    private void Update()
    {
        if (HasNewFrame)
        {
            var frame = MIMA_CharacterPoseControlRadical.OscToFrame(oscFrameData);
            characterController.ApplyFrame(frame);
            numFramesProcessed++;
            HasNewFrame = false;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            characterController.gameObject.SetActive(characterController.gameObject.activeSelf);
        }
    }
}
