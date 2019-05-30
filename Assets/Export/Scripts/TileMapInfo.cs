using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapInfo : ScriptableObject
{

    public int mapIndex;
    public List<int> posIndex;
	[System.NonSerialized]
    public List<double> posY;
    public List<TileMapObjInfo> objInfoList;

    public Dictionary<int, TileMapObjInfo> mapInfoDic;

    public void Init()
    {
		if (mapInfoDic != null)
		{
			return;
		}
		mapInfoDic = new Dictionary<int, TileMapObjInfo>();
        for (int i = 0; i < posIndex.Count; i++)
        {
			mapInfoDic.Add(posIndex[i],objInfoList[i]);
        }
    }
}
[System.Serializable]
public struct TileMapObjInfo
{
    public int objIndex;
    public int objRotY;
	public TileMapObjInfo(int index, int rot){
		objIndex = index;
		objRotY = rot;
	}
}