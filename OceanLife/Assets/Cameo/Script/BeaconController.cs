using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Cameo;
using LitJson;

public class DicBeaconInfo {
	public int intBuildingId;
	public string strBuildingName;
	public int intAreaId;
	public string strAreaName;
	public int intMajor;
	public int intMinor;
	public string strUuid;
}

public class BeaconController : Singleton<BeaconController> {

	private string strBeaconInfoUrl = "";

	private System.Action dataReadyCallback;
	private bool isBeaconDataReady = false;
	private List<int> _lstAreaId = new List<int>();
	private string strLstCurrentAreaId = "[]";
	private List<string> _lstSceneNameNotChangeLocation = new List<string>();

	public void init(System.Action dataReadyCallBackIn) {
		strBeaconInfoUrl = "http://" + Config.Instance.strServerIp + "/JsonData/jsonBeacons.json";

		_lstSceneNameNotChangeLocation.Add ("CreatureList");
		_lstSceneNameNotChangeLocation.Add ("CreatureInfo");

		dataReadyCallback = dataReadyCallBackIn;
		Debug.Log ("[BeaconController] initData / strBeaconInfoUrl: " + strBeaconInfoUrl);

		DataCenter.Instance.GetJson (strBeaconInfoUrl, OnLoadLstDicBeaconInfoReturn);
	}

	void OnLoadLstDicBeaconInfoReturn(DataResult dataResult) {
		if (!dataResult.IsSuccess)
			return;

		var lstDicBeaconInfo = JsonMapper.ToObject<List<DicBeaconInfo>> ((string) dataResult.Result);
		TempDataHolder.Instance.SaveTempData ("lstDicBeaconInfo", lstDicBeaconInfo);

		if (isBeaconDataReady) {
			return;
		}

		Cameo.Sensor.Instance.Initialize (Config.Instance.isTestBeacon);
		isBeaconDataReady = true;

		if (dataReadyCallback != null) {
			dataReadyCallback ();
		}
	}

	public bool IsDataReady() {
		return isBeaconDataReady;
	}

	public void OnLocationChange() {
		_lstAreaId = Cameo.LocationManager.Instance.lstIntCurrentAreaIds;
		string strLstNewAreaId = JsonMapper.ToJson (_lstAreaId);

		Debug.Log ("[BeaconController] OnLocationChange / strLstNewAreaId: " + strLstNewAreaId);
		Debug.Log ("[BeaconController] OnLocationChange / strLstCurrentAreaId: " + strLstCurrentAreaId);

		if (strLstNewAreaId != strLstCurrentAreaId) {
			Debug.Log ("[BeaconController] OnLocationChange / strLstNewAreaId and strLstCurrentAreaId are different!");

			if (!IsNeedChangeLocation ())
				return;
			
			strLstCurrentAreaId = strLstNewAreaId;
			HandleLocationChange ();
		}
	}

	private bool IsNeedChangeLocation() {
		string strCurrentSceneName = SceneNavigator.Instance.GetStrCurrentSceneName ();

		// 判斷目前 App 頁面是否停留在生物列表或生物介紹頁
		if (_lstSceneNameNotChangeLocation.Contains (strCurrentSceneName)) {
			Dictionary<string, object> _sceneSetting = SceneNavigator.Instance.GetCurrentSceneSetting ();
			int intAreaId;

			if (strCurrentSceneName == "CreatureList") {
				intAreaId = (int)_sceneSetting ["intAreaId"];
			} else {
				DicCreatureBriefInfo _dicCreatureBriefInfo = CreatureDataManager.Instance.GetDicCreatureBriefInfoByCreatureId ((string) _sceneSetting ["strCreatureId"]);
				intAreaId = _dicCreatureBriefInfo.intAreaId;
			}

			// 判斷目前使用者所在的區域包含在目前 Beacon 所發送的區域列表內
			if (_lstAreaId.Contains (intAreaId))
				return false;

			// 表示目前 App 雖然頁面在生物列表或生物介紹頁
			// 但是所在區域並不在目前 Beacon 回傳的區域列表內
			return true;
		}

		// 目前 App 的頁面不在生物列表或是生物介紹頁
		return true;
	}

	private void HandleLocationChange() {
		if (_lstAreaId.Count < 1)
			return;

		if (_lstAreaId.Count == 1) {
			GotoAreaInfoPage ();
		} else {
			GotoAreaListForBeaconPage ();
		}
	}

	public void GotoAreaListForBeaconPage() {
		Dictionary<string, object> dicSceneSetting = new Dictionary<string, object> ();
		dicSceneSetting.Add ("lstAreaId", _lstAreaId);
		var dicSceneInfo = new DicSceneInfo ("AreaListForBeacon", dicSceneSetting);
		dicSceneInfo.strBgImageUrl = "img/mainMenuBg";
		dicSceneInfo.BgColor = new Color32 (69, 162, 180, 255);
		BackgroundController.Instance.SetCurrentBackgroundImage ("img/mainMenuBg", new Color32 (69, 162, 180, 255));
		SceneNavigator.Instance.PushScene (dicSceneInfo);
	}

	public void GotoAreaInfoPage() {
		DicAreaInfo _dicAreaInfo = AreaInfoManager.Instance.GetAreaInfoByAreaId (_lstAreaId [0]);
		int _intBuildingId = AreaInfoManager.Instance.GetBuildingIdByName (_dicAreaInfo.strBuildingName);

		Dictionary<string, object> dicSceneSetting = new Dictionary<string, object> ();
		dicSceneSetting.Add ("strBuildingName", _dicAreaInfo.strBuildingName);
		dicSceneSetting.Add ("intBuildingId", _intBuildingId);
		dicSceneSetting.Add ("intAreaId", _lstAreaId [0]);
		var dicSceneInfo = new DicSceneInfo ("AreaInfo", dicSceneSetting);

		string strBuildingName = AreaInfoManager.Instance.GetStrBuildingNameByAreaId (_lstAreaId [0]);
		DicBuildingInfo _dicBuildingInfo = AreaInfoManager.Instance.GetBuildingInfoByName (strBuildingName);
		dicSceneInfo.strBgImageUrl = "img/areaBg_" + _dicBuildingInfo.strBuildingCode;
		dicSceneInfo.BgColor = new Color32 (99, 99, 99, 255);

		BackgroundController.Instance.SetCurrentBackgroundImage ("img/areaBg_" + _dicBuildingInfo.strBuildingCode, new Color32 (99, 99, 99, 255));
		SceneNavigator.Instance.PushScene (dicSceneInfo);
	}
}
