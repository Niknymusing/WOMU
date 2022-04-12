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
    internal readonly List<string> BuiltInTextureSources = new List<string>() {"black", "white" };
    
    public virtual List<string> GetExternalSources()
    {
        return new List<string>() {"black", "white" };
    }
    
    public Action sourcesChanged;


    public virtual Texture GetTextureForSource(string source)
    {
        return Texture2D.blackTexture;
    }


    internal static Texture GetBuiltInTexture(string name)
    {
        switch (name)
        {
            case "white":
                return Texture2D.whiteTexture;
            case "black":
            default:
                return Texture2D.blackTexture;
        }
    }
}
