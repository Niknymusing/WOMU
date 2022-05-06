using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class smooth_follow_v2 : MonoBehaviour {
    public Transform target;
	public int channel = 0;
	public int audio_component = 2;
    public float damping = 2.0f;

	public Vector3 positionMagnitude = new Vector3(0f,0f,0f);
	public Vector3 positionOffset = new Vector3(0f,0f,0f);
	public bool audioInX = false;
	public bool audioInY = false;
	public bool audioInZ = false;
	private Vector3 tmpPosMag;
	
    private Vector3 currentPos = new Vector3(0f, 0f, 0f);
    private Vector3 nextPos = new Vector3(0f, 0f, 0f);

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		tmpPosMag = positionMagnitude;

		if(audioInX == true){
			tmpPosMag.x *= OSC_channels.OSCch_data[channel,audio_component];
		}
		if(audioInY == true){
			tmpPosMag.y *= OSC_channels.OSCch_data[channel,audio_component];
		}
		if(audioInZ == true){
			tmpPosMag.z *= OSC_channels.OSCch_data[channel,audio_component];
		}

		currentPos = this.transform.position;
		nextPos = Vector3.Scale((target.transform.position + positionOffset), tmpPosMag);

    }

    void LateUpdate()
    {
        

        //dampen position
        currentPos = Vector3.Lerp(currentPos, nextPos, damping * Time.deltaTime);

        //set new position
        this.transform.position = currentPos;
    }
}
