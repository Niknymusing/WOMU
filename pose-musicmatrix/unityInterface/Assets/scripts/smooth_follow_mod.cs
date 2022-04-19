using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class smooth_follow_mod : MonoBehaviour
{
    public Transform target;
    public float damping = 2.0f;
    private Vector3 currentPos = new Vector3(0f, 0f, 0f);
    private Vector3 nextPos = new Vector3(0f, 0f, 0f);
    private float current_ry = 0f;
    private float next_ry = 0f;

    
    void Update()
    {
        nextPos = target.transform.position;
        next_ry = target.transform.localEulerAngles.y;
    }

    
    void LateUpdate()
    {
        currentPos = this.transform.position;
        current_ry = this.transform.localEulerAngles.y;

        //dampen position & ry
        currentPos = Vector3.Lerp(currentPos, nextPos, damping * Time.deltaTime);
        current_ry = Mathf.Lerp(current_ry, next_ry, damping * Time.deltaTime);

        //set new position & rotation
        this.transform.position = currentPos;
        this.transform.localEulerAngles = new Vector3(0f, current_ry, 0f);
    }
}
