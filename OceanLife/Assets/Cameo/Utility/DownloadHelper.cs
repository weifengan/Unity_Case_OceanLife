using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using Cameo;

public enum DownloadDataType {
	TEXT,
	IMAGE
}

public class DownloadResult {
	public bool IsSuccess;
	public string Error = "";
	public object Result = null;
	public byte[] ResultBytes = null;
	public Dictionary<string, object> Params = null;
}

public class DownloadMission {
	public string StrUrl;
	public DownloadDataType DataType;
	public System.Action<DownloadResult> CallBack;
	public Dictionary<string, object> Params;
} 

public class DownloadHelper : Singleton<DownloadHelper> {
	
	private List<DownloadMission> _lstQueue = new List<DownloadMission> ();
	private int intCount = 0;
	private int intMax = 10;

	public void DownloadData(string strUrl, DownloadDataType dataType, System.Action<DownloadResult> callBack, Dictionary<string, object> dicParams = null) {
		DownloadMission downloadMission = new DownloadMission ();
		downloadMission.StrUrl = strUrl;
		downloadMission.DataType = dataType;
		downloadMission.CallBack = callBack;
		downloadMission.Params = dicParams;
		_lstQueue.Add (downloadMission);

		GoDownload ();
	}

	void GoDownload() {
		if (_lstQueue.Count != 0) {
			if (intCount < intMax) {
				DownloadMission downloadMission = _lstQueue [0];
				switch (downloadMission.DataType) 
				{
				case DownloadDataType.TEXT:
					StartCoroutine (DownloadTextData (downloadMission.StrUrl, downloadMission.CallBack, downloadMission.Params));
					break;
				case DownloadDataType.IMAGE:
					StartCoroutine (DownloadImageData (downloadMission.StrUrl, downloadMission.CallBack, downloadMission.Params));
					break;
				}
				_lstQueue.RemoveAt (0);
			}

			if (_lstQueue.Count !=0)
				Invoke ("GoDownload", 0.05f);
		}
	}

	IEnumerator DownloadTextData(string strUrl, System.Action<DownloadResult> callBack, Dictionary<string, object> dicParams) {
		intCount++;

		WWW w = new WWW (strUrl);
		yield return w;

		DownloadResult result = new DownloadResult ();
		if (w.error != null) {
			Debug.Log ("[LoaderHelper] LoadTextData / Error: " + w.error);
			result.IsSuccess = false;
			result.Error = w.error;
			result.Params = dicParams;
		} else {
			result.IsSuccess = true;
			result.Result = (object) w.text;
			result.Params = dicParams;
		}

		intCount--;
		callBack (result);
	}

	IEnumerator DownloadImageData(string strUrl, System.Action<DownloadResult> callBack, Dictionary<string, object> dicParams) {
		intCount++;
		//Texture2D texture = new Texture2D ();

		WWW w = new WWW (strUrl);
		yield return w;

		DownloadResult result = new DownloadResult ();
		if (w.error != null) {
			Debug.Log ("[LoaderHelper] LoadTextData / Error: " + w.error);
			result.IsSuccess = false;
			result.Error = w.error;
			result.Params = dicParams;
		} else {
			//w.LoadImageIntoTexture (w.);
			result.IsSuccess = true;
			result.Result = (object) w.textureNonReadable;
			result.ResultBytes = w.bytes;
			result.Params = dicParams;
		}

		intCount--;
		callBack (result);
	}

}
