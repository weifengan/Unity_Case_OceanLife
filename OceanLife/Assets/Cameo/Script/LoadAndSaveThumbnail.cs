using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using Cameo;

public class LoadAndSaveThumbnail : MonoBehaviour {

//	private string strJsonBeaconsUrl = "http://210.65.11.236/JsonData/jsonBeacons.json";
//	private string strJsonLstDicAreaUrl = "http://210.65.11.236/JsonData/jsonLstDicArea.json";
//	private string strJsonDicAreaInfoUrl = "http://210.65.11.236/JsonData/jsonDicAreaInfo.json";
//	private string strJsonLstDicFunnyQuestionUrl = "http://210.65.11.236/JsonData/jsonLstDicFunnyQuestion.json";
	private string strJsonLstDicCreatureUrl = "http://210.65.11.236/JsonData/jsonLstDicCreature.json";
//	private string strJsonCreatureInfoPrefix = "http://210.65.11.236/JsonData/creature/";

	private string strJsonBeaconsUrl = "http://nmmba.tapmovie.com/JsonData/jsonBeacons.json";
	private string strJsonLstDicAreaUrl = "http://nmmba.tapmovie.com/JsonData/jsonLstDicArea.json";
	private string strJsonDicAreaInfoUrl = "http://nmmba.tapmovie.com/JsonData/jsonDicAreaInfo.json";
	private string strJsonLstDicFunnyQuestionUrl = "http://nmmba.tapmovie.com/JsonData/jsonLstDicFunnyQuestion.json";
//	private string strJsonLstDicCreatureUrl = "http://nmmba.tapmovie.com/JsonData/jsonLstDicCreature.json";
	private string strJsonCreatureInfoPrefix = "http://nmmba.tapmovie.com/JsonData/creature/";

	private string _jsonCachePath = "";
	private string _creatureJsonCachePath = "";

	private List<DicBeaconInfo> _lstBeaconInfo = new List<DicBeaconInfo>();
	private List<string> _lstDownloadQueue = new List<string> ();
	private int intTotlaDownloadCount = 0;
	private int intDownloadCount = 0;
	private int intMaxDownload = 10;
	private List<DicCreatureBriefInfo> lstDicCreatureBriefInfo;
	private List<string> _lstEmptyThumbnailUrl = new List<string> ();
	private List<Dictionary<string, string>> _lstError = new List<Dictionary<string, string>>();
	private List<string> _listFailJson = new List<string> ();
	private int intCreaturInfoIndex = 0;

	// Use this for initialization
	void Start () {
		Debug.Log ("[LoadAndSaveThumbnail] Start / Downloaded Folder: " + Application.persistentDataPath);
		_jsonCachePath = Application.persistentDataPath + "/json";
		_creatureJsonCachePath = Application.persistentDataPath + "/json/creature";

//		GetLstBeacons ();
//
//		GetLstDicAreaJson ();
//		GetDicAreaInfoJson ();
//		GetLstDicFunnyQuestionJson ();

		GetLstDicCreatureJson();
		//GetCreatureInfo();
		//GetThumbnail ();
	}

	void GetLstBeacons() {
		string strFileMD5 = MD5.Instance.getStrMd5String (strJsonBeaconsUrl);
		Dictionary<string, object> dicParamsPassToCallback = new Dictionary<string, object> ();
		dicParamsPassToCallback.Add ("strUrl", strJsonBeaconsUrl);
		dicParamsPassToCallback.Add ("strDataMissionKey", strFileMD5);
		dicParamsPassToCallback.Add ("strFilePath", _jsonCachePath + "/" + strFileMD5 + ".json");
		DownloadHelper.Instance.DownloadData (strJsonBeaconsUrl, DownloadDataType.TEXT, onDownloadJsonComplete, dicParamsPassToCallback);
	}

	void GetLstDicAreaJson() {
		string strFileMD5 = MD5.Instance.getStrMd5String (strJsonLstDicAreaUrl);
		Dictionary<string, object> dicParamsPassToCallback = new Dictionary<string, object> ();
		dicParamsPassToCallback.Add ("strUrl", strJsonLstDicAreaUrl);
		dicParamsPassToCallback.Add ("strDataMissionKey", strFileMD5);
		dicParamsPassToCallback.Add ("strFilePath", _jsonCachePath + "/" + strFileMD5 + ".json");
		DownloadHelper.Instance.DownloadData (strJsonLstDicAreaUrl, DownloadDataType.TEXT, onDownloadJsonComplete, dicParamsPassToCallback);
	}

	void GetDicAreaInfoJson() {
		string strFileMD5 = MD5.Instance.getStrMd5String (strJsonDicAreaInfoUrl);
		Dictionary<string, object> dicParamsPassToCallback = new Dictionary<string, object> ();
		dicParamsPassToCallback.Add ("strUrl", strJsonDicAreaInfoUrl);
		dicParamsPassToCallback.Add ("strDataMissionKey", strFileMD5);
		dicParamsPassToCallback.Add ("strFilePath", _jsonCachePath + "/" + strFileMD5 + ".json");
		DownloadHelper.Instance.DownloadData (strJsonDicAreaInfoUrl, DownloadDataType.TEXT, onDownloadJsonComplete, dicParamsPassToCallback);
	}

	void GetLstDicFunnyQuestionJson() {
		string strFileMD5 = MD5.Instance.getStrMd5String (strJsonLstDicFunnyQuestionUrl);
		Dictionary<string, object> dicParamsPassToCallback = new Dictionary<string, object> ();
		dicParamsPassToCallback.Add ("strUrl", strJsonLstDicFunnyQuestionUrl);
		dicParamsPassToCallback.Add ("strDataMissionKey", strFileMD5);
		dicParamsPassToCallback.Add ("strFilePath", _jsonCachePath + "/" + strFileMD5 + ".json");
		DownloadHelper.Instance.DownloadData (strJsonLstDicFunnyQuestionUrl, DownloadDataType.TEXT, onDownloadJsonComplete, dicParamsPassToCallback);
	}

	void GetLstDicCreatureJson() {
		intTotlaDownloadCount = 1;
		string strFileMD5 = MD5.Instance.getStrMd5String (strJsonLstDicCreatureUrl);
		Dictionary<string, object> dicParamsPassToCallback = new Dictionary<string, object> ();
		dicParamsPassToCallback.Add ("strUrl", strJsonLstDicFunnyQuestionUrl);
		dicParamsPassToCallback.Add ("strDataMissionKey", strFileMD5);
		dicParamsPassToCallback.Add ("strFilePath", _jsonCachePath + "/" + strFileMD5 + ".json");
		dicParamsPassToCallback.Add ("isCreatureLst", true);
		DownloadHelper.Instance.DownloadData (strJsonLstDicCreatureUrl, DownloadDataType.TEXT, onDownloadJsonComplete, dicParamsPassToCallback);
	}

	void GetCreatureInfo() {
		intTotlaDownloadCount = lstDicCreatureBriefInfo.Count;
		LoadCreatureInfo ();
	}

	void LoadCreatureInfo() {
		if (intCreaturInfoIndex >= lstDicCreatureBriefInfo.Count) {
			Debug.Log ("[LoadAndSaveThumbnail] LoadCreatureInfo / load complete");
		} else {
			string strCreatureUrl = strJsonCreatureInfoPrefix + lstDicCreatureBriefInfo [intCreaturInfoIndex].strCreatureId + ".json";
			string strFileMD5 = MD5.Instance.getStrMd5String (strCreatureUrl);
			Debug.Log (strCreatureUrl + " " + strFileMD5);
			string strFilePath = _creatureJsonCachePath + "/" + strFileMD5 + ".json";

			Dictionary<string, object> dicParamsPassToCallback = new Dictionary<string, object> ();
			dicParamsPassToCallback.Add ("strUrl", strCreatureUrl);
			dicParamsPassToCallback.Add ("strDataMissionKey", strFileMD5);
			dicParamsPassToCallback.Add ("strFilePath", strFilePath);
			DownloadHelper.Instance.DownloadData (strCreatureUrl, DownloadDataType.TEXT, onDownloadJsonComplete, dicParamsPassToCallback);

			intCreaturInfoIndex++;
			Debug.Log ("[LoadAndSaveThumbnail] LoadCreatureInfo -> intCreaturInfoIndex / intTotal: " + intCreaturInfoIndex + " / " + lstDicCreatureBriefInfo.Count);
			Invoke ("LoadCreatureInfo", 0.1f);
		}
	}

	void onDownloadJsonComplete(DownloadResult downloadResult) {
		//Debug.Log ("[LoadAndSaveThumbnail] onDownloadLstDicAreaComplete / downloadResult: " + downloadResult.Result);
		intTotlaDownloadCount--;
		Debug.Log ("[LoadAndSaveThumbnail] onDownloadJsonComplete / intTotlaDownloadCount: " + intTotlaDownloadCount);
		Dictionary<string, object> dicParams = downloadResult.Params;
		string strUrl = (string) dicParams ["strUrl"];
		string strFileMD5 = (string) dicParams ["strDataMissionKey"];
		string strFilePath = (string) dicParams ["strFilePath"];

		if (!downloadResult.IsSuccess) {
			//Debug.Log (strUrl + " is fail.");
			_listFailJson.Add (strUrl);
			return;
		}

		if (intTotlaDownloadCount == 0) {
			Debug.Log ("[LoadAndSaveThumbnail] Download Creature Complete and Fail url is: " + JsonMapper.ToJson(_listFailJson));
		}

		SaveFileHelper.Instance.SaveTextData (strFilePath, (string)downloadResult.Result, null, null);

		if (dicParams.ContainsKey ("isCreatureLst")) {
			lstDicCreatureBriefInfo = JsonMapper.ToObject<List<DicCreatureBriefInfo>> ((string) downloadResult.Result);
			Debug.Log (JsonMapper.ToJson(lstDicCreatureBriefInfo));
			//GetCreatureInfo ();
			DownloadThumbnail ();
		}
	}

	void DownloadThumbnail() {
		// Test For 404 Not Found
		// StartCoroutine (LoadAndSaveImageWithMD5Encrypt ("http://db.nmmba.gov.tw/BioSDS_WEB/BioSDS_Photo/Description/Fish/NMMBA10166_1.jpg"));
		for (int i=0; i<lstDicCreatureBriefInfo.Count; i++) {
			if (lstDicCreatureBriefInfo [i].strThumbnailUrl != "") {
				_lstDownloadQueue.Add (lstDicCreatureBriefInfo [i].strThumbnailUrl);

				// 20160802 bigcookie: 先不抓生物介紹頁的縮圖，檔案太多太大，APP 裝不下
				//				var lstDicCreatureData = CreatureDataManager.Instance.GetLstDicCreatureDataByCreatureId (lstAllDicCreatureBriefInfo [i].strCreatureId);
				//
				//				if (lstDicCreatureData == null)
				//					continue;
				//				
				//				for (int j = 0; j < lstDicCreatureData.Count; j++) {
				//					if (lstDicCreatureData [j].strDataType != "image" && lstDicCreatureData [j].strDataType != "video")
				//						continue;
				//					
				//					if (lstDicCreatureData [j].strThumbnailUrl != "") {
				//						_lstDownloadQueue.Add (lstDicCreatureData [j].strThumbnailUrl);
				//					} else if (lstDicCreatureData [j].strDataType == "image" && lstDicCreatureData [j].strValue != "") {
				//						_lstDownloadQueue.Add (lstDicCreatureData [j].strValue);
				//					}
				//				}
			} else {
				_lstEmptyThumbnailUrl.Add (lstDicCreatureBriefInfo [i].strCreatureId);
			}
		}

		Debug.Log ("[LoadAndSaveThumbnail] Start / intTotlaDownloadCount: " + _lstDownloadQueue.Count);
		StartDownload ();
	}

	void StartDownload() {
		if (_lstDownloadQueue.Count != 0) {
			//Debug.Log ("[LoadAndSaveThumbnail] StartDownload / intDownloadCount: " + intDownloadCount);
			if (intDownloadCount < intMaxDownload) {
				string strDownloadUrl = _lstDownloadQueue [0];
				//Debug.Log ("[LoadAndSaveThumbnail] StartDownload / strDownloadUrl: " + strDownloadUrl);
				StartCoroutine (LoadAndSaveImageWithMD5Encrypt (strDownloadUrl));
				_lstDownloadQueue.RemoveAt (0);
				Debug.Log ("[LoadAndSaveThumbnail] StartDownload / intTotlaDownloadCount: " + _lstDownloadQueue.Count);
			}
			Invoke ("StartDownload", 0.05f);
		} else {
			Debug.Log ("[LoadAndSaveThumbnail] StartDownload / Download Complete!");
			Debug.Log ("[LoadAndSaveThumbnail] Summery / _lstEmptyThumbnailUrl: " + JsonMapper.ToJson(_lstEmptyThumbnailUrl));
			Debug.Log ("[LoadAndSaveThumbnail] Summery / _lstError: " + JsonMapper.ToJson(_lstError));
		}
	}

	IEnumerator LoadAndSaveImageWithMD5Encrypt(string strImageUrl) {
		string strThumbnailFolderPath = Application.persistentDataPath + "/thumbnail";
		string strImageMD5 = MD5.Instance.getStrMd5String (strImageUrl);

		if (System.IO.File.Exists (strThumbnailFolderPath + "/" + strImageMD5 + ".jpg")) {
			//Debug.Log ("[LoadAndSaveThumbnail] LoadAndSaveImageWithMD5Encrypt / Already Downloaded: " + strImageMD5 + ".jpg");
			yield return false;
		}

		intDownloadCount++;

		if (!System.IO.File.Exists (strThumbnailFolderPath)) {
			System.IO.Directory.CreateDirectory (strThumbnailFolderPath);
		}

		//Debug.Log ("[LoadAndSaveThumbnail] LoadAndSaveImageWithMD5Encrypt / Before download");

		WWW www = new WWW (strImageUrl);
		yield return www;

		string HttpStatus = "";
		if(www.responseHeaders.Count > 0) {
			HttpStatus = www.responseHeaders["STATUS"];
		}

		if (HttpStatus.IndexOf ("404") >= 0) {
			intDownloadCount--;
			Dictionary<string, string> dic404Log = new Dictionary<string, string> ();
			dic404Log.Add (HttpStatus, strImageUrl);
			_lstError.Add (dic404Log);
			//Debug.Log ("[LoadAndSaveThumbnail] LoadAndSaveImageWithMD5Encrypt / 404 Not Found: " + strImageUrl);
			yield return false;
		}

		if (www.error != null) {
			Debug.Log ("[LoadAndSaveThumbnail] LoadAndSaveImageWithMD5Encrypt / Error: " + www.error + ", strImageUrl: " + strImageUrl);
			Dictionary<string, string> dicErrorLog = new Dictionary<string, string> ();
			dicErrorLog.Add (www.error, strImageUrl);
			_lstError.Add (dicErrorLog);
		} else {
			System.IO.File.WriteAllBytes (strThumbnailFolderPath + "/" + strImageMD5 + ".jpg", www.bytes);
			//Debug.Log ("[LoadAndSaveThumbnail] LoadAndSaveImageWithMD5Encrypt / strImageMD5, strImageUrl: " + strImageMD5 + ", " + strImageUrl);
		}

		intDownloadCount--;
	}

}
