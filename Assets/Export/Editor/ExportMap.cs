using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class ExportMap : Editor
{
    static string m_MapName = "Map_KOW";
    static Vector2Int m_MapSpiltSize = new Vector2Int(5,5);
    [MenuItem("Map/Export")]
    public static void Export()
    {
        //加载MapInfo
		Dictionary<string,List<GameObject>> layerObjDic;
        string mapInfo;
        LoadMap(m_MapName,out layerObjDic, out mapInfo);

        //加载全局设置信息
        var settingsInfoPath = AssetDatabase.GUIDToAssetPath(uteGLOBAL3dMapEditor.uteSettingstxt);
        StreamReader rd = new StreamReader(settingsInfoPath);
        string setting = rd.ReadToEnd();
        rd.Close();
        string[] infoSplited = setting.Split(':');
        var globalGridSizeX = System.Convert.ToInt32(infoSplited[3]);
        var globalGridSizeZ = System.Convert.ToInt32(infoSplited[4]);

        //计算map分块大小,创建分块map配置
        int map_w = globalGridSizeX / m_MapSpiltSize.x;
        int map_h = globalGridSizeZ / m_MapSpiltSize.y;
        TileMapInfo[,] mapList = new TileMapInfo[m_MapSpiltSize.x, m_MapSpiltSize.y];
        for (int w = 0; w < m_MapSpiltSize.x; w++)
        {
            for (int h = 0; h < m_MapSpiltSize.y; h++)
            {
                mapList[w, h] = GetStriptableObject<TileMapInfo>(string.Format("Assets/Map/MapInfo_{0}_{1}.asset", w, h));
                mapList[w, h].mapIndex = h * map_w + w;
                mapList[w, h].posIndex = new List<int>();
                mapList[w, h].objIndexList = new List<int>();
                mapList[w, h].objRotList = new List<int>();
            }
        }

        //创建地形obj配置
        ObjRefInfo objRefInfo = GetStriptableObject<ObjRefInfo>("Assets/Map/ObjRefInfo.asset");
        objRefInfo.objRef = new List<ObjInfo>();
        objRefInfo.mats = new List<Material>();
		var terrainObjList = layerObjDic["DEFAULT"];
        objRefInfo.gameObjectRef = terrainObjList;
        for (int i = 0; i < terrainObjList.Count; i++)
        {
            //顶点和UV
            MeshFilter meshFilter = terrainObjList[i].GetComponent<MeshFilter>();
            List<Vector3> v = new List<Vector3>();
            List<Vector2> v2 = new List<Vector2>();
            meshFilter.sharedMesh.GetVertices(v);
            for (int j = 0; j < v.Count; j++)
            {
                v2.Add(new Vector2(v[j].x, v[j].z));
            }
            List<Vector2> uvs = new List<Vector2>();
            meshFilter.sharedMesh.GetUVs(0, uvs);
            //材质
            MeshRenderer meshRender = terrainObjList[i].GetComponent<MeshRenderer>();
            Material mat = meshRender.sharedMaterial;
            int matIndex = 0;
            if (!objRefInfo.mats.Contains(mat))
            {
                matIndex = objRefInfo.mats.Count;
                objRefInfo.mats.Add(mat);
            }
            else
            {
                for (int j = 0; j < objRefInfo.mats.Count; j++)
                {
                    if (objRefInfo.mats[j].name == mat.name)
                    {
                        matIndex = j;
                    }
                }
            }
            objRefInfo.objRef.Add(new ObjInfo(v2.ToArray(), uvs.ToArray(), matIndex));
        }
        UnityEditor.EditorUtility.SetDirty(objRefInfo);

		//地形分块数据
        string[] myMapInfoAll = mapInfo.Split("$"[0]);
        for (int i = 0; i < myMapInfoAll.Length - 1; i++)
        {
            string[] myMapParts = myMapInfoAll[i].Split(":"[0]);
            string layerName = myMapParts[10].ToString();
			if (layerName == "DEFAULT")
			{
				int objID = System.Convert.ToInt32(myMapParts[0]);
				int pX = System.Convert.ToInt32(myMapParts[1])-500+globalGridSizeX/2 - 1;
				int pZ = System.Convert.ToInt32(myMapParts[3])-500+globalGridSizeZ/2 - 1;
				int rY = System.Convert.ToInt32(myMapParts[5]);
				string staticInfo = myMapParts[7];//1:static
				
				int index_x = Mathf.FloorToInt(pX/map_w); 
				int index_z = Mathf.FloorToInt(pZ/map_h);
				var _mapinfo = mapList[index_x,index_z];
				_mapinfo.posIndex.Add(pZ*globalGridSizeX + pX);
				_mapinfo.objRotList.Add(rY);
				_mapinfo.objIndexList.Add(objID);
				
        		UnityEditor.EditorUtility.SetDirty(_mapinfo);
			}
        }
		
        UnityEditor.AssetDatabase.SaveAssets();
    }

    public static void LoadMap(string name, out Dictionary<string, List<GameObject>> LayerObjDic,out string mapinfo)
    {
        StreamReader sr = new StreamReader(uteGLOBAL3dMapEditor.getMapsDir() + name + ".txt");
        mapinfo = sr.ReadToEnd();
        sr.Close();

    	string[] allMapBaseItems = mapinfo.Split("$"[0]);
    	List<string> allGuids = new List<string>();

    	for(int i=0;i<allMapBaseItems.Length-1;i++)
    	{
    		string[] allInfoSplit = allMapBaseItems[i].Split(":"[0]);
    		allGuids.Add(allInfoSplit[0].ToString()+"&"+allInfoSplit[10].ToString());
    	}

    	allGuids = RemoveDuplicates(allGuids);


        LayerObjDic = new Dictionary<string, List<GameObject>>();

        for (int i = 0; i < allGuids.Count; i++)
        {
			string[] str = allGuids[i].Split("&"[0]);
            string guid = str[0];
			string layer = str[1];
            string opath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            GameObject tGO = (GameObject)UnityEditor.AssetDatabase.LoadMainAssetAtPath(opath);
			if (!LayerObjDic.ContainsKey(layer))
			{
				LayerObjDic.Add(layer, new List<GameObject>());
			}
			LayerObjDic[layer].Add(tGO);
            mapinfo = mapinfo.Replace(guid, i.ToString());
        }
    }

    private static  List<string> RemoveDuplicates(List<string> myList)
    {
        List<string> newList = new List<string>();

        for(int i=0;i<myList.Count;i++)
            if (!newList.Contains(myList[i].ToString()))
                newList.Add(myList[i].ToString());

        return newList;
    }

    static T GetStriptableObject<T>(string path) where T : ScriptableObject
    {
        T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
        }
        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<T>();
            UnityEditor.AssetDatabase.CreateAsset(asset, path);
        }
        return asset;
    }
}
