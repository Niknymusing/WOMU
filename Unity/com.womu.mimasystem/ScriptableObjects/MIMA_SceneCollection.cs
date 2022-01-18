using System.Collections.Generic;
using UnityEngine;

namespace MIMA
{
    [CreateAssetMenu(fileName = "NewSceneCollection", menuName = "MIMA/New Scene Collection", order = 0)]
    public class MIMA_SceneCollection : ScriptableObject
    {
        public List<MIMA_Scene> scenes = new List<MIMA_Scene>();
    }
}