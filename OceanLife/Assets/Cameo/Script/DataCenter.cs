using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using Cameo;

public class DataResult {
	public bool IsSuccess;
	public string Error = "";
	public object Result = null;
	public Dictionary<string, object> Params = null;
}

public class DataMission {
	public string StrUrl;
	public string StrType;
	public Dictionary<string, object> Params = null;
	public System.Action<DataResult> Callback = null;
}

public class DataCenter :Singleton<DataCenter> {

	private string _jsonResourcePath = "json";
	private string _creatureJsonResourcePath = "json/creature";
	private string _thumbnailResourcePath = "thumbnail";

	// KID - 35 行 在 Awake 內呼叫
	//private string _jsonCachePath = Application.persistentDataPath + "/json";
	private string _jsonCachePath;
	//private string _creatureJsonCachePath = Application.persistentDataPath + "/json/creature";
	private string _creatureJsonCachePath;
	//private string _thumbnailCachePath = Application.persistentDataPath + "/thumbnail";
	private string _thumbnailCachePath;
	// KID


	private Dictionary<string, DataMission> _dataMissionQueue = new Dictionary<string, DataMission> ();

	// KID
	private void Awake()
	{
		_jsonCachePath = Application.persistentDataPath + "/json";
		_creatureJsonCachePath = Application.persistentDataPath + "/json/creature";
		_thumbnailCachePath = Application.persistentDataPath + "/thumbnail";
	}
	// KID

	void Start() {
		if (!System.IO.File.Exists (_jsonCachePath)) {
			System.IO.Directory.CreateDirectory (_jsonCachePath);
		}

		if (!System.IO.File.Exists (_creatureJsonCachePath)) {
			System.IO.Directory.CreateDirectory (_creatureJsonCachePath);
		}

		if (!System.IO.File.Exists (_thumbnailCachePath)) {
			System.IO.Directory.CreateDirectory (_thumbnailCachePath);
		}
	}

	public void GetJson(string strUrl, System.Action<DataResult> callback, Dictionary<string, object> dicParams = null) {
		string strReturnJson = "";
		DataMission dataMission = new DataMission ();
		dataMission.StrUrl = strUrl;
		dataMission.Callback = callback;
		dataMission.Params = dicParams;
		dataMission.StrType = "Json";

		string strFileMD5 = MD5.Instance.getStrMd5String (strUrl);
		_dataMissionQueue.Add (strFileMD5, dataMission);

		DataResult result = new DataResult ();
		result.Error = "From Cache";
		strReturnJson = GetJsonFromFromCache (strFileMD5);

		if (strReturnJson == "") {
			strReturnJson = GetJsonFromResource (strFileMD5);
			result.Error = "From Resource";
		}

		if (strReturnJson != "") {
			result.IsSuccess = true;
			result.Result = (object)strReturnJson;
			result.Params = dataMission.Params;
			dataMission.Callback (result);
		}

		GetJsonData (strFileMD5);
	}

	public void GetCreatureJson(string strUrl, System.Action<DataResult> callback, Dictionary<string, object> dicParams = null) {
		string strReturnJson = "";
		DataMission dataMission = new DataMission ();
		dataMission.StrUrl = strUrl;
		dataMission.Callback = callback;
		dataMission.Params = dicParams;
		dataMission.StrType = "CreatureJson";
		string strFileMD5 = MD5.Instance.getStrMd5String (strUrl);
		_dataMissionQueue.Add (strFileMD5, dataMission);

		DataResult result = new DataResult ();
		result.Error = "From Cache";
		strReturnJson = GetJsonFromFromCache (strFileMD5);

		if (strReturnJson == "") {
			strReturnJson = GetJsonFromResource (strFileMD5);
			result.Error = "From Resource";
		}

		if (strReturnJson != "") {
			result.IsSuccess = true;
			result.Result = (object)strReturnJson;
			result.Params = dataMission.Params;
			dataMission.Callback (result);
		}

		GetJsonData (strFileMD5);
	}

	string GetJsonFromFromCache(string strFileMD5) {
		DataMission dataMission = _dataMissionQueue [strFileMD5];
		if (System.IO.File.Exists ((dataMission.StrType == "Json" ? _jsonCachePath : _creatureJsonCachePath) + "/" + strFileMD5 + ".json")) {
			System.IO.StreamReader streamReader = new System.IO.StreamReader ((dataMission.StrType == "Json" ? _jsonCachePath : _creatureJsonCachePath) + "/" + strFileMD5 + ".json");
			string strContent = streamReader.ReadToEnd ();
			streamReader.Close ();

			return strContent;
		}

		return "";
	}

	string GetJsonFromResource(string strFileMD5) {
		DataMission dataMission = _dataMissionQueue [strFileMD5];
		TextAsset jsonTextAsset = ResourceManager.Instance.GetAsset ((dataMission.StrType == "Json" ? _jsonResourcePath : _creatureJsonResourcePath) + "/" + strFileMD5, typeof(TextAsset)) as TextAsset;
		if (jsonTextAsset != null) {
			return jsonTextAsset.text;
		} else {
			return "";
		}
	}

	void GetJsonData(string strFileMD5) {
		DataMission dataMission = _dataMissionQueue [strFileMD5];

		Dictionary<string, object> dicParamsPassToCallback = new Dictionary<string, object> ();
		dicParamsPassToCallback.Add ("strDataMissionKey", strFileMD5);
		dicParamsPassToCallback.Add ("strFilePath", (dataMission.StrType == "Json" ? _jsonCachePath : _creatureJsonCachePath) + "/" + strFileMD5 + ".json");
		DownloadHelper.Instance.DownloadData (dataMission.StrUrl, DownloadDataType.TEXT, OnDownloadCallback, dicParamsPassToCallback);
	}

	public void GetThumbnail(string strUrl, System.Action<DataResult> callback, Dictionary<string, object> dicParams = null) {
		string strFileMD5 = MD5.Instance.getStrMd5String (strUrl);
		DataResult result = new DataResult ();

		if (_dataMissionQueue.ContainsKey (strFileMD5)) {
			result.IsSuccess = false;
			result.Result = null;
			result.Params = dicParams;
			callback (result);
			return;
		}

		DataMission dataMission = new DataMission ();
		dataMission.StrUrl = strUrl;
		dataMission.Callback = callback;
		dataMission.Params = dicParams;
		dataMission.StrType = "Image";

		_dataMissionQueue.Add (strFileMD5, dataMission);


		// Get Data From ResourceManager
		Sprite spriteImage = ResourceManager.Instance.GetAsset (_thumbnailResourcePath + "/" + strFileMD5, typeof(Sprite)) as Sprite;
		if (spriteImage != null) {
			result.IsSuccess = true;
			result.Result = (object) spriteImage;
			result.Params = dataMission.Params;
			dataMission.Callback (result);
			RemoveMission (strFileMD5);
			DebugLogger.Instance.LogWarnning ("[DataCenter] Data From ResourceManager");
			return;
		}

		// Get Data From Cache
		if (System.IO.File.Exists (_thumbnailCachePath + "/" + strFileMD5 + ".jpg")) {
			System.IO.StreamReader streamReader = new System.IO.StreamReader (_thumbnailCachePath + "/" + strFileMD5 + ".jpg");
			Texture2D texture = new Texture2D (1, 1);
			byte[] imgData = System.IO.File.ReadAllBytes (_thumbnailCachePath + "/" + strFileMD5 + ".jpg");
			texture.LoadImage (imgData);

			result.IsSuccess = true;
			result.Result = (object) Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
			result.Params = dataMission.Params;
			dataMission.Callback (result);
			RemoveMission (strFileMD5);
			DebugLogger.Instance.LogWarnning ("[DataCenter] Data From Cache");
			return;
		}

		Dictionary<string, object> dicParamsPassToCallback = new Dictionary<string, object> ();
		dicParamsPassToCallback.Add ("strDataMissionKey", strFileMD5);
		dicParamsPassToCallback.Add ("strFilePath", _thumbnailCachePath + "/" + strFileMD5 + ".jpg");
		DownloadHelper.Instance.DownloadData (dataMission.StrUrl, DownloadDataType.IMAGE, OnDownloadCallback, dicParamsPassToCallback);
	}

	void OnDownloadCallback(DownloadResult downloadResult) {
		Dictionary<string, object> dicParams = downloadResult.Params;
		string strFileMD5 = (string) dicParams ["strDataMissionKey"];
		string strFilePath = (string) dicParams ["strFilePath"];
		DataMission dataMission = _dataMissionQueue[strFileMD5];

		DataResult dataResult = new DataResult ();

		if (!downloadResult.IsSuccess) {
			dataResult.IsSuccess = false;
			dataResult.Error = downloadResult.Error;
			dataResult.Params = dataMission.Params;
			dataMission.Callback (dataResult);
			RemoveMission ((string) dicParams ["strDataMissionKey"]);
			return;
		}

		dataResult.IsSuccess = true;
		dataResult.Params = dataMission.Params;
		dataResult.Error = "From Download";
		if (dataMission.StrType == "Image") {
			Texture2D texture = (Texture2D) downloadResult.Result;
			dataResult.Result = (object) Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));;
		} else {
			dataResult.Result = downloadResult.Result;
		}
		dataMission.Callback (dataResult);
		DebugLogger.Instance.LogWarnning ("[DataCenter] Data From Download");

		Dictionary<string, object> dicParamsPassToCallback = new Dictionary<string, object> ();
		dicParamsPassToCallback.Add ("strDataMissionKey", strFileMD5);
		if (dataMission.StrType == "Image") {
			SaveFileHelper.Instance.SaveImageData (strFilePath, downloadResult.ResultBytes, OnSaveFileComplete, dicParamsPassToCallback);
		} else {
			SaveFileHelper.Instance.SaveTextData (strFilePath, (string) downloadResult.Result, OnSaveFileComplete, dicParamsPassToCallback);
		}
	}

	void OnSaveFileComplete(SaveResult saveResult) {
		Dictionary<string, object> dicParams = saveResult.Params;
		string strFileMD5 = (string) dicParams ["strDataMissionKey"];
		RemoveMission (strFileMD5);
	}

	void RemoveMission(string strFileMD5) {
		_dataMissionQueue.Remove (strFileMD5);
	}
}
