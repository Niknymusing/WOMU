using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class WrapperDLL : MonoBehaviour
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void MyCallbackDelegate(string msg);
    private MyCallbackDelegate callback;

    public bool isConnected = false;

    [DllImport("Wrapper")]
    public static extern void Connect(string uri, string userToken, string room);

    [DllImport("Wrapper")]
    public static extern void Disconnect();

    [DllImport("Wrapper")]
    public static extern void DataReceivedCallback(MyCallbackDelegate callbackMsg);


    [SerializeField] public string uriServer;
    [SerializeField] public string userToken;
    [SerializeField] public string room;

    [SerializeField] private SimulationServer _simulateServer;

    private void Start()
    {
       
    }

    public void DoConnect()
    {
        callback = CallbackServer;
        DataReceivedCallback(callback);

        isConnected = true;
        Connect(uriServer, userToken, room);
    }

    private void CallbackServer(string msg)
    {
        try
        {
            Debug.Log(msg);
            if (!msg.Contains("Connected"))
                _simulateServer.ReadServerData(msg);

        }
        catch (Exception)
        {
            return;
        }
    }

    public void DisconnectServer()
    {
        if (isConnected) Disconnect();
        isConnected = false;
    }

    private void OnApplicationQuit()
    {
        if (isConnected) DisconnectServer();
    }
}
