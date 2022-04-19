using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class object_rotator2 : MonoBehaviour {
    public int channel = 0;
    public int audio_component = 1;
    public bool continuousRotate = false;
    public Vector3 offset = new Vector3(1f,1f,1f);
    public Vector3 speed = new Vector3(1f, 1f, 1f);
    public float damping = 2f;

    private Vector3 tempRot = new Vector3(0f, 0f, 0f);
    private Vector3 finalRot = new Vector3(0f, 0f, 0f);

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        tempRot = this.transform.localEulerAngles;
    }

    private void LateUpdate()
    {
        finalRot.x = Mathf.LerpAngle(tempRot.x, (OSC_channels.OSCch_data[channel, audio_component] * speed.x + offset.x), damping * Time.deltaTime);
        finalRot.y = Mathf.LerpAngle(tempRot.y, (OSC_channels.OSCch_data[channel, audio_component] * speed.y + offset.y), damping * Time.deltaTime);
        finalRot.z = Mathf.LerpAngle(tempRot.z, (OSC_channels.OSCch_data[channel, audio_component] * speed.z + offset.z), damping * Time.deltaTime);

        transform.localEulerAngles = finalRot;
    }
}
