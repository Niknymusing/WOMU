using System.Collections;
using System.Collections.Generic;
using MIMA;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MIMA_RadicalWSCharacter : MIMA_CharacterPoseControlBase
{
    public SkinnedMeshRenderer smr;
    public AnimationPlaybackManager radicalPlaybackManager;

    public Transform smrRootBone;

    /// <summary>
    /// Here we use current Client ID in place of RoomID
    /// </summary>
    private string _currentClientID;

    void Start()
    {
        base.Start();

        StartCoroutine(FindSMR());

    }

    IEnumerator FindSMR()
    {
        while (smr == null)
        {
            smr = GetComponentInChildren<SkinnedMeshRenderer>();
            yield return null;
        }

        Debug.Log("Found SMR " + smr.name + " on character object");
        vfx.SetSkinnedMeshRenderer("targetSkinnedMeshRenderer", smr);
        int numVerts = smr.sharedMesh.vertexCount;
        Debug.Log("Number of verts : " + numVerts);
        vfx.SetInt("targetVertexCount", numVerts);
        
        vfx.transform.SetParent(smr.rootBone);
        vfx.transform.localPosition = Vector3.zero;
        vfx.transform.localRotation = Quaternion.identity;
        vfx.transform.localScale = Vector3.one;
        
    }

    new void Update()
    {
        base.Update();

        if (clientID != _currentClientID)
        {
            _currentClientID = clientID;
            radicalPlaybackManager.roomName = _currentClientID;
            radicalPlaybackManager.DisconnectFromRoom();
            radicalPlaybackManager.ConnectToWSS();
        }
        
    }
    
    


}
