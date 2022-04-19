using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class smooth_follow_pos : MonoBehaviour {
    public Transform target;
    public float damping = 2.0f;
    private Vector3 currentPos = new Vector3(0f, 0f, 0f);
    private Vector3 nextPos = new Vector3(0f, 0f, 0f);

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        nextPos = target.transform.position;

    }

    void LateUpdate()
    {
        currentPos = this.transform.position;

        //dampen position
        currentPos = Vector3.Lerp(currentPos, nextPos, damping * Time.deltaTime);

        //set new position
        this.transform.position = currentPos;
    }
}
