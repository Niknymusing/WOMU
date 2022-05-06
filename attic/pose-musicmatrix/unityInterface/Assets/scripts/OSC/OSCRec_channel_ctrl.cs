using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSCRec_channel_ctrl : MonoBehaviour {
    public string RemoteIP = "127.0.0.1";
    public int SendToPort = 57131;
    public int ListenerPort = 7000;
    public int channel = 0;
    public string OSC_address = "/CH0";
    public bool enableDamping = false;
    public float dampingMultiplier = 1.0f;

    private Osc handler;
    private UDPPacketIO udp;
    private float damping = 2.0f;
    private float[] tmpOSC = new float[7];

    // Use this for initialization
    void Start () {
        //DontDestroyOnLoad(this);
        udp = this.GetComponent<UDPPacketIO>();
        udp.init(RemoteIP, SendToPort, ListenerPort);
        handler = this.GetComponent<Osc>();
        handler.init(udp);

        handler.SetAddressHandler(OSC_address, ListenEvent);
    }
	
	public void ListenEvent(OscMessage oscMessage)
    {
        tmpOSC[0] = (float)oscMessage.Values[0]; 
        tmpOSC[1] = (float)oscMessage.Values[1]; 
        tmpOSC[2] = (float)oscMessage.Values[2]; 
        tmpOSC[3] = (float)oscMessage.Values[3]; 
        tmpOSC[4] = (float)oscMessage.Values[4]; 
        tmpOSC[5] = (float)oscMessage.Values[5]; 
        tmpOSC[6] = (float)oscMessage.Values[6];

    }

    private void LateUpdate()
    {
        //Shifting all OSC data to late update for a smooth transition
        if (enableDamping == true)
        {
            
            damping = tmpOSC[1] * dampingMultiplier;
            
            OSC_channels.OSCch_data[channel, 0] = MathS.LerpUnclamped(OSC_channels.OSCch_data[channel, 0], tmpOSC[0], damping * Time.deltaTime);
            
            OSC_channels.OSCch_data[channel, 2] = MathS.LerpUnclamped(OSC_channels.OSCch_data[channel, 2], tmpOSC[2], damping * Time.deltaTime);
            
            OSC_channels.OSCch_data[channel, 3] = MathS.LerpUnclamped(OSC_channels.OSCch_data[channel, 3], tmpOSC[3], damping * Time.deltaTime);
            
            OSC_channels.OSCch_data[channel, 4] = MathS.LerpUnclamped(OSC_channels.OSCch_data[channel, 4], tmpOSC[4], damping * Time.deltaTime);
            
            OSC_channels.OSCch_data[channel, 5] = MathS.LerpUnclamped(OSC_channels.OSCch_data[channel, 5], tmpOSC[5], damping * Time.deltaTime);
            
            OSC_channels.OSCch_data[channel, 6] = MathS.LerpUnclamped(OSC_channels.OSCch_data[channel, 6], tmpOSC[6], damping * Time.deltaTime);
        }
        else
        {
            OSC_channels.OSCch_data[channel, 0] = tmpOSC[0];
            OSC_channels.OSCch_data[channel, 1] = tmpOSC[1];
            OSC_channels.OSCch_data[channel, 2] = tmpOSC[2];
            OSC_channels.OSCch_data[channel, 3] = tmpOSC[3];
            OSC_channels.OSCch_data[channel, 4] = tmpOSC[4];
            OSC_channels.OSCch_data[channel, 5] = tmpOSC[5];
            OSC_channels.OSCch_data[channel, 6] = tmpOSC[6];
        }
    }
}
