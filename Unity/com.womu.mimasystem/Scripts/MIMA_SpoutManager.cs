using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Klak.Spout;
using System;

public class MIMA_SpoutManager : MonoBehaviour
{
    public static MIMA_SpoutManager Instance
    {
        get { return _instance;  }
    }

    private static MIMA_SpoutManager _instance;

    public static List<string> GetExternalSources()
    {
        return SpoutManager.GetSourceNames().ToList();
    }
    

    public List<SpoutReceiver> receivers = new List<SpoutReceiver>();
    private Dictionary<SpoutReceiver, RenderTexture> renderTextures = new Dictionary<SpoutReceiver, RenderTexture>();

    public Action sourcesChanged;

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
        
    }

    public Texture GetTextureForSource(string source)
    {
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
        // ensure we have receivers for all sources
        var sources = SpoutManager.GetSourceNames().ToList();

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
            if (!receivers.Exists(r => r.sourceName == s))
            {
                var newReceiverGO = Instantiate(spoutReceiverPrefab, Vector3.zero, Quaternion.identity, transform);
                var newReceiver = newReceiverGO.GetComponent<SpoutReceiver>();
                newReceiver.sourceName = s;
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
}
