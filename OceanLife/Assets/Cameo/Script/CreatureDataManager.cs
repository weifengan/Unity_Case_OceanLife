using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using Cameo;

public class DicCreatureBriefInfo {
	public int intAreaId;
	public string strCreatureId;
	public string strCreatureName;
	public string strThumbnailUrl;
	public string strGenusId;
	public string strSpeciesId;
}

public class DicCreatureData {
	public string strDataType;
	public string strName;
	public string strValue;
	public string strThumbnailUrl;
}

public class CreatureDataManager : Singleton<CreatureDataManager> {

	private string strJsonLstDicCreatureUrl = "http://" + Config.Instance.strServerIp + "/JsonData/jsonLstDicCreature.json";
	private string strJsonCreatureInfoPrefix = "http://" + Config.Instance.strServerIp + "/JsonData/creature/";
	private System.Action dataReadyCallback;
	private bool isDataReady = false;
	private Dictionary<string, System.Action<List<DicCreatureData>>> _dicGetCreatureCallback = new Dictionary<string, System.Action<List<DicCreatureData>>> ();

	public void initData(System.Action dataReadyCallBackIn) {
		dataReadyCallback = dataReadyCallBackIn;

		List<string> lstCollectedCreatureId = UserProgress.LoadLstCollectedCreatureId ();
		TempDataHolder.Instance.SaveTempData ("lstCollectedCreatureId", lstCollectedCreatureId);

		DataCenter.Instance.GetJson (strJsonLstDicCreatureUrl, OnLoadLstDicCreatureReturn);
	}

	void OnLoadLstDicCreatureReturn(DataResult dataResult) {
		if (!dataResult.IsSuccess)
			return;

		var lstDicCreatureBriefInfo = JsonMapper.ToObject<List<DicCreatureBriefInfo>> ((string) dataResult.Result);
		TempDataHolder.Instance.SaveTempData ("lstDicCreatureBriefInfo", lstDicCreatureBriefInfo);

		isDataReady = true;
		if (dataReadyCallback != null) {
			dataReadyCallback ();
		}

		CaculateCollectionInfo ();
	}

	public bool IsDataReady() {
		return isDataReady;
	}

	private void CaculateCollectionInfo() {
		var lstAllDicCreatureBriefInfo = TempDataHolder.Instance.GetTempData ("lstDicCreatureBriefInfo") as List<DicCreatureBriefInfo>;
		Dictionary<string, int> dicAllAreaCollectionNumberInfo = new Dictionary<string, int> ();

		for (int i=0; i<lstAllDicCreatureBriefInfo.Count; i++) {
			int intCollectedNumber = 0;

			if (dicAllAreaCollectionNumberInfo.ContainsKey(lstAllDicCreatureBriefInfo[i].intAreaId.ToString())) {
				intCollectedNumber = dicAllAreaCollectionNumberInfo[lstAllDicCreatureBriefInfo[i].intAreaId.ToString()];
			}

			if (CheckIsCreatureCollected (lstAllDicCreatureBriefInfo [i].strCreatureId)) {
				intCollectedNumber += 1;
			}

			dicAllAreaCollectionNumberInfo [lstAllDicCreatureBriefInfo [i].intAreaId.ToString()] = intCollectedNumber;
		}

		TempDataHolder.Instance.SaveTempData ("dicAllAreaCollectionNumberInfo", dicAllAreaCollectionNumberInfo);
	}

	public List<DicCreatureBriefInfo> GetLstDicCreatureBriefInfoByAreaId(int intAreaId) {
		var lstDicCreatureBriefInfoReturn = new List<DicCreatureBriefInfo> ();
		var lstAllDicCreatureBriefInfo = TempDataHolder.Instance.GetTempData ("lstDicCreatureBriefInfo") as List<DicCreatureBriefInfo>;

		for (int i = 0; i < lstAllDicCreatureBriefInfo.Count; i++) {
			if (lstAllDicCreatureBriefInfo [i].intAreaId == intAreaId) {
				lstDicCreatureBriefInfoReturn.Add (lstAllDicCreatureBriefInfo [i]);
			}
		}

		return lstDicCreatureBriefInfoReturn;
	}

	public int GetIntAllCreatureNumber() {
		var lstAllDicCreatureBriefInfo = TempDataHolder.Instance.GetTempData ("lstDicCreatureBriefInfo") as List<DicCreatureBriefInfo>;
		return lstAllDicCreatureBriefInfo.Count;
	}

	public int GetIntCreatureNumberByAreaId (int intAreaId) {
		return GetLstDicCreatureBriefInfoByAreaId (intAreaId).Count;
	}

	public DicCreatureBriefInfo GetDicCreatureBriefInfoByCreatureId(string strCreatureId) {
		var lstAllDicCreatureBriefInfo = TempDataHolder.Instance.GetTempData ("lstDicCreatureBriefInfo") as List<DicCreatureBriefInfo>;

		for (int i = 0; i < lstAllDicCreatureBriefInfo.Count; i++) {
			if (lstAllDicCreatureBriefInfo [i].strCreatureId == strCreatureId) {
				return lstAllDicCreatureBriefInfo [i];
			}
		}

		return null;
	}

	public void GetLstDicCreatureDataByCreatureId(string strCreatureId, System.Action<List<DicCreatureData>> callback) {
		_dicGetCreatureCallback.Add (strCreatureId, callback);
		var strCreatureInfoUrl = strJsonCreatureInfoPrefix + strCreatureId + ".json";
		Dictionary<string, object> _passToCallParams = new Dictionary<string, object> ();
		_passToCallParams.Add ("strCreatureId", strCreatureId);
		DataCenter.Instance.GetCreatureJson (strCreatureInfoUrl, OnGetLstDicCreatureDataByCreatureIdReturn, _passToCallParams);
	}

	void OnGetLstDicCreatureDataByCreatureIdReturn(DataResult dataResult) {
		if (!dataResult.IsSuccess)
			return;

		string strCreatureId = (string)dataResult.Params ["strCreatureId"];
		if (_dicGetCreatureCallback.ContainsKey(strCreatureId)) {
			_dicGetCreatureCallback [strCreatureId] (JsonMapper.ToObject<List<DicCreatureData>> ((string) dataResult.Result));
			_dicGetCreatureCallback.Remove (strCreatureId);
		}
	}

//	public List<DicCreatureData> GetLstDicCreatureDataByCreatureId(string strCreatureId) {
//		TextAsset jsonLstCreatureData = ResourceManager.Instance.GetAsset ("json/creature/" + strCreatureId, typeof(TextAsset)) as TextAsset;
//		if (jsonLstCreatureData == null)
//			return null;
//		
//		var lstDicCreatureData = JsonMapper.ToObject<List<DicCreatureData>> (jsonLstCreatureData.text);
//		return lstDicCreatureData;
//	}

	public List<DicCreatureData> GetLstDicCreatureMediaDataByCreatureId(string strCreatureId) {
//		var lstDicCreatureData = GetLstDicCreatureDataByCreatureId (strCreatureId);
//		var lstDicCreatureMediaData = new List<DicCreatureData> ();
//
//		for (int i = 0; i < lstDicCreatureData.Count; i++) {
//			if (lstDicCreatureData [i].strDataType != "text") {
//				lstDicCreatureMediaData.Add (lstDicCreatureData [i]);
//			}
//		}
//
//		return lstDicCreatureMediaData;
		return null;
	}

	public List<DicCreatureData> GetLstDicCreatureTextDataByCreatureId(string strCreatureId) {
//		var lstDicCreatureData = GetLstDicCreatureDataByCreatureId (strCreatureId);
//		var lstDicCreatureTextData = new List<DicCreatureData> ();
//
//		for (int i = 0; i < lstDicCreatureData.Count; i++) {
//			if (lstDicCreatureData [i].strDataType == "text") {
//				lstDicCreatureTextData.Add (lstDicCreatureData [i]);
//			}
//		}
//
//		return lstDicCreatureTextData;
		return null;
	}

	public Sprite GetCreatureListThumbnailSpriteByUrl(string strImageUrl) {
		string strImagePath = "thumbnail/" + MD5.Instance.getStrMd5String (strImageUrl);
		Sprite spriteImage = ResourceManager.Instance.GetAsset (strImagePath, typeof(Sprite)) as Sprite;

		return spriteImage;
	}

	public bool CheckIsCreatureCollected(string strCreatureId) {
		List<string> lstCollectedCreatureId = (List<string>) TempDataHolder.Instance.GetTempData ("lstCollectedCreatureId");
		return lstCollectedCreatureId.Contains (strCreatureId);
	}

	public void SetCreatureCollected(string strCreaturId) {
		List<string> lstCollectedCreatureId = (List<string>) TempDataHolder.Instance.GetTempData ("lstCollectedCreatureId");
		if (lstCollectedCreatureId.Contains (strCreaturId))
			return;

		lstCollectedCreatureId.Add (strCreaturId);
		UserProgress.SaveLstCollectedCreatureId (lstCollectedCreatureId);
		TempDataHolder.Instance.SaveTempData ("lstCollectedCreatureId", lstCollectedCreatureId);

		CaculateCollectionInfo ();
	}

	public int GetCollectionNumber() {
		List<string> lstCollectedCreatureId = (List<string>) TempDataHolder.Instance.GetTempData ("lstCollectedCreatureId");
		return lstCollectedCreatureId.Count;
	}

	public int GetIntAreaCollectedNumberByAreaId(int intAreaId) {
		Dictionary<string, int> dicAllAreaCollectionNumberInfo = (Dictionary<string, int>) TempDataHolder.Instance.GetTempData ("dicAllAreaCollectionNumberInfo");
		if (dicAllAreaCollectionNumberInfo.ContainsKey (intAreaId.ToString ())) {
			return dicAllAreaCollectionNumberInfo [intAreaId.ToString ()];
		} else {
			return 0;
		}
	}
}
