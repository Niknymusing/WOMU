using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skybox_dimmer : MonoBehaviour {
	public int channel = 0;
	public int audio_component = 1;
	public float exposureOffset = 0.3f;
	public float exposureMagnitude = 1f;

	public float damping = 1f;
	private float exposure = 0f;
	private float tmpExp;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		tmpExp = RenderSettings.skybox.GetFloat("_Exposure");
	}

	void LateUpdate(){
		float nextExp = (OSC_channels.OSCch_data[channel,audio_component] * exposureMagnitude) + exposureOffset;
		exposure = Mathf.LerpUnclamped(tmpExp, nextExp, damping * Time.deltaTime);
		RenderSettings.skybox.SetFloat("_Exposure", exposure);
		RenderSettings.ambientIntensity = exposure;
	}
}
