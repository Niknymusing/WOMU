using System.Collections;
using System.Collections.Generic;
using MIMA;
using UnityEngine;

public class MIMA_RadicalWSCharacter : MIMA_CharacterPoseControlBase
{
    public SkinnedMeshRenderer smr;

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
    }
    
    


}
