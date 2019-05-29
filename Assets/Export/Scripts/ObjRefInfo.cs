using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjRefInfo : ScriptableObject
{

    public List<GameObject> gameObjectRef;
    public List<ObjInfo> objRef;
	public List<Material> mats;

}
[Serializable]
public struct ObjInfo
{
    public Vector2[] vestices;

    public Vector2[] uvs;
    public int matIndex;
    public ObjInfo(Vector2[] v, Vector2[] uv, int matindex)
    {
        vestices = v;
        uvs = uv;
        matIndex = matindex;
    }
}