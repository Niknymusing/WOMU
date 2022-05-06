using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class position_riser : MonoBehaviour {
    public int channel = 0;
    public int audio_component = 2;
    public float speed = 1.0f;
    public float offset = 0f;
    public float damping = 2.0f;

    private float tempY = 0f;
    private float finalY = 0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        tempY = this.transform.localPosition.y;		
	}

    private void LateUpdate()
    {
        finalY = Mathf.LerpUnclamped(tempY, OSC_channels.OSCch_data[channel, audio_component] * speed + offset, damping * Time.deltaTime);
        this.transform.localPosition = new Vector3(0f, finalY, 0f);
    }
}
