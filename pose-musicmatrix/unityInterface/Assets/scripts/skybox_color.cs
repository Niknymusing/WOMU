using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skybox_color : MonoBehaviour
{
    public string animClipName;
    private Animation anim;
    public Color currentColor;
    public Color finalColor;
    public Material skybox_mat;
    public Material expansion_mat;
    public GameObject discExpander;
    public float t = 0f;
    private float hue2, sat2, val2 = 0f;
    private Color tmpColor;
    private float hueShift;
    
    void Start()
    {
        anim = GetComponent<Animation>();
    }

    void Update(){
        if (Input.GetKeyDown("space")){
            StartCoroutine(ColorUpdate());
            discExpander.SetActive(true);
        }
    }
    public IEnumerator ColorUpdate() 
    {
        currentColor = skybox_mat.GetColor("_Tint");
        finalColor = expansion_mat.GetColor("_Color");
        anim[animClipName].speed = 0.35f;
        anim.Play(animClipName);
		yield return WaitForAnim(animClipName);
    }

    IEnumerator WaitForAnim(string animClipName)
    {
		
        yield return null;
        //Debug.Log(tempTime);
        while (anim.IsPlaying(animClipName)){

            // convert from RGB to HSV
            Color.RGBToHSV(currentColor, out float hue, out float sat, out float val);
            Color.RGBToHSV(finalColor, out hue2, out sat2, out val2);

            hueShift = Mathf.Lerp(hue, hue2, t);
            skybox_mat.SetColor("_Tint", Color.HSVToRGB(hueShift, sat2, 0.4f));
            yield return null;
        }
        discExpander.SetActive(false);
        expansion_mat.SetColor("_Color", Color.HSVToRGB(Random.Range(0f, 1f), sat2, val2));
        yield return null;
    }
}
