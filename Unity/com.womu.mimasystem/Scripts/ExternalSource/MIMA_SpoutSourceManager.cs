using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
using Klak.Spout;
#endif

using System;

public class MIMA_SpoutSourceManager : MIMA_ExternalSourceManagerBase
{
    #if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    public static MIMA_SpoutSourceManager Instance
    {
        get { return _instance;  }
    }

    private static MIMA_SpoutSourceManager _instance;

    private List<string> sourceList = new List<string>();
    public override List<string> GetExternalSources()
    {
        #if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        sourceList.Clear();
        sourceList.AddRange(SpoutManager.GetSourceNames());
        sourceList.AddRange(BuiltInTextureSources);
        return sourceList;
#endif

#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            var list = Plugin_CreateServerList();
            var count = Plugin_GetServerListCount(list);
            sourceList.Clear();

            if (count == 0){
                return new string[0];
            }
            else
            {
                for (var i = 0; i < count; i++)
                {
                    var pName = Plugin_GetNameFromServerList(list, i);
                    var pAppName = Plugin_GetAppNameFromServerList(list, i);
                    
                    var name = (pName != IntPtr.Zero) ? Marshal.PtrToStringAnsi(pName) : "(no name)";
                    var appName = (pAppName != IntPtr.Zero) ? Marshal.PtrToStringAnsi(pAppName) : "(no app name)";
                    
                    sourceList.Add(name + "~" + appName);
                }
            }
            Plugin_DestroyServerList(list);
            sourceList.AddRange(BuiltInTextureSources);
            return sourceList;

#endif

    }
    
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    public List<SpoutReceiver> receivers = new List<SpoutReceiver>();
    private Dictionary<SpoutReceiver, RenderTexture> renderTextures = new Dictionary<SpoutReceiver, RenderTexture>();
#endif
    
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
    public List<SyphonClient> receivers = new List<SyphonClient>();
    private Dictionary<SyphonClient, RenderTexture> renderTextures = new Dictionary<SyphonClient, RenderTexture>();
#endif
    

    public float SourceUpdateIntervalSeconds = 1.0f;

    public GameObject spoutReceiverPrefab;
    

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

        var r = receivers.FindLast(s => s.sourceName == source);
        if (r != null) return r.targetTexture;
        else
        {
            Debug.LogError($"ERROR - could not find texture for {source}");
            return Texture2D.blackTexture;
        }
    }

    
    void FixedUpdate()
    {
        
        

    }

    IEnumerator SourceUpdateRoutine()
    {
        while (this.enabled)
        {
            yield return new WaitForSeconds(SourceUpdateIntervalSeconds);
            
            // ensure we have receivers for all sources
            var sources = GetExternalSources();

            bool didSourcesChange = false;
        
            // remove all sources
            foreach (var r in receivers)
            {
                if (!sources.Contains(r.sourceName))
                {
                    Debug.Log($"removing old source {r.sourceName}");
                    Destroy(r.gameObject);
                    renderTextures.Remove(r);
                    receivers.Remove(r);
                    didSourcesChange = true;
                }
            }
        
            // create new ones
            foreach (var s in sources)
            {
                // don't create receiver for built in sources
                if (BuiltInTextureSources.Contains(s)) continue;
                
                
                if (!receivers.Exists(r => r.sourceName == s))
                {
                
#if UNITY_STANDALONE_WIN
                    var newReceiverGO = Instantiate(spoutReceiverPrefab, Vector3.zero, Quaternion.identity, transform);
                    var newReceiver = newReceiverGO.GetComponent<SpoutReceiver>();
                    newReceiver.sourceName = s;
#else
                    var newReceiverGO = Instantiate(syphonReceiverPrefab, Vector3.zero, Quaternion.identity, transform);
                    var newReceiver = newReceiverGO.GetComponent<SyphonClient>();
#endif
                    
                    
                    didSourcesChange = true;
                    receivers.Add(newReceiver);
                    var rt = new RenderTexture(1920, 1080, 0);
                    newReceiver.targetTexture = rt;
                    renderTextures.Add(newReceiver, rt);
                    Debug.Log($"created new receiver for {s}");
                }
            }
        
            if (didSourcesChange && sourcesChanged != null) sourcesChanged.Invoke();
            
            
            
        }
        yield return null;
    }
#else 
    void Start() { this.gameObject.SetActive(false);}
    private void OnValidate()
    {
        this.gameObject.SetActive(false);
        Debug.LogWarning("Deactivating Spout on unsupported platform");
    }


#endif

}
