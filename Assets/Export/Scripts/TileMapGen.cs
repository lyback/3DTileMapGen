using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TileMapGen : MonoBehaviour
{
    public GameObject Root;                      // 场景根节点
    ObjRefInfo m_objRefInfoAsset;
    TileMapInfo[,] m_tilemapInfo;
    int m_tilemap_W;
    int m_tilemap_H;
    int m_map_W;
    int m_map_H;
    Vector2 m_offset;
    Vector2Int m_visibleView;
    VisiableObj[,] m_visibleObj;
    Dictionary<int, TileMapObjPool> m_poolDic = new Dictionary<int, TileMapObjPool>();

    int m_lastPos_x = int.MaxValue;
    int m_lastPos_z = int.MaxValue;

    public void Init(ObjRefInfo objRefInfo, int mapSizeW, int mapSizeH, int w, int h, Vector2Int view, Vector2 offset)
    {
        m_objRefInfoAsset = objRefInfo;
        m_visibleView = view;
        m_offset = offset;
        m_visibleObj = new VisiableObj[view.x, view.y];

        m_map_W = mapSizeW;
        m_map_H = mapSizeH;

        m_tilemap_W = w;
        m_tilemap_H = h;
        m_tilemapInfo = new TileMapInfo[mapSizeW / w, mapSizeH / h];
    }

    public void MoveTo(float x, float z)
    {
        int start_x = Mathf.FloorToInt(x);
        int start_z = Mathf.FloorToInt(z);
        if (start_x == m_lastPos_x && start_z == m_lastPos_z)
        {
            return;
        }
        m_lastPos_x = start_x;
        m_lastPos_z = start_z;
        for (int i = 0; i < m_visibleObj.GetLength(0); i++)
        {
            for (int j = 0; j < m_visibleObj.GetLength(1); j++)
            {
                int _pos_x = start_x + i;
                int _pos_z = start_z + j;
                if (_pos_x >= m_map_W || _pos_x < 0 || _pos_z >= m_map_H || _pos_z < 0)
                {
                    SetVisibleObjNUll(i, j);
                    continue;
                }
                int _index_x = _pos_x / m_tilemap_W;
                int _index_z = _pos_z / m_tilemap_H;
                TileMapInfo _mapInfo = GetTileMapInfo(_index_x, _index_z);
                // int map_x = _pos_x % m_tilemap_W;
                // int map_y = _pos_z % m_tilemap_H;
                int map_index = _pos_z * m_map_W + _pos_x;
                TileMapObjInfo mapObjInfo;
                if (_mapInfo.mapInfoDic.TryGetValue(map_index, out mapObjInfo))
                {
                    int objIndex = mapObjInfo.objIndex;
                    int objRotY = mapObjInfo.objRotY;
                    SetVisibleObj(i, j, objIndex, i, j, objRotY);
                }
                else
                {
                    SetVisibleObjNUll(i, j);
                }
            }
        }
        pos_temp.x = start_x;
        pos_temp.z = start_z;
        Root.transform.position = pos_temp;
    }
    Vector3 pos_temp = Vector3.zero;
    Vector3 rot_temp = Vector3.zero;
    Vector3 hide_temp = new Vector3(0f, 1000f, 0f);
    void SetVisibleObj(int i, int j, int objIndex, int x, int z, int rotY)
    {
        var obj = m_visibleObj[i, j];
        pos_temp.x = x + m_offset.x;
        pos_temp.z = z + m_offset.y;
        rot_temp.y = rotY;
        if (obj.obj != null)
        {
            if (obj.index == objIndex)
            {
                obj.obj.transform.localPosition = pos_temp;
                obj.obj.transform.localEulerAngles = rot_temp;
                return;
            }
            obj.obj.transform.localPosition = hide_temp;
            RecycleMapObj(obj.index, obj.obj);
        }
        m_visibleObj[i, j].index = objIndex;
        m_visibleObj[i, j].obj = GetMapObjFromPool(objIndex);
        obj = m_visibleObj[i, j];
        obj.obj.transform.localPosition = pos_temp;
        obj.obj.transform.localEulerAngles = rot_temp;
    }
    void SetVisibleObjNUll(int i, int j)
    {
        var obj = m_visibleObj[i, j];
        if (obj.obj != null)
        {
            obj.obj.transform.localPosition = hide_temp;
            RecycleMapObj(obj.index, obj.obj);
            m_visibleObj[i, j].obj = null;
            m_visibleObj[i, j].index = -1;
        }
    }
    GameObject GetMapObjFromPool(int index)
    {
        TileMapObjPool pool;
        if (m_poolDic.TryGetValue(index, out pool))
        {
            return pool.Get();
        }
        pool = new TileMapObjPool(m_objRefInfoAsset.gameObjectRef[index], Root.transform);
        m_poolDic.Add(index, pool);
        return pool.Get();
    }
    void RecycleMapObj(int index, GameObject obj)
    {
        m_poolDic[index].Recycle(obj);
    }
    TileMapInfo GetTileMapInfo(int w, int h)
    {
        if (m_tilemapInfo[w, h] == null)
        {
            m_tilemapInfo[w, h] = AssetDatabase.LoadAssetAtPath<TileMapInfo>(string.Format("Assets/Map/MapInfo_{0}_{1}.asset", w, h));
            m_tilemapInfo[w, h].Init();
        }
        return m_tilemapInfo[w, h];
    }
    struct VisiableObj
    {
        public int index;
        public GameObject obj;
    }
}
