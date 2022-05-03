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

        private int emitterPositionID = Shader.PropertyToID("positionOffset_position");

     

        // Update is called once per frame
        new void Update()
        {
            base.Update();
            // every frame, update the emitter position to a random one of the landmarks
            
        }
    }

}

