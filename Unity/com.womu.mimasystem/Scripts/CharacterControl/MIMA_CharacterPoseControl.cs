using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MIMA_CharacterPoseControl : MonoBehaviour
{
    // matches Mixamo skeleton layout
    public string name_rootBone = "Hips";

    public string name_spineBone0 = "Spine";
    public string name_spineBone1 = "Spine1";
    public string name_spineBone2 = "Spine2";

    public string name_neckBone = "Neck";
    public string name_headBone = "Head";

    public string name_rightShoulderBone = "RightShoulder";
    public string name_rightArmBone = "RightArm";
    public string name_rightForearmBone = "RightForeArm";
    public string name_rightHandBone = "RightHand";

    public string name_leftShoulderBone = "LeftShoulder";
    public string name_leftArmBone = "LeftArm";
    public string name_leftForearmBone = "LeftForeArm";
    public string name_leftHandBone = "LeftHand";

    public string name_rightLegUpBone = "RightUpLeg";
    public string name_rightLegBone = "RightLeg";
    public string name_rightFootBone = "RightFoot";

    public string name_leftLegUpBone = "LeftUpLeg";
    public string name_leftLegBone = "LeftLeg";
    public string name_leftFootBone = "LeftFoot";

    private Transform transform_rootBone;

    private Transform transform_spineBone0;
    private Transform transform_spineBone1;
    private Transform transform_spineBone2;

    private Transform transform_neckBone;
    private Transform transform_headBone;

    private Transform transform_rightShoulderBone;
    private Transform transform_rightArmBone;
    private Transform transform_rightForearmBone;
    private Transform transform_rightHandBone;

    private Transform transform_leftShoulderBone;
    private Transform transform_leftArmBone;
    private Transform transform_leftForearmBone;
    private Transform transform_leftHandBone;

    private Transform transform_rightLegUpBone;
    private Transform transform_rightLegBone;
    private Transform transform_rightFootBone;

    private Transform transform_leftLegUpBone;
    private Transform transform_leftLegBone;
    private Transform transform_leftFootBone;

    [Serializable]
    public class SkeletonFrame
    {
        public Vector3 rootPosition;
        public Quaternion rootRotation;

        public Quaternion rot_spineBone0;
        public Quaternion rot_spineBone1;
        public Quaternion rot_spineBone2;

        public Quaternion rot_neckBone;
        public Quaternion rot_headBone;

        public Quaternion rot_rightShoulderBone;
        public Quaternion rot_rightArmBone;
        public Quaternion rot_rightForearmBone;
        public Quaternion rot_rightHandBone;

        public Quaternion rot_leftShoulderBone;
        public Quaternion rot_leftArmBone;
        public Quaternion rot_leftForearmBone;
        public Quaternion rot_leftHandBone;

        public Quaternion rot_rightLegUpBone;
        public Quaternion rot_rightLegBone;
        public Quaternion rot_rightFootBone;

        public Quaternion rot_leftLegUpBone;
        public Quaternion rot_leftLegBone;
        public Quaternion rot_leftFootBone;
    }

    public Action<SkeletonFrame> OnFrame;

    public float frameFPS = 60.0f;

    // Start is called before the first frame update
    void Start()
    {
        // Find transforms in children and assign
        var transforms = GetComponentsInChildren<Transform>();

        transform_rootBone = transforms.First(t => t.name.Contains(name_rootBone));

        // spine might be hard because multiple things called Spine[x]
        transform_spineBone0 = transforms.First(t => t.name.Contains(name_spineBone0));
        transform_spineBone1 = transforms.First(t => t.name.Contains(name_spineBone1));
        transform_spineBone2 = transforms.First(t => t.name.Contains(name_spineBone2));

        transform_neckBone = transforms.First(t => t.name.Contains(name_neckBone));
        transform_headBone = transforms.First(t => t.name.Contains(name_headBone));

        transform_rightShoulderBone = transforms.First(t => t.name.Contains(name_rightShoulderBone));
        transform_rightArmBone = transforms.First(t => t.name.Contains(name_rightArmBone));
        transform_rightForearmBone = transforms.First(t => t.name.Contains(name_rightForearmBone));
        transform_rightHandBone = transforms.First(t => t.name.Contains(name_rightHandBone));

        transform_leftShoulderBone = transforms.First(t => t.name.Contains(name_leftShoulderBone));
        transform_leftArmBone = transforms.First(t => t.name.Contains(name_leftArmBone));
        transform_leftForearmBone = transforms.First(t => t.name.Contains(name_leftForearmBone));
        transform_leftHandBone = transforms.First(t => t.name.Contains(name_leftHandBone));

        transform_rightLegUpBone = transforms.First(t => t.name.Contains(name_rightLegUpBone));
        transform_rightLegBone = transforms.First(t => t.name.Contains(name_rightLegBone));
        transform_rightFootBone = transforms.First(t => t.name.Contains(name_rightFootBone));

        transform_leftLegUpBone = transforms.First(t => t.name.Contains(name_leftLegUpBone));
        transform_leftLegBone = transforms.First(t => t.name.Contains(name_leftLegBone));
        transform_leftFootBone = transforms.First(t => t.name.Contains(name_leftFootBone));

        
    }

    public void StartExportFrames()
    {
        StartCoroutine(MakeFrameRoutine());
    }
    
    

    IEnumerator MakeFrameRoutine()
    {
        while (true)
        {
            var newFrame = new SkeletonFrame();
            newFrame.rootPosition = transform_rootBone.position;
            newFrame.rootRotation = transform_rootBone.rotation;

            newFrame.rot_spineBone0 = transform_spineBone0.localRotation;
            newFrame.rot_spineBone1 = transform_spineBone1.localRotation;
            newFrame.rot_spineBone2 = transform_spineBone2.localRotation;
            newFrame.rot_neckBone = transform_neckBone.localRotation;
            newFrame.rot_headBone = transform_headBone.localRotation;
            newFrame.rot_rightShoulderBone = transform_rightShoulderBone.localRotation;
            newFrame.rot_rightArmBone = transform_rightArmBone.localRotation;
            newFrame.rot_rightForearmBone = transform_rightForearmBone.localRotation;
            newFrame.rot_rightHandBone = transform_rightHandBone.localRotation;
            newFrame.rot_leftShoulderBone = transform_leftShoulderBone.localRotation;
            newFrame.rot_leftArmBone = transform_leftArmBone.localRotation;
            newFrame.rot_leftForearmBone = transform_leftForearmBone.localRotation;
            newFrame.rot_leftHandBone = transform_leftHandBone.localRotation;
            newFrame.rot_rightLegUpBone = transform_rightLegUpBone.localRotation;
            newFrame.rot_rightLegBone = transform_rightLegBone.localRotation;
            newFrame.rot_rightFootBone = transform_rightFootBone.localRotation;
            newFrame.rot_leftLegUpBone = transform_leftLegUpBone.localRotation;
            newFrame.rot_leftLegBone = transform_leftLegBone.localRotation;
            newFrame.rot_leftFootBone = transform_leftFootBone.localRotation;
        
            if (OnFrame != null) OnFrame.Invoke(newFrame);

            yield return new WaitForSeconds(60.0f / frameFPS);
        }
    }

    public void ApplyFrame(SkeletonFrame frame)
    {
        transform_rootBone.position = frame.rootPosition;
        transform_rootBone.rotation = frame.rootRotation;
        transform_spineBone0.localRotation = frame.rot_spineBone0;
        transform_spineBone1.localRotation = frame.rot_spineBone1;
        transform_spineBone2.localRotation = frame.rot_spineBone2;
        transform_neckBone.localRotation = frame.rot_neckBone;
        transform_headBone.localRotation = frame.rot_headBone;
        transform_rightShoulderBone.localRotation = frame.rot_rightShoulderBone;
        transform_rightArmBone.localRotation = frame.rot_rightArmBone;
        transform_rightForearmBone.localRotation = frame.rot_rightForearmBone;
        transform_rightHandBone.localRotation = frame.rot_rightHandBone;
        transform_leftShoulderBone.localRotation = frame.rot_leftShoulderBone;
        transform_leftArmBone.localRotation = frame.rot_leftArmBone;
        transform_leftForearmBone.localRotation = frame.rot_leftForearmBone;
        transform_leftHandBone.localRotation = frame.rot_leftHandBone;
        transform_rightLegUpBone.localRotation = frame.rot_rightLegUpBone;
        transform_rightLegBone.localRotation = frame.rot_rightLegBone;
        transform_rightFootBone.localRotation = frame.rot_rightFootBone;
        transform_leftLegUpBone.localRotation = frame.rot_leftLegUpBone;
        transform_leftLegBone.localRotation = frame.rot_leftLegBone;
        transform_leftFootBone.localRotation = frame.rot_leftFootBone;
    }

    public static OscMessage[] FrameToOsc(SkeletonFrame frame)
    {
        var messageDict = new Dictionary<string, object[]>();
        messageDict.Add("rootPosition", VectorToObjectArgs(frame.rootPosition));
        messageDict.Add("rootRotation", QuaternionToObjectArgs(frame.rootRotation));
        messageDict.Add("rot_spineBone0", QuaternionToObjectArgs(frame.rot_spineBone0));
        messageDict.Add("rot_spineBone1", QuaternionToObjectArgs(frame.rot_spineBone1));
        messageDict.Add("rot_spineBone2", QuaternionToObjectArgs(frame.rot_spineBone2));
        messageDict.Add("rot_neckBone", QuaternionToObjectArgs(frame.rot_neckBone));
        messageDict.Add("rot_headBone", QuaternionToObjectArgs(frame.rot_headBone));
        messageDict.Add("rot_rightShoulderBone", QuaternionToObjectArgs(frame.rot_rightShoulderBone));
        messageDict.Add("rot_rightArmBone", QuaternionToObjectArgs(frame.rot_rightArmBone));
        messageDict.Add("rot_rightForearmBone", QuaternionToObjectArgs(frame.rot_rightForearmBone));
        messageDict.Add("rot_rightHandBone", QuaternionToObjectArgs(frame.rot_rightHandBone));
        messageDict.Add("rot_leftShoulderBone", QuaternionToObjectArgs(frame.rot_leftShoulderBone));
        messageDict.Add("rot_leftArmBone", QuaternionToObjectArgs(frame.rot_leftArmBone));
        messageDict.Add("rot_leftForearmBone", QuaternionToObjectArgs(frame.rot_leftForearmBone));
        messageDict.Add("rot_leftHandBone", QuaternionToObjectArgs(frame.rot_leftHandBone));
        messageDict.Add("rot_rightLegUpBone", QuaternionToObjectArgs(frame.rot_rightLegUpBone));
        messageDict.Add("rot_rightLegBone", QuaternionToObjectArgs(frame.rot_rightLegBone));
        messageDict.Add("rot_rightFootBone", QuaternionToObjectArgs(frame.rot_rightFootBone));
        messageDict.Add("rot_leftLegUpBone", QuaternionToObjectArgs(frame.rot_leftLegUpBone));
        messageDict.Add("rot_leftLegBone", QuaternionToObjectArgs(frame.rot_leftLegBone));
        messageDict.Add("rot_leftFootBone", QuaternionToObjectArgs(frame.rot_leftFootBone));

        var messages = new List<OscMessage>();
        foreach (var k in messageDict.Keys)
        {
            var newMsg = new OscMessage();
            newMsg.address = k;
            newMsg.values = new ArrayList(messageDict[k]);
            messages.Add(newMsg);
        }

        return messages.ToArray();
    }

    public static SkeletonFrame OscToFrame(OscMessage[] messages)
    {
        var frame = new SkeletonFrame();

        frame.rootPosition = ObjectArgsToVector(messages.First(m => m.address == "rootPosition").values);
        frame.rootRotation = ObjectArgsToQuaternion(messages.First(m => m.address == "rootRotation").values);
        frame.rot_spineBone0 = ObjectArgsToQuaternion(messages.First(m => m.address == "rot_spineBone0").values);
        frame.rot_spineBone1 = ObjectArgsToQuaternion(messages.First(m => m.address == "rot_spineBone1").values);
        frame.rot_spineBone2 = ObjectArgsToQuaternion(messages.First(m => m.address == "rot_spineBone2").values);
        frame.rot_neckBone = ObjectArgsToQuaternion(messages.First(m => m.address == "rot_neckBone").values);
        frame.rot_headBone = ObjectArgsToQuaternion(messages.First(m => m.address == "rot_headBone").values);
        frame.rot_rightShoulderBone = ObjectArgsToQuaternion(messages.First(m => m.address == "rot_rightShoulderBone").values);
        frame.rot_rightArmBone = ObjectArgsToQuaternion(messages.First(m => m.address == "rot_rightArmBone").values);
        frame.rot_rightForearmBone = ObjectArgsToQuaternion(messages.First(m => m.address == "rot_rightForearmBone").values);
        frame.rot_rightHandBone = ObjectArgsToQuaternion(messages.First(m => m.address == "rot_rightHandBone").values);
        frame.rot_leftShoulderBone = ObjectArgsToQuaternion(messages.First(m => m.address == "rot_leftShoulderBone").values);
        frame.rot_leftArmBone = ObjectArgsToQuaternion(messages.First(m => m.address == "rot_leftArmBone").values);
        frame.rot_leftForearmBone = ObjectArgsToQuaternion(messages.First(m => m.address == "rot_leftForearmBone").values);
        frame.rot_leftHandBone = ObjectArgsToQuaternion(messages.First(m => m.address == "rot_leftHandBone").values);
        frame.rot_rightLegUpBone = ObjectArgsToQuaternion(messages.First(m => m.address == "rot_rightLegUpBone").values);
        frame.rot_rightLegBone = ObjectArgsToQuaternion(messages.First(m => m.address == "rot_rightLegBone").values);
        frame.rot_rightFootBone = ObjectArgsToQuaternion(messages.First(m => m.address == "rot_rightFootBone").values);
        frame.rot_leftLegUpBone = ObjectArgsToQuaternion(messages.First(m => m.address == "rot_leftLegUpBone").values);
        frame.rot_leftLegBone = ObjectArgsToQuaternion(messages.First(m => m.address == "rot_leftLegBone").values);
        frame.rot_leftFootBone = ObjectArgsToQuaternion(messages.First(m => m.address == "rot_leftFootBone").values);

        return frame;
    }

    private static object[] VectorToObjectArgs(Vector3 v)
    {
        return new object[] {v.x, v.y, v.z};
    }

    private static Vector3 ObjectArgsToVector(ArrayList args)
    {
        return new Vector3((float) args[0], (float) args[1], (float) args[2]);
    }

    private static object[] QuaternionToObjectArgs(Quaternion q)
    {
        return new object[] {q.x, q.y, q.z, q.w};
    }

    private static Quaternion ObjectArgsToQuaternion(ArrayList args)
    {
        return new Quaternion((float) args[0], (float) args[1], (float) args[2], (float) args[3]);
    }

    // Update is called once per frame
    void Update()
    {
    }
}