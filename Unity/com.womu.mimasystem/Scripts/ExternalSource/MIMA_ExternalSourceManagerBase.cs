using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
using Klak.Spout;
#endif
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
using Klak.Syphon;
#endif
using System;

public class MIMA_ExternalSourceManagerBase : MonoBehaviour
{
    public virtual List<string> GetExternalSources()
    {
        return new List<string>();
    }
    
    public Action sourcesChanged;


    public virtual Texture GetTextureForSource(string source)
    {
        return Texture2D.blackTexture;
    }
    
}
