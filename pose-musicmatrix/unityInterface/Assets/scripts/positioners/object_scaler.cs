using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class object_scaler : MonoBehaviour {
    public int channel = 0;
    public int audio_component = 1;
    public Vector3 scaleMag = new Vector3(1f, 1f, 1f);
    public Vector3 scaleOffset = new Vector3(0f, 0f, 0f);
    public float damping = 2f;

    private Vector3 tempScale = new Vector3(0f, 0f, 0f);
    private Vector3 finalScale = new Vector3(0f, 0f, 0f);

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        tempScale = this.transform.localScale;
		
	}

    private void LateUpdate()
    {
        finalScale.x = Mathf.LerpUnclamped(tempScale.x, (OSC_channels.OSCch_data[channel, audio_component] * scaleMag.x + scaleOffset.x), damping * Time.deltaTime);
        finalScale.y = Mathf.LerpUnclamped(tempScale.y, (OSC_channels.OSCch_data[channel, audio_component] * scaleMag.y + scaleOffset.y), damping * Time.deltaTime);
        finalScale.z = Mathf.LerpUnclamped(tempScale.z, (OSC_channels.OSCch_data[channel, audio_component] * scaleMag.z + scaleOffset.z), damping * Time.deltaTime);

        transform.localScale = finalScale;
    }
}
