using UnityEngine;
using System.Collections;

public class anim_ctrl : MonoBehaviour {

    private Animator anim;
    private float tempTime;

    // Use this for initialization
    void Start () {

        anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        tempTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;
        if (tempTime >= 1.0f)
        {
            anim.enabled = false;
        }
    }
}
