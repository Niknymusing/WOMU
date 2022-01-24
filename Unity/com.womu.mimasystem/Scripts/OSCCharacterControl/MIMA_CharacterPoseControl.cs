using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityOSC;

public class MIMA_CharacterPoseControl : MonoBehaviour
{
    public static string FRAME_KEY = "rootPosition";
    
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

    public Transform transform_rootBone;

    public Transform transform_spineBone0;
    public Transform transform_spineBone1;
    public Transform transform_spineBone2;

    public Transform transform_neckBone;
    public Transform transform_headBone;

    public Transform transform_rightShoulderBone;
    public Transform transform_rightArmBone;
    public Transform transform_rightForearmBone;
    public Transform transform_rightHandBone;

    public Transform transform_leftShoulderBone;
    public Transform transform_leftArmBone;
    public Transform transform_leftForearmBone;
    public Transform transform_leftHandBone;

    public Transform transform_rightLegUpBone;
    public Transform transform_rightLegBone;
    public Transform transform_rightFootBone;

    public Transform transform_leftLegUpBone;
    public Transform transform_leftLegBone;
    public Transform transform_leftFootBone;

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

    public float frameFPS = 30.0f;

    
    
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
        yield return new WaitWhile(()=> transform_rootBone == null);
        
        while (true)
        {
            var newFrame = new SkeletonFrame();
            newFrame.rootPosition = transform_rootBone.localPosition;
            newFrame.rootRotation = transform_rootBone.localRotation;

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

            yield return new WaitForSeconds(1.0f / frameFPS);
        }
    }

    public void ApplyFrame(SkeletonFrame frame)
    {
        transform_rootBone.localPosition = frame.rootPosition;
        transform_rootBone.localRotation = frame.rootRotation;
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

    public static OSCMessage[] FrameToOsc(SkeletonFrame frame)
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

        var messages = new List<OSCMessage>();
        foreach (var k in messageDict.Keys)
        {
            messages.Add(MIMA_OSCManager.CreateMessage(k, messageDict[k]));
        }

        return messages.ToArray();
    }

    public static SkeletonFrame OscToFrame(Dictionary<string, object[]> messages)
    {
        var frame = new SkeletonFrame();

        if (messages.ContainsKey("rootPosition")) frame.rootPosition = ObjectArgsToVector(messages["rootPosition"]);
        if (messages.ContainsKey("rootRotation")) frame.rootRotation = ObjectArgsToQuaternion(messages["rootRotation"]);
        if (messages.ContainsKey("rot_spineBone0")) frame.rot_spineBone0 = ObjectArgsToQuaternion(messages["rot_spineBone0"]);
        if (messages.ContainsKey("rot_spineBone1")) frame.rot_spineBone1 = ObjectArgsToQuaternion(messages["rot_spineBone1"]);
        if (messages.ContainsKey("rot_spineBone2")) frame.rot_spineBone2 = ObjectArgsToQuaternion(messages["rot_spineBone2"]);
        if (messages.ContainsKey("rot_neckBone")) frame.rot_neckBone = ObjectArgsToQuaternion(messages["rot_neckBone"]);
        if (messages.ContainsKey("rot_headBone")) frame.rot_headBone = ObjectArgsToQuaternion(messages["rot_headBone"]);
        if (messages.ContainsKey("rot_rightShoulderBone")) frame.rot_rightShoulderBone = ObjectArgsToQuaternion(messages["rot_rightShoulderBone"]);
        if (messages.ContainsKey("rot_rightArmBone")) frame.rot_rightArmBone = ObjectArgsToQuaternion(messages["rot_rightArmBone"]);
        if (messages.ContainsKey("rot_rightForearmBone")) frame.rot_rightForearmBone = ObjectArgsToQuaternion(messages["rot_rightForearmBone"]);
        if (messages.ContainsKey("rot_rightHandBone")) frame.rot_rightHandBone = ObjectArgsToQuaternion(messages["rot_rightHandBone"]);
        if (messages.ContainsKey("rot_leftShoulderBone")) frame.rot_leftShoulderBone = ObjectArgsToQuaternion(messages["rot_leftShoulderBone"]);
        if (messages.ContainsKey("rot_leftArmBone")) frame.rot_leftArmBone = ObjectArgsToQuaternion(messages["rot_leftArmBone"]);
        if (messages.ContainsKey("rot_leftForearmBone")) frame.rot_leftForearmBone = ObjectArgsToQuaternion(messages["rot_leftForearmBone"]);
        if (messages.ContainsKey("rot_leftHandBone")) frame.rot_leftHandBone = ObjectArgsToQuaternion(messages["rot_leftHandBone"]);
        if (messages.ContainsKey("rot_rightLegUpBone")) frame.rot_rightLegUpBone = ObjectArgsToQuaternion(messages["rot_rightLegUpBone"]);
        if (messages.ContainsKey("rot_rightLegBone")) frame.rot_rightLegBone = ObjectArgsToQuaternion(messages["rot_rightLegBone"]);
        if (messages.ContainsKey("rot_rightFootBone")) frame.rot_rightFootBone = ObjectArgsToQuaternion(messages["rot_rightFootBone"]);
        if (messages.ContainsKey("rot_leftLegUpBone")) frame.rot_leftLegUpBone = ObjectArgsToQuaternion(messages["rot_leftLegUpBone"]);
        if (messages.ContainsKey("rot_leftLegBone")) frame.rot_leftLegBone = ObjectArgsToQuaternion(messages["rot_leftLegBone"]);
        if (messages.ContainsKey("rot_leftFootBone")) frame.rot_leftFootBone = ObjectArgsToQuaternion(messages["rot_leftFootBone"]);

        return frame;
    }

    private static object[] VectorToObjectArgs(Vector3 v)
    {
        return new object[] {v.x, v.y, v.z};
    }

    private static Vector3 ObjectArgsToVector(object[] args)
    {
        return new Vector3((float) args[0], (float) args[1], (float) args[2]);
    }

    private static object[] QuaternionToObjectArgs(Quaternion q)
    {
        return new object[] {q.x, q.y, q.z, q.w};
    }

    private static Quaternion ObjectArgsToQuaternion(object[] args)
    {
        return new Quaternion((float) args[0], (float) args[1], (float) args[2], (float) args[3]);
    }

    // Update is called once per frame
    void Update()
    {
    }
}