using System.Collections.Generic;
using UnityEngine;

public static partial class AIDataHandlingTools
{
    
    public static readonly int s_numberOf3x2Joints = 26;
    public static readonly int s_lengthOf3DRawDataArray = 3 +  4 * s_numberOf3x2Joints;


    public static readonly List<Quaternion> kPreRotations3x2 = new List<Quaternion>()
    {
         new Quaternion(4.030417E-19f, 1.110216E-16f, -1.301043E-18f,1f),
         new Quaternion(0f, 0f, 0f,1f),
         new Quaternion(-0.4283147f, 0.4939117f, -0.5563718f,0.5128822f),
         new Quaternion(-0.04993252f, -0.003204805f, 0.06396444f,0.9966971f),
         new Quaternion(-0.03107698f, -0.03775379f, -0.07834869f,0.995726f),
         new Quaternion(0f, 0f, 0f,1f),
         new Quaternion(0.5128822f, 0.5563718f, 0.4939117f,0.4283147f),
         new Quaternion(-0.04993252f, -0.003204805f, 0.06396444f,0.9966971f),
         new Quaternion(-0.03107698f, -0.03775379f, -0.07834869f,0.995726f),
         new Quaternion(0f, 0f, 0f,1f),
         new Quaternion(0.517247f, -0.4821364f, 0.517247f,-0.4821364f),
         new Quaternion(-5.21E-33f, -2.38997E-18f, -0.03903118f,0.999238f),
         new Quaternion(2.910698E-32f, 1.315149E-19f, 0.002147801f,0.9999977f),
         new Quaternion(0f, 0f, 0f,1f),
         new Quaternion(-2.112198E-32f, 1.065002E-17f, 0.1739281f,0.9847584f),
         new Quaternion(-5.551115E-17f, 5.551115E-17f, -0.1140702f,0.9934727f),
         new Quaternion(0f, 0f, 0f,1f),
         new Quaternion(0.731616f, 0.2010817f, 0.6383029f,0.1298982f),
         new Quaternion(0.01948894f, 0.08264948f, 0.2286845f,0.96979f),
         new Quaternion(4.235165E-22f, -2.073571E-25f, 0f,1f),
         new Quaternion(-0.7044159f, 0.06162841f, 0.06162843f,0.7044162f),
         new Quaternion(0f, 0f, 0f,1f),
         new Quaternion(0.2010817f, -0.731616f, -0.1298982f,0.6383029f),
         new Quaternion(0.01948894f, 0.08264948f, 0.2286845f,0.96979f),
         new Quaternion(4.235165E-22f, 3.134022E-25f, -1.29247E-26f,1f),
         new Quaternion(-0.7044142f, 0.06162856f, 0.06162828f,0.7044179f),
    };

 
    [SerializeField] public static bool applayPreRotation = true;



    public static AIFrame CreateAIFrameFromRawData(float[] data3d, float timestamp = 0)
    {
        if (data3d.Length != s_lengthOf3DRawDataArray)
        {
            Debug.Log("Wrong 3d raw data");
            return null;
        }
        AIFrame res = new AIFrame(); ;

        int convertBefore = -1;
        int convertAfter = 1;


        res.rootPosition = new Vector3(-data3d[0], data3d[1], data3d[2]); //conver to left handed


        int index = 3;
        var preRots = kPreRotations3x2;
        for (int i = 0; i < s_numberOf3x2Joints; i++)
        {
            Quaternion rotation = new Quaternion(
                    data3d[index + 1],
                    convertBefore * data3d[index + 2],
                    convertBefore * data3d[index + 3],
                    data3d[index + 0]);

            var finale = rotation;
            if (applayPreRotation)
            {
                finale = preRots[i] * rotation;
            }
            finale.y *= convertAfter;
            finale.z *= convertAfter;
            res.rotations.Add(finale);
            index += 4;
        }
        res.Timestamp = timestamp;
        return res;
    }
}