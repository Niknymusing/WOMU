﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MIMA_OSCServer : MonoBehaviour
{
    public OSC osc;
    public OSC.OSCStatus currentStatus = OSC.OSCStatus.Initial;
    public bool debugMode = true;
    public Action<string, object[]> OnMessage;
    
    private void Start()
    {
        
    }

    public void StartOSCServer(int portIn, int portOut)
    {
        
        osc.inPort = portIn;
        osc.outPort = portOut;
        
        osc.InitializeOSC();
        osc.SetAllMessageHandler(OnOSCMessage);
        osc.OnStatusChange += status =>
        {
            currentStatus = status;
        };
    }


    public void SendMessage(string address, object[] args)
    {
        if (debugMode) Debug.Log($"Sending to {address}");
        var msg = CreateMessage(address, args);
        if (osc != null && currentStatus != OSC.OSCStatus.Error)
        {
            osc.Send(msg);
        }
    }

    public OscMessage CreateMessage(string address, object[] args)
    {
        var argList = new ArrayList(args);
        var msg = new OscMessage();
        msg.address = address;
        msg.values = argList;
        return msg;
    }

    public void SendMessages(List<OscMessage> messages)
    {
        if (debugMode) Debug.Log($"Sending {messages.Count} messages");
        var msgList = new ArrayList(messages);
        SendMessages(messages);
    }
    
    public void SendMessages(ArrayList messages)
    {
        if (osc != null && currentStatus != OSC.OSCStatus.Error)
        {   
            osc.Send(messages);
        }
    }

    private void OnOSCMessage(OscMessage msg)
    {
        Debug.Log($"OSC Message {msg.address}");
        if (OnMessage != null)
        {
            OnMessage.Invoke(msg.address, msg.values.ToArray());
        }
    }
}