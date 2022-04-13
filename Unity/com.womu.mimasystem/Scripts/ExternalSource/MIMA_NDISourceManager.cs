using System.Collections.Generic;
using UnityEngine;

namespace MIMA
{
    public class MIMA_NDISourceManager : MIMA_ExternalSourceManagerBase
    {

        public static MIMA_NDISourceManager Instance
        {
            get { return _instance; }
        }

        private static MIMA_NDISourceManager _instance;

        
        
        private void Awake()
        {
            if (_instance != null)
            {
                Debug.LogError("ERROR - there should only be one instance of this class");
            }

            _instance = this;
        }


        public override List<string> GetExternalSources()
        {


            return new List<string>();

        }
    }
}