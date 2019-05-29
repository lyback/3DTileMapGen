using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TileMapRoot : RenderDynamicMesh
{
    TileMapInfo[] m_tilemapInfo;
    int m_tilemap_W;
    int m_tilemap_H;
    int m_visibleSize;

    public GameObject RootTerrain;                      // 场景根节点
    protected override GameObject Root
    {
        get
        {
            return RootTerrain;
        }
    }

    public void Init(int w, int h, int size)
    {
        m_visibleSize = size;
        m_tilemap_W = w;
        m_tilemap_H = h;
        m_tilemapInfo = new TileMapInfo[w * h];
    }

    public void MoveTo(float x, float z)
    {
        ClearMeshData();
		
    }


    void ClearMeshData()
    {
        m_vertices.Clear();
        m_uvs.Clear();
        m_subMeshIndexDic.Clear();
        for (int i = 0; i < m_triangles.Count; i++)
        {
            m_triangles[i].Clear();
        }
    }
}
