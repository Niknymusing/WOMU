using System;
using UnityEngine;
[Serializable]
public class Bone
{
    public string name;

    public Transform boneTransform;
    public bool enabled;
    public Quaternion defaultRotation;
    public Vector3 defaultEuler;

    [Header("Debugging")] public Vector3 angleOutput;
    public Quaternion quaternionOutput;

    public Bone(Transform transform, Transform boneTransform, bool active)
    {
        this.boneTransform = boneTransform;
        defaultRotation = boneTransform != null ? boneTransform.localRotation : Quaternion.identity;
        if (boneTransform)
        {
            defaultEuler = boneTransform.localEulerAngles;
            name = boneTransform.name;
        }

        enabled = active && boneTransform != null;

    }
}