using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Klak.Ndi;

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
        
        void Start()
        {
            StartCoroutine(SourceUpdateRoutine());
        }
    
        public float SourceUpdateIntervalSeconds = 1.0f;
        public GameObject ndiReceiverPrefab;
        public List<NdiReceiver> receivers = new List<NdiReceiver>();

        IEnumerator SourceUpdateRoutine()
        {
            while (this.enabled)
            {
                yield return new WaitForSeconds(SourceUpdateIntervalSeconds);

                // ensure we have receivers for all sources
                var sources = GetExternalSources();

                bool didSourcesChange = false;

                // remove all prefabs for sources that don't exist anymore
                var removalList = receivers.Where(r => !sources.Contains(r.ndiName));
                foreach (var r in removalList)
                {
                    Debug.Log($"removing old source {r.ndiName}");
                    Destroy(r.gameObject);
                    receivers.Remove(r);
                    didSourcesChange = true;
                }

                // create new ones
                foreach (var s in sources)
                {
                    // don't create receiver for built in sources
                    if (BuiltInTextureSources.Contains(s)) continue;


                    if (!receivers.Exists(r => r.ndiName == s))
                    {
                        var newReceiverGO = Instantiate(ndiReceiverPrefab, Vector3.zero, Quaternion.identity,
                            transform);
                        var newReceiver = newReceiverGO.GetComponent<NdiReceiver>();
                        newReceiver.ndiName = s;

                        didSourcesChange = true;
                        receivers.Add(newReceiver);
                        Debug.Log($"created new receiver for {s}");
                    }
                }

                if (didSourcesChange && sourcesChanged != null) sourcesChanged.Invoke();
            }

            yield return null;
        }


        private List<string> sourceList = new List<string>();

        public override List<string> GetExternalSources()
        {
            sourceList.Clear();
            sourceList.AddRange(NdiFinder.sourceNames);
            return sourceList;
        }

        public override Texture GetTextureForSource(string source)
        {
            var r = receivers.Where(r => r.ndiName == source);
            if (r.Count() > 0)
            {
                return r.First().texture;
            }
            else
            {
                Debug.LogError($"ERROR - could not find texture for {source}");
                return Texture2D.blackTexture;
            }
        }
    }
}