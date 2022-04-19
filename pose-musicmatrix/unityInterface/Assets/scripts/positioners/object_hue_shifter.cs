using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class object_hue_shifter : MonoBehaviour {
    public Color assetColor;
    public int channel = 0;
    public int audio_component = 1;

    public Vector3 HSVVariance = new Vector3(0f,0f,0f);
    public string shaderAttributeName = "_ReflectColor";

    public bool hueInput = false;
    public bool satInput = false;
    public bool valInput = false;
    public float damping = 2f;
    private float H,S,V;
    private Color finalColor;
    private Color setColor;
    private Renderer rend;
	// Use this for initialization
	void Start () {
		rend = GetComponent<Renderer> ();
	}
	
	// Update is called once per frame
	void Update () {
        Color.RGBToHSV(assetColor, out H, out S, out V);
        if(hueInput == true){
            H += OSC_channels.OSCch_data[channel, audio_component] * HSVVariance.x;
        }
        if(satInput == true){
            S += OSC_channels.OSCch_data[channel, audio_component] * HSVVariance.y;
        }
        if(valInput == true){
            V += OSC_channels.OSCch_data[channel, audio_component] * HSVVariance.z;
        }
        //H = Mathf.Clamp01(hsbColor.h);
        //S = Mathf.Clamp01(hsbColor.s);
        //V = Mathf.Clamp01(hsbColor.v);

        finalColor = Color.HSVToRGB(H,S,V,true);
        
    }

    void LateUpdate (){
        setColor = Color.Lerp(assetColor, finalColor, damping * Time.deltaTime);
        rend.material.SetColor(shaderAttributeName, setColor);
    }
}
