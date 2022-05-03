using System.Collections;
using System.Collections.Generic;
using MergetoolGui;
using MIMA;
using UnityEngine;

namespace MIMA
{
    public class MIMA_CharacterParticleEffect : MIMA_Effect
    {
        public MIMA_CharacterPoseControlMediaPipe controller;

        private int emitterPositionID = Shader.PropertyToID("positionOffset");

     

        // Update is called once per frame
        new void Update()
        {
            base.Update();
            // every frame, update the emitter position to a random one of the landmarks
            int val = Random.Range(0, 24);
            
            Vector3 emitterPos = Vector3.zero;

            if (val == 0) emitterPos = controller.Nose.position;
            if (val == 1) emitterPos = controller.LeftEye.position;
            if (val == 2) emitterPos = controller.RightEye.position;
            if (val == 3) emitterPos = controller.LeftShoulder.position;
            if (val == 4) emitterPos = controller.RightShoulder.position;
            if (val == 5) emitterPos = controller.LeftElbow.position;
            if (val == 6) emitterPos = controller.RightElbow.position;
            if (val == 7) emitterPos = controller.LeftWrist.position;
            if (val == 8) emitterPos = controller.RightWrist.position;
            if (val == 9) emitterPos = controller.LeftPinky.position;
            if (val == 10) emitterPos = controller.RightPinky.position;
            if (val == 11) emitterPos = controller.LeftIndex.position;
            if (val == 12) emitterPos = controller.RightIndex.position;
            if (val == 13) emitterPos = controller.LeftThumb.position;
            if (val == 14) emitterPos = controller.RightThumb.position;
            if (val == 15) emitterPos = controller.LeftHip.position;
            if (val == 16) emitterPos = controller.RightHip.position;
            if (val == 17) emitterPos = controller.LeftKnee.position;
            if (val == 18) emitterPos = controller.RightKnee.position;
            if (val == 19) emitterPos = controller.LeftAnkle.position;
            if (val == 20) emitterPos = controller.RightAnkle.position;
            if (val == 21) emitterPos = controller.LeftHeel.position;
            if (val == 22) emitterPos = controller.RightHeel.position;
            if (val == 23) emitterPos = controller.LeftFootIndex.position;
            if (val == 24) emitterPos = controller.RightFootIndex.position;
            
            vfx.SetVector3(emitterPositionID, emitterPos);
        }
    }

}

