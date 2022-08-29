using System.Collections.Generic;
using UnityEngine;


public partial class RawAIFrameData
{
    public RawAIFrameData() { }

    public float[] pose3dData;
    public float timestamp;
}

[System.Serializable]
public partial class AIFrame
{

    public AIFrame()
    {
        rotations = new List<Quaternion>();
    }
    public AIFrame(Vector3 rootPosition, List<Quaternion> rotations)
    {
        this.rootPosition = rootPosition;
        this.rotations = rotations;
    }

    public Vector3 rootPosition;
    public List<Quaternion> rotations;
    public float Timestamp;

}
