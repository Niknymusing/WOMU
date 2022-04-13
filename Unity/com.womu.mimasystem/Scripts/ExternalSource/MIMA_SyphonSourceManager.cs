using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Klak.Syphon;
using System;
using System.Runtime.InteropServices;


public class MIMA_SyphonSourceManager : MIMA_ExternalSourceManagerBase
{
    public static MIMA_SyphonSourceManager Instance
    {
        get { return _instance;  }
    }

    private static MIMA_SyphonSourceManager _instance;
    
     
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
//#if true
    

// Struct for holding info about syphon sources, because they annoyingly have both server + app name
    struct SyphonSourceStruct
    {
        public string serverName;
        public string appName;
        public string AsSingleString
        {
            get { return serverName + "~" + appName; }
        }

        public SyphonSourceStruct(string s, string a)
        {
            serverName = s;
            appName = a;
        }
    }

    private List<SyphonSourceStruct> syphonSources = new List<SyphonSourceStruct>();
    
    // class for holding a reference to the actual Syphon Client object, and the render texture it points to
    class SyphonClientReference
    {
        public SyphonClient client;
        public SyphonSourceStruct sourceInfo;
        public RenderTexture rt;
    }
    private List<SyphonClientReference> receivers = new List<SyphonClientReference>();
   

    public override List<string> GetExternalSources()
    {
        var list = syphonSources.ConvertAll(s => s.AsSingleString);
        list.AddRange(BuiltInTextureSources);

        return list;
    }

    public float SourceUpdateIntervalSeconds = 1.0f;
    
    public GameObject syphonReceiverPrefab;

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogError("ERROR - there should only be one instance of this class");
        }

        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SourceUpdateRoutine());
    }

    public override Texture GetTextureForSource(string source)
    {
        if (BuiltInTextureSources.Contains(source)) return GetBuiltInTexture(source);
        
        string[] parts = source.Split('~');
        string serverName = parts[0];
        string appName = parts[1];

        var r = receivers.FindLast(s => s.sourceInfo.AsSingleString == source);
        if (r != null) return r.rt;
        else
        {
            Debug.LogError($"ERROR - could not find texture for {source}");
            return Texture2D.blackTexture;
        }
    }


    IEnumerator SourceUpdateRoutine()
    {
        while (this.enabled)
        {
            yield return new WaitForSeconds(SourceUpdateIntervalSeconds);
            
            
            UpdateSyphonSourcesFromPlugin();
            
            // ensure we have receivers for all sources
            
            bool didSourcesChange = false;
        
            // remove all sources
            foreach (var r in receivers)
            {   
                if (syphonSources.FindAll(s => s.AsSingleString == r.sourceInfo.AsSingleString).Count == 0)
                {
                    Debug.Log($"removing old source {r.sourceInfo.AsSingleString}");
                    Destroy(r.client.gameObject);
                    r.rt.Release();
                    receivers.Remove(r);
                    didSourcesChange = true;
                }
            }
        
            // create new ones
            foreach (var s in syphonSources)
            {
                // don't create receiver for built in sources
                if (BuiltInTextureSources.Contains(s)) continue;

                if (!receivers.Exists(r => r.sourceInfo.AsSingleString == s.AsSingleString))
                {
                    var newReceiverGO = Instantiate(syphonReceiverPrefab, Vector3.zero, Quaternion.identity, transform);
                    var newClient = newReceiverGO.GetComponent<SyphonClient>();
                    newClient.appName = s.appName;
                    newClient.serverName = s.serverName;
                    
                    didSourcesChange = true;

                    var newRef = new SyphonClientReference();
                    newRef.client = newClient;
                    newRef.rt = new RenderTexture(1920, 1080, 0);
                    newClient.targetTexture = newRef.rt;
                    newRef.sourceInfo = s;
                    
                    receivers.Add(newRef);
                    
                    Debug.Log($"created new receiver for {s.AsSingleString}");
                }
            }
        
            if (didSourcesChange && sourcesChanged != null) sourcesChanged.Invoke();
            
            
            
        }
        yield return null;
    }

    private void UpdateSyphonSourcesFromPlugin()
    {
        // UPDATING SYPHON SOURCES FROM PLUGIN
        syphonSources.Clear();
            
        var list = Plugin_CreateServerList();
        var count = Plugin_GetServerListCount(list);
            

        if (count == 0)
        {
            Debug.Log("No syphon sources found");
        }
        else
        {
            for (var i = 0; i < count; i++)
            {
                var pName = Plugin_GetNameFromServerList(list, i);
                var pAppName = Plugin_GetAppNameFromServerList(list, i);
                    
                var name = (pName != IntPtr.Zero) ? Marshal.PtrToStringAnsi(pName) : "(no name)";
                var appName = (pAppName != IntPtr.Zero) ? Marshal.PtrToStringAnsi(pAppName) : "(no app name)";
                    
                syphonSources.Add(new SyphonSourceStruct(name, appName));
                    
            }
        }
        Plugin_DestroyServerList(list);
            
        // END UPDATE
    }
        
        #region Syphon plugin entry points

        [DllImport("KlakSyphon")]
        static extern IntPtr Plugin_CreateServerList();

        [DllImport("KlakSyphon")]
        static extern void Plugin_DestroyServerList(IntPtr list);

        [DllImport("KlakSyphon")]
        static extern int Plugin_GetServerListCount(IntPtr list);

        [DllImport("KlakSyphon")]
        static extern IntPtr Plugin_GetNameFromServerList(IntPtr list, int index);

        [DllImport("KlakSyphon")]
        static extern IntPtr Plugin_GetAppNameFromServerList(IntPtr list, int index);

        #endregion

#else
    void Start() { this.gameObject.SetActive(false);}
    private void OnValidate()
    {
        this.gameObject.SetActive(false);
        Debug.LogWarning("Deactivating Syphon on unsupported platform");
    }
#endif
}
