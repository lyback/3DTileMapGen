using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TileMapObjPool
{
    GameObject m_Prefab;
    Transform m_Parent;
    Queue<GameObject> m_ObjCache = new Queue<GameObject>();

    public TileMapObjPool(GameObject prefab, Transform parent)
    {
        m_Prefab = prefab;
        m_Parent = parent;
    }
    public GameObject Get()
    {
        if (m_ObjCache.Count > 0)
        {
            return m_ObjCache.Dequeue();
        }
        var obj = GameObject.Instantiate(m_Prefab);
        obj.transform.parent = m_Parent;
        return obj;
    }
    public void Recycle(GameObject obj)
    {
        m_ObjCache.Enqueue(obj);
    }

}