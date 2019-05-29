using UnityEngine;
using System.Collections;
using System.IO;

public class uteSaveMap : MonoBehaviour {
#if UNITY_EDITOR
	[HideInInspector]
	public bool isSaving;
	public uteOptionsBox options;
	private string boundsInfo;

	private void Start()
	{
		boundsInfo = "";
		isSaving = false;
	}

	public void PassMapBounds(float mostLeft, float mostRight, float mostForward, float mostBack, float mostUp, float mostBottom)
	{
		boundsInfo += mostLeft.ToString()+":"+mostRight.ToString()+":"+mostForward.ToString()+":"+mostBack.ToString()+":"+mostUp.ToString()+":"+mostBottom.ToString();
	}

	public IEnumerator SaveMap(string mapName, bool isItMap)
	{
		isSaving = true;
		yield return 0;

		GameObject main = (GameObject) GameObject.Find("MAP");
		uteTagObject[] allObjects = main.GetComponentsInChildren<uteTagObject>();
		string info = "";

		for(int i=0;i<allObjects.Length;i++)
		{
			if(i%2000==0) yield return 0;
			
			GameObject obj = (GameObject) ((uteTagObject)allObjects[i]).gameObject;
			string objGUID = ((uteTagObject)allObjects[i]).objGUID;
			bool objIsStatic = ((uteTagObject)allObjects[i]).isStatic;
			string layerName = ((uteTagObject)allObjects[i]).layerName;
			bool objTC = ((uteTagObject)allObjects[i]).isTC;
			string tcFamilyName = "-";

			if(obj.GetComponent<uteTcTag>())
			{
				tcFamilyName = ((uteTcTag) obj.GetComponent<uteTcTag>()).tcFamilyName;
			}

			string staticInfo = "0";
			string tcInfo = "0";

			if(objIsStatic)
				staticInfo = "1";

			if(objTC)
				tcInfo = "1";

			info += objGUID+":"+obj.transform.position.x+":"+obj.transform.position.y+":"+obj.transform.position.z+":"+((int)obj.transform.localEulerAngles.x)+":"+((int)obj.transform.localEulerAngles.y)+":"+((int)obj.transform.localEulerAngles.z)+":"+staticInfo+":"+tcInfo+":"+tcFamilyName+":"+layerName+":$";
		}

		string path;

		if(isItMap)
		{
			path = uteGLOBAL3dMapEditor.getMapsDir();
		}
		else
		{
			path = uteGLOBAL3dMapEditor.getPatternsDir();
		}

		StreamWriter sw = new StreamWriter(path+mapName+".txt");
		sw.Write("");
		sw.Write(info);
		sw.Flush();
		sw.Close();

		SaveMapSettings(mapName,isItMap);

		isSaving = false;

		yield return 0;
	}

	private void SaveMapSettings(string mapName, bool isItMap)
	{
		if(isItMap)
		{
			string path = uteGLOBAL3dMapEditor.getMapsDir();
			GameObject MAIN = (GameObject) GameObject.Find("MAIN");
			GameObject YArea = (GameObject) GameObject.Find("MAIN/YArea");
			GameObject MapEditorCamera = (GameObject) GameObject.Find("MAIN/YArea/MapEditorCamera");

			string info = MAIN.transform.position.x+":"+MAIN.transform.position.y+":"+MAIN.transform.position.z+":"+MAIN.transform.localEulerAngles.x+":"+MAIN.transform.localEulerAngles.y+":"+MAIN.transform.localEulerAngles.z+":"+YArea.transform.localEulerAngles.x+":"+YArea.transform.localEulerAngles.y+":"+YArea.transform.localEulerAngles.z+":"+MapEditorCamera.transform.localEulerAngles.x+":"+MapEditorCamera.transform.localEulerAngles.y+":"+MapEditorCamera.transform.localEulerAngles.z+":";

			info += options.isEditorLightOn+":"+options.isCastShadows+":"+uteGLOBAL3dMapEditor.XZsnapping+":"+uteGLOBAL3dMapEditor.OverlapDetection+":"+options.isShowGrid+":"+uteGLOBAL3dMapEditor.CalculateXZPivot+":"+options.snapOnTop+":"+boundsInfo+":";
			info += options.isUse360Snap+":";

			StreamWriter sw = new StreamWriter(path+mapName+"_info.txt");
			sw.Write("");
			sw.Write(info);
			sw.Flush();
			sw.Close();
		}
	}

	private float RoundToHalf(float point)
	{
		point *= 2.0f;
		point = Mathf.Round(point);
		point /= 2.0f;

		return point;
	}
#endif
}
