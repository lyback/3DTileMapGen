using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RenderObject : MonoBehaviour
{
	void Awake()
	{
		DoInit();
	}

	void OnDestroy()
	{
		DoShut();
	}

	public virtual void Dispose()
	{
		GameObject.Destroy(gameObject);
	}

	protected virtual void DoInit()
	{

	}

	protected virtual void DoShut()
	{

	}

}
