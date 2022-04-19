using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class object_rotator : MonoBehaviour {
    public int channel = 0;
    public int audio_component = 1;
    public bool continuousRotate = false;
    public float offset = 0f;
    public float speed = 1f;
    public float damping = 2f;

    private float tempY;
    private float RY;
	
	// Update is called once per frame
	void Update () {
        tempY = transform.localEulerAngles.y;		
	}

    void LateUpdate()
    {
        RY = MathS.LerpUnclamped(tempY, (OSC_channels.OSCch_data[channel, audio_component] * speed + offset), damping * Time.deltaTime);
        if (continuousRotate == true)
        {
            RY = OSC_channels.OSCch_data[channel, audio_component] * speed;
            transform.Rotate(new Vector3(0, RY, 0));
        }
        else
        {
            transform.localEulerAngles = new Vector3(0, RY, 0);
        }

    }
}
