using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISkeleton : MonoBehaviour
{
    [SerializeField] public string prefix;
    [SerializeField] public HumanoidRigData rigData;
    [SerializeField] private Transform root;
    [SerializeField] Bone[] bones;

    public Vector3 rootPos;
    static string[] gen32Keys
    {
        get
        {
            return new string[]
            {
                "root_r",

                "SpineDummy_r",
                "Spine_r",
                "Spine1_r",
                "Spine2_r",

                "NeckDummy_r",
                "Neck_r",
                "Head_r",

                "RightShoulderDummy_r",
                "RightShoulder_r",
                "RightArm_r",
                "RightForeArm_r",
                "RightHand_r",

                "LeftShoulderDummy_r",
                "LeftShoulder_r",
                "LeftArm_r",
                "LeftForeArm_r",
                "LeftHand_r",

                "RightUpLegDummy_r",
                "RightUpLeg_r",
                "RightLeg_r",
                "RightFoot_r",

                "LeftUpLegDummy_r",
                "LeftUpLeg_r",
                "LeftLeg_r",
                "LeftFoot_r",
            };
        }
    }

    static string[] gen30Keys
    {
        get
        {
            return new string[]
            {
                "root_r",

                "", //"SpineDummy_r",
                "lowerback_r",
                "upperback_r",
                "thorax_r",

                "lowerneck_r",
                "upperneck_r",
                "head_r",

                "rclavicle_r",
                "rhumerus_r",
                "rradius_r",
                "rwrist_r",
                "rhand_r",

                "lclavicle_r",
                "lhumerus_r",
                "lradius_r",
                "lwrist_r",
                "lhand_r",

                "rhipjoint_r",
                "rfemur_r",
                "rtibia_r",
                "rfoot_r",

                "lhipjoint_r",
                "lfemur_r",
                "ltibia_r",
                "lfoot_r",
            };
        }
    }

    public static string[] gen30FromReader
    {
        get
        {
            return new string[]
            {
                "root_r",

                "lhipjoint_r",
                "lfemur_r",
                "ltibia_r",
                "lfoot_r",

                "rhipjoint_r",
                "rfemur_r",
                "rtibia_r",
                "rfoot_r",

                "",//"SpineDummy_r",
                "lowerback_r",
                "upperback_r",
                "thorax_r",

                "lowerneck_r",
                "upperneck_r",
                "head_r",

                "lclavicle_r",
                "lhumerus_r",
                "lradius_r",
                "lwrist_r",
                "",//"lhand_r",

                "rclavicle_r",
                "rhumerus_r",
                "rradius_r",
                "rwrist_r",
                "",//"rhand_r",
            };
        }
    }
    public static string[] gen32FromReader
    {
        get
        {
            return new string[]
            {
                "root_r",               //"root_r", 
                                        //
                "LeftUpLegDummy_r",     //"lhipjoint_r",
                "LeftUpLeg_r",          //"lfemur_r",
                "LeftLeg_r",            //"ltibia_r",
                "LeftFoot_r",           //"lfoot_r",
                                        //
                "RightUpLegDummy_r",    //"rhipjoint_r",
                "RightUpLeg_r",         //"rfemur_r",
                "RightLeg_r",           //"rtibia_r",
                "RightFoot_r",          //"rfoot_r",
                                        //
                "SpineDummy_r",         //"",//"SpineDummy_r",
                "Spine_r",              //"lowerback_r",
                "Spine1_r",             //"upperback_r",
                "Spine2_r",             //"thorax_r",
                                        //
                "NeckDummy_r",          //"lowerneck_r",
                "Neck_r",               //"upperneck_r",
                "Head_r",               //"head_r",
                                        //
                "RightShoulderDummy_r", //"lclavicle_r",
                "RightShoulder_r",      //"lhumerus_r",
                "RightArm_r",           //"lradius_r",
                "RightForeArm_r",       //"lwrist_r",
                "RightHand_r",          //"",//"lhand_r",
                                        //
                "LeftShoulderDummy_r",  //"rclavicle_r",
                "LeftShoulder_r",       //"rhumerus_r",
                "LeftArm_r",            //"rradius_r",
                "LeftForeArm_r",        //"rwrist_r",
                "LeftHand_r",           //"",//"rhand_r",
            };
        }
    }

    public void SetupBones()
    {
        var boneList = new List<Bone>();
        string nn = gameObject.name;
        var radicalNames = GetReaderNames(AIPlayer.Instance.Type);
        var boneNames = new List<string>(GetNames(AIPlayer.Instance.Type));
        var rigBoneNames = rigData.BoneNames;
        var transforms = transform.GetComponentsInChildren<Transform>();

        for (int i = 0; i < radicalNames.Length; i++)
        {
            string radName = radicalNames[i];
            int index = boneNames.IndexOf(radName);
            string boneName = rigBoneNames[index];
            Transform bone = transforms.FirstOrDefault(t => t.name == prefix + boneName);
            if (bone != null)
            {
                Bone currentBone = new Bone(transform, bone, true);
                boneList.Add(currentBone);
            }
        }

        bones = boneList.ToArray();
    }

    public static string[] GetNames(RigType type)
    {
        switch (type)
        {
            case RigType.Gen3x0:
                return gen30Keys;
            case RigType.Gen3x2:
                return gen32Keys;
        }

        return new string[0];
    }
    public static string[] GetReaderNames(RigType type)
    {
        switch (type)
        {
            case RigType.Gen3x0:
                return gen30FromReader;
            case RigType.Gen3x2:
                return gen32FromReader;
        }

        return new string[0];
    }


    public void UpdateFromOnPrem(AIFrame aIFrame, bool useRootMotion)
    {
        if (bones.Length == 0) SetupBones();
        if(float.IsNaN(aIFrame.rootPosition.x) || float.IsNaN(aIFrame.rootPosition.y) || float.IsNaN(aIFrame.rootPosition.z))
        {
            return;
        }
        if (useRootMotion)
        {
            bones[0].boneTransform.localPosition = aIFrame.rootPosition;
        }

        for (int i = 0; i < bones.Length; i++)
        {
            if (bones[i].boneTransform != null)
                bones[i].boneTransform.localRotation = aIFrame.rotations[i];
            else
            {
                print("null");
            }

        }
    }
}