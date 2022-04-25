using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

namespace MIMA
{
    public class MIMA_Effect : MonoBehaviour
    {
        public VisualEffect vfx;

 
        [Serializable]
        public class EffectParameter
        {
            public string name;
            public Type valueType;

            private int _id = -1;

            public int id
            {
                get
                {
                    if (_id == -1) _id = Shader.PropertyToID(name);
                    return _id;
                }
            }
        }

        public Dictionary<string, EffectParameter> parameters = new  Dictionary<string, EffectParameter>();

        private void OnValidate()
        {
            UpdateParamsList();
        }

        public void Start()
        {
            UpdateParamsList();
            
        }

        internal void UpdateParamsList()
        {
            parameters.Clear();
            if (vfx.visualEffectAsset != null)
            {
                var plist = new List<VFXExposedProperty>();
                vfx.visualEffectAsset.GetExposedProperties(plist);
                foreach (var p in plist)
                {
                    var np = new EffectParameter();
                    np.name = p.name;
                    np.valueType = p.type;
                    parameters.Add(p.name, np);
                }
            }
        }

        public void SetFloatParam(string name, float value)
        {
            if (parameters.ContainsKey(name))
            {
                var p = parameters[name];
                if (p.valueType == typeof(float)) vfx.SetFloat(p.id, value);
                else Debug.LogError($"Error - parameter {name} is not a float");
            }
            else
            {
                Debug.LogError($"Error - no parameter {name} found in vfx graph");
            }
        }
        
        public void SetIntParam(string name, int value)
        {
            if (parameters.ContainsKey(name))
            {
                var p = parameters[name];
                if (p.valueType == typeof(int)) vfx.SetInt(p.id, value);
                else if (p.valueType == typeof(uint)) vfx.SetUInt(p.id, (uint)value);
                else Debug.LogError($"Error - parameter {name} is not an int");
            }
            else
            {
                Debug.LogError($"Error - no parameter {name} found in vfx graph");
            }
        }
        
        public void SetVectorParam(string name, Vector3 value)
        {
            if (parameters.ContainsKey(name))
            {
                var p = parameters[name];
                if (p.valueType == typeof(Vector2)) vfx.SetVector2(p.id, new Vector2(value.x, value.y));
                else if (p.valueType == typeof(Vector3)) vfx.SetVector3(p.id, value);
                else Debug.LogError($"Error - parameter {name} is not a Vector2 or 3");
            }
            else
            {
                Debug.LogError($"Error - no parameter {name} found in vfx graph");
            }
        }
        
        public void SetBooleanParam(string name, bool value)
        {
            if (parameters.ContainsKey(name))
            {
                var p = parameters[name];
                if (p.valueType == typeof(bool)) vfx.SetBool(p.id, value);
                else Debug.LogError($"Error - parameter {name} is not a boolean");
            }
            else
            {
                Debug.LogError($"Error - no parameter {name} found in vfx graph");
            }
        }

        
        

        private void Update()
        {
            
        }
    }
}