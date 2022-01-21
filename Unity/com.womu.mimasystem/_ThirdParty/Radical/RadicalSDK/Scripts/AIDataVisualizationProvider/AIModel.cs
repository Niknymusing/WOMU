using System.Linq;
using UnityEngine;

public class AIModel : MonoBehaviour
{
    public Avatar avatar;
    public Transform pivot;
    public Transform rootJoint;
    public Transform hipJoint;

    public Transform footReference;
    public Transform headReference;

    Vector3 defaultPosition;

    public Vector3 defaultInputPosition;
    public float floorDistance;

    private void Reset()
    {
        var anim = GetComponent<Animator>();
        if (anim) avatar = anim.avatar;
    }

    public void SetDefaultPosition(Vector3 position)
    {
        pivot.position = position;
        defaultPosition = pivot.localPosition;
    }

    public void SetInputStart(Vector3 input)
    {
        defaultInputPosition = input;
    }


    public void ResetToDefault()
    {
        //pivot.localPosition = defaultPosition;
    }


    public void SetupAuto()
    {
        var anim = transform.GetComponentInChildren<Animator>();
        avatar = anim.avatar;

        var transforms = GetComponentsInChildren<Transform>();
        var hipNames = new string[] { "Hips", "hip", "root", "center", "pelvis" };
        Transform hips = null;
        for (int i = 0; i < hipNames.Length; i++)
        {
            hips = transforms.FirstOrDefault(t => t.name.ToLower().Contains(hipNames[i]));
            if (hips != null) break;
        }

        pivot = transform;
        hipJoint = hips;
        if (hips == null)
        {
            Debug.Log("Setup failed at " + name);
            enabled = false;
        }
    }
}