using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityOSC;


public class MIMA_OSCManager : MonoBehaviour
{
    public OSCClient oscClient;
    public OSCServer oscServer;
    
    public bool debugMode = true;
    public Action<string, object[]> OnMessage;

    private bool oscSending = false;
    private bool oscReceiving = false;
    
    private void Start()
    {
        
    }


    public bool IsSending
    {
        get { return oscSending;  }
    }

    public void StartOSCSending(string ip, int portOut)
    {
        oscSending = true;

        Debug.Log($"Creating new OSC Client at {ip} : {portOut}");
        oscClient = new OSCClient(IPAddress.Parse(ip), portOut);
        oscClient.Connect();
           
      
    }

    public void StartOSCReceiving(int portIn)
    {
        oscReceiving = true;
        
        oscServer = new OSCServer(portIn);
        
        oscServer.PacketReceivedEvent += (server, packet) =>
        {
            if (OnMessage != null) OnMessage.Invoke(packet.Address, packet.Data.ToArray());
        };
    }

    public void StopOsc()
    {
        if (oscClient != null) oscClient.Close();
        if (oscServer != null) oscServer.Close();
        oscSending = false;
        oscReceiving = false;
    }


    public void SendMessage(string address, object[] args)
    {
        if (debugMode) Debug.Log($"Sending to {address}");
        oscClient.Send(CreateMessage(address, args));
    }

    public static OSCMessage CreateMessage(string address, object[] args)
    {
        var msg = new OSCMessage(address);
        foreach (var d in args)
        {
            msg.Append(d);
        }
        return msg;
    }

    public void SendMessages(OSCMessage[] messages)
    {
        if (debugMode) Debug.Log($"Sending {messages.Length} messages");
        foreach (var message in messages)
        {
            oscClient.Send(message);
        }
        
    }

}