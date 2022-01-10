using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HumanoidRigData", menuName = "Data/HumanoidRig")]
public class HumanoidRigData : ScriptableObject
{
    public string rootBone;

    public string spineBone0;
    public string spineBone1;
    public string spineBone2;
    public string spineBone3;

    public string neckBone0;
    public string neckBone1;
    public string headBone;

    public string rightShoulderBone0;
    public string rightShoulderBone1;
    public string rightArmBone;
    public string rightForearmBone;
    public string rightHandBone;

    public string leftShoulderBone0;
    public string leftShoulderBone1;
    public string leftArmBone;
    public string leftForearmBone;
    public string leftHandBone;

    public string rightUpperLegBone0;
    public string rightUpperLegBone1;
    public string rightLegBone;
    public string rightFootBone;

    public string leftUpperLegBone0;
    public string leftUpperLegBone1;
    public string leftLegBone;
    public string leftFootBone;

    public string[] BoneNames
    {
        get
        {
            return new string[]
            {
                rootBone,

                spineBone0,
                spineBone1,
                spineBone2,
                spineBone3,

                neckBone0,
                neckBone1,
                headBone,

                rightShoulderBone0,
                rightShoulderBone1,
                rightArmBone,
                rightForearmBone,
                rightHandBone,

                leftShoulderBone0,
                leftShoulderBone1,
                leftArmBone,
                leftForearmBone,
                leftHandBone,

                rightUpperLegBone0,
                rightUpperLegBone1,
                rightLegBone,
                rightFootBone,

                leftUpperLegBone0,
                leftUpperLegBone1,
                leftLegBone,
                leftFootBone
            };
        }
    }

    public string[] BoneNames_RadicalOrdered
    {
        get
        {
            return new string[]
            {
                rootBone,

                leftUpperLegBone0,
                leftUpperLegBone1,
                leftLegBone,
                leftFootBone,

                rightUpperLegBone0,
                rightUpperLegBone1,
                rightLegBone,
                rightFootBone,

                spineBone0,
                spineBone1,
                spineBone2,
                spineBone3,

                neckBone0,
                neckBone1,
                headBone,

                rightShoulderBone0,
                rightShoulderBone1,
                rightArmBone,
                rightForearmBone,
                rightHandBone,

                leftShoulderBone0,
                leftShoulderBone1,
                leftArmBone,
                leftForearmBone,
                leftHandBone,

            };
        }
    }

}
