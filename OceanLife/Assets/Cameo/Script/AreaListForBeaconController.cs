using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Cameo;
using LitJson;

public class AreaListForBeaconController : MonoBehaviour {
	
	public GameObject _areaListScrollRect;
	public RectTransform _contentHolder;
	public GameObject _areaColumn;

	private Dictionary<string, object> _sceneSetting;
	private List<int> _listAreaId;
	private int _intClickAreaId;

	// Use this for initialization
	void Start () {
		_sceneSetting = SceneNavigator.Instance.GetCurrentSceneSetting ();

		TitlebarController.Instance.SetTitleText ("目前您附近的展覽點");
		TitlebarController.Instance.SetBackButtonClickCallback (OnBackClick);
		TitlebarController.Instance.ShowTitlebar ();

		_listAreaId = (List<int>)_sceneSetting ["lstAreaId"];

		createAreaList ();

		iTweenRectTweener.FadeCanvasGroupAlpha (_areaListScrollRect.gameObject.GetComponent<CanvasGroup>(), 1.2f, 0.3f);
	}

	void OnDestroy() {
		TitlebarController.Instance.RemoveBackButtonClickCallback (OnBackClick);
	}

	void createAreaList() {
		for (int i = 0; i < _listAreaId.Count; i++) {
			GameObject areaColumn = GameObject.Instantiate (_areaColumn);
			DicAreaInfo _dicAreaInfo = AreaInfoManager.Instance.GetAreaInfoByAreaId (_listAreaId[i]);
			areaColumn.GetComponent<AreaColumn> ().InitAreaColumn (_dicAreaInfo.strAreaName, _dicAreaInfo.intAreaId, OnAreaColumnClick);
			areaColumn.GetComponent<RectTransform> ().SetParent (_contentHolder, false);
		}
	}

	private void OnAreaColumnClick(int intAreaId) {
		_intClickAreaId = intAreaId;
		FadeOutScrollRectAndBg("GotoAreaInfo");
	}

	public void GotoAreaInfo() {
		DicAreaInfo _dicAreaInfo = AreaInfoManager.Instance.GetAreaInfoByAreaId (_intClickAreaId);
		int _intBuildingId = AreaInfoManager.Instance.GetBuildingIdByName (_dicAreaInfo.strBuildingName);

		Dictionary<string, object> dicSceneSetting = new Dictionary<string, object> ();
		dicSceneSetting.Add ("strBuildingName", _dicAreaInfo.strBuildingName);
		dicSceneSetting.Add ("intBuildingId", _intBuildingId);
		dicSceneSetting.Add ("intAreaId", _intClickAreaId);
		var dicNextSceneInfo = new DicSceneInfo ("AreaInfo", dicSceneSetting);
		string strBuildingName = AreaInfoManager.Instance.GetStrBuildingNameByAreaId (_intClickAreaId);
		DicBuildingInfo _dicBuildingInfo = AreaInfoManager.Instance.GetBuildingInfoByName (strBuildingName);
		dicNextSceneInfo.strBgImageUrl = "img/areaBg_" + _dicBuildingInfo.strBuildingCode;
		dicNextSceneInfo.BgColor = new Color32 (99, 99, 99, 255);
		SceneNavigator.Instance.PushScene (dicNextSceneInfo);
	}

	public void OnBackClick() {
		FadeOutScrollRectAndBg("GoBack");
	}

	public void GoBack() {
		SceneNavigator.Instance.PopScene ();
	}

	private void FadeOutScrollRectAndBg(string strOnCompeleteFunction) {
		string strNextSceneBgImgUrl = "";
		Color32 colorNextSceneColor = new Color32 ();
		if (strOnCompeleteFunction == "GoBack") {
			DicSceneInfo _dicNextSceneInfo = SceneNavigator.Instance.GetPrevSceneInfo ();
			strNextSceneBgImgUrl = _dicNextSceneInfo.strBgImageUrl;
			colorNextSceneColor = _dicNextSceneInfo.BgColor;
		} else {
			string strBuildingName = AreaInfoManager.Instance.GetStrBuildingNameByAreaId (_intClickAreaId);
			DicBuildingInfo _dicBuildingInfo = AreaInfoManager.Instance.GetBuildingInfoByName (strBuildingName);
			strNextSceneBgImgUrl = "img/areaBg_" + _dicBuildingInfo.strBuildingCode;
			colorNextSceneColor = new Color32 (99, 99, 99, 255);
		}

		BackgroundController.Instance.FadeBackgroundTo (strNextSceneBgImgUrl, colorNextSceneColor, 0.3f);
		iTweenRectTweener.FadeCanvasGroupAlpha (_areaListScrollRect.gameObject.GetComponent<CanvasGroup>(), 0, 0.3f, strOnCompeleteFunction, this.gameObject);
	}
}
