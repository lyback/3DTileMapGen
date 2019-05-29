using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RenderDynamicMesh : RenderObject
{
    protected static int[] INDEXS = new int[4];

    protected Mesh m_mesh;
    protected MeshRenderer m_renderer;
    protected MeshFilter m_filter;
    #region 绘制相关
    protected List<Vector3> m_vertices; // 顶点列表
    protected List<Vector2> m_uvs;      // uv列表
    protected List<List<int>> m_triangles;    // 三角面列表
    protected Dictionary<int,int> m_subMeshIndexDic;
    #endregion

    protected virtual GameObject Root
    {
        get
        {
            return gameObject;
        }
    }
    protected override void DoInit()
    {
        base.DoInit();

        m_vertices = new List<Vector3>();
        m_uvs = new List<Vector2>();
        m_triangles = new List<List<int>>(4);
        for (int i = 0; i < 4; i++)
        {
            m_triangles.Add(new List<int>(5000));
        }
        m_subMeshIndexDic = new Dictionary<int,int>();

        m_mesh = new Mesh();
        m_mesh.MarkDynamic();

        m_renderer = Root.GetComponent<MeshRenderer>();
        m_filter = Root.GetComponent<MeshFilter>();
        m_filter.sharedMesh = m_mesh;
    }

    protected void FillTriangles(List<int> triangles)
    {
        triangles.Add(INDEXS[0]);
        triangles.Add(INDEXS[2]);
        triangles.Add(INDEXS[3]);
        triangles.Add(INDEXS[0]);
        triangles.Add(INDEXS[3]);
        triangles.Add(INDEXS[1]);
    }
}
