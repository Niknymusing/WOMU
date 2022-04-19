using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class light_intensity : MonoBehaviour {
    public int channel = 0;
    public int audio_component = 2;
    public float intensityAmp = 1.0f;
    public float intensityOffset = 1.0f;
    public bool enableControl = false;
    public float damping = 2.0f;

    private Light lgt;
    private float tmpIntensity;
    private float nextIntensity;
    private float finalIntensity;

    // Use this for initialization
    void Start () {
        lgt = this.GetComponent<Light>();
	}
	
	// Update is called once per frame
	void Update () {
        tmpIntensity = lgt.intensity;		
	}

    private void LateUpdate()
    {
        nextIntensity = OSC_channels.OSCch_data[channel, audio_component] * intensityAmp + intensityOffset;

        if(enableControl == true)
        {
            //nextIntensity *= OSC_control.OSCctrl[2];
        }

        finalIntensity = Mathf.LerpUnclamped(tmpIntensity, nextIntensity, damping * Time.deltaTime);
        lgt.intensity = finalIntensity;
    }
}
