using System;
using UnityEngine;

namespace MIMA.OSCCharacterControl
{
    public class MIMA_MediaPipeLandmark : MonoBehaviour
    {
        public float GizmoSize = 1.0f;
        public Color GizmoColor = Color.yellow;
        public void OnDrawGizmosSelected()
        {
            Gizmos.color = GizmoColor;
            Gizmos.DrawWireSphere(transform.position, GizmoSize);
        }

        
    }
}