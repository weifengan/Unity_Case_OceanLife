using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using Cameo;

public class DicAreaInfo {
	public int intBuildingId;
	public string strBuildingName;
	public int intAreaId;
	public string strAreaName;
}

public class DicAreaDescription {
	public int intAreaId;
	public string strAreaName;
	public string strDescription;
	public string strUrl;
	public string strImgUrl;
}

public class DicBuildingInfo {
	public int intBuildingId;
	public string strBuildingCode;
	public string strBuildingName;
}

public class AreaInfoManager : Singleton<AreaInfoManager> {

	private string strJsonLstDicAreaUrl = "http://" + Config.Instance.strServerIp + "/JsonData/jsonLstDicArea.json";
	private string strJsonDicAreaInfoUrl = "http://" + Config.Instance.strServerIp + "/JsonData/jsonDicAreaInfo.json";
	private System.Action dataReadyCallback;
	private bool isLstDicAreaReady = false;
	private bool isDicAreaInfoReady = false;

	public void initData(System.Action dataReadyCallBackIn) {
		dataReadyCallback = dataReadyCallBackIn;
		Debug.Log ("[AreaInfoManager] initData / strJsonLstDicAreaUrl: " + strJsonLstDicAreaUrl);
		Debug.Log ("[AreaInfoManager] initData / strJsonLstDicAreaUrl: " + strJsonDicAreaInfoUrl);

		DataCenter.Instance.GetJson (strJsonLstDicAreaUrl, OnLoadLstDicAreaReturn);
		DataCenter.Instance.GetJson (strJsonDicAreaInfoUrl, OnLoadDicAreaInfoReturn);
	}

	void OnLoadLstDicAreaReturn(DataResult dataResult) {
		if (!dataResult.IsSuccess)
			return;

		var lstDicAreaData = JsonMapper.ToObject<List<DicAreaInfo>> ((string) dataResult.Result);
		TempDataHolder.Instance.SaveTempData ("lstDicArea", lstDicAreaData);

		var dicBuildings = new Dictionary<string, DicBuildingInfo> ();

		for (int i = 0; i < lstDicAreaData.Count; i++) {
			if (!dicBuildings.ContainsKey(lstDicAreaData[i].strBuildingName)) {
				DicBuildingInfo dicBuldingInfo = new DicBuildingInfo ();
				dicBuldingInfo.intBuildingId = lstDicAreaData [i].intBuildingId;
				dicBuldingInfo.strBuildingName = lstDicAreaData [i].strBuildingName;

				if (lstDicAreaData [i].strBuildingName == "臺灣水域館")
//					dicBuldingInfo.strBuildingCode = "taiwanWaters";
					dicBuldingInfo.strBuildingCode = "color";
				if (lstDicAreaData [i].strBuildingName == "珊瑚王國館")
//					dicBuldingInfo.strBuildingCode = "coralKingdom";
					dicBuldingInfo.strBuildingCode = "color";
				if (lstDicAreaData [i].strBuildingName == "世界水域館")
//					dicBuldingInfo.strBuildingCode = "worldWaters";
					dicBuldingInfo.strBuildingCode = "color";

				dicBuildings [lstDicAreaData [i].strBuildingName] = dicBuldingInfo;
			}
		}

		TempDataHolder.Instance.SaveTempData ("dicBuildings", dicBuildings);
		isLstDicAreaReady = true;
		CheckIsDataReady ();
	}

	void OnLoadDicAreaInfoReturn(DataResult dataResult) {
		if (dataResult.IsSuccess) {
			var lstDicAreaDescription = JsonMapper.ToObject<List<DicAreaDescription>> ((string) dataResult.Result);
			TempDataHolder.Instance.SaveTempData ("lstDicAreaDestcrition", lstDicAreaDescription);
			isDicAreaInfoReady = true;
			CheckIsDataReady ();
		}

	}

	public void CheckIsDataReady() {
		if (isLstDicAreaReady && isDicAreaInfoReady) {
			if (dataReadyCallback != null) {
				dataReadyCallback ();
			}
		}
	}

	public bool IsDataReady() {
		return (isLstDicAreaReady && isDicAreaInfoReady);
	}

	public List<DicAreaInfo> GetAllListAreaInfo() {
		return TempDataHolder.Instance.GetTempData ("lstDicArea") as List<DicAreaInfo>;
	}

	public List<DicAreaInfo> GetLstAreaInfoByBuildingId(int intBuildingId) {
		var lstDicAreaInfoReturn = new List<DicAreaInfo> ();
		var lstDicArea = TempDataHolder.Instance.GetTempData ("lstDicArea") as List<DicAreaInfo>;

		for (int i = 0; i < lstDicArea.Count; i++) {
			if (lstDicArea [i].intBuildingId == intBuildingId) {
				lstDicAreaInfoReturn.Add (lstDicArea [i]);
			}
		}

		return lstDicAreaInfoReturn;
	}

	public DicAreaInfo GetAreaInfoByAreaId(int intAreaId) {
		var lstDicArea = TempDataHolder.Instance.GetTempData ("lstDicArea") as List<DicAreaInfo>;
		for (int i = 0; i < lstDicArea.Count; i++) {
			if (lstDicArea[i].intAreaId == intAreaId) {
				return lstDicArea [i];
			}
		}

		return null;
	}

	public DicAreaDescription GetAreaDescriptionByAreaId(int intAreaId) {
		DicAreaDescription dicAreaDescription = null;
		string strDescription = "";
		var lstDicAreaDescription = TempDataHolder.Instance.GetTempData ("lstDicAreaDestcrition") as List<DicAreaDescription>;
		for (int i = 0; i < lstDicAreaDescription.Count; i++) {
			if (lstDicAreaDescription[i].intAreaId == intAreaId) {
				dicAreaDescription = lstDicAreaDescription [i];
			}
		}

		return dicAreaDescription;
	}

	public DicBuildingInfo GetBuildingInfoByName(string strBuildingName) {
		var _dicBuildings = TempDataHolder.Instance.GetTempData ("dicBuildings") as Dictionary<string, DicBuildingInfo>;
		return _dicBuildings [strBuildingName];
	}

	public int GetBuildingIdByName(string strBuildingName) {
		var _dicBuildings = TempDataHolder.Instance.GetTempData ("dicBuildings") as Dictionary<string, DicBuildingInfo>;
		return _dicBuildings [strBuildingName].intBuildingId;
	}

	public string GetStrBuildingNameByAreaId(int intAreaId) {
		var lstDicArea = TempDataHolder.Instance.GetTempData ("lstDicArea") as List<DicAreaInfo>;

		for (int i = 0; i < lstDicArea.Count; i++) {
			if (lstDicArea [i].intAreaId == intAreaId) {
				return lstDicArea [i].strBuildingName;
			}
		}

		return "";
	}
}
