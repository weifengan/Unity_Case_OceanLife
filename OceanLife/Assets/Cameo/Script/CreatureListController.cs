using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Cameo;
using LitJson;

public class CreatureListController : MonoBehaviour {

	public GameObject _creatureList;
	public GameObject _CreatureListItem;
	public GameObject _CreatureListItem2;
	public RectTransform _LeftColumn;
	public RectTransform _RightColumn;
	public ScrollRect _scrollRect;
	public GameObject _Loading;

	private Dictionary<string, object> _sceneSetting;
	private DicAreaInfo _dicAreaInfo;
	private List<DicCreatureBriefInfo> _lstDicCreatureBriefInfo;
	private float _leftColumnHeight = 0f;
	private float _rightColumnHeight = 0f;
	private int intTotalCreatureNumber = 0;

	private string strClickCreatureId;

	// Use this for initialization
	void Start () {
		_sceneSetting = SceneNavigator.Instance.GetCurrentSceneSetting ();
		_dicAreaInfo = AreaInfoManager.Instance.GetAreaInfoByAreaId ((int) _sceneSetting["intAreaId"]);

		TitlebarController.Instance.SetTitleText (_dicAreaInfo.strAreaName);
		TitlebarController.Instance.SetBackButtonClickCallback (OnBackClick);
		TitlebarController.Instance.ShowTitlebar ();

		_lstDicCreatureBriefInfo = CreatureDataManager.Instance.GetLstDicCreatureBriefInfoByAreaId (_dicAreaInfo.intAreaId);

		intTotalCreatureNumber = _lstDicCreatureBriefInfo.Count;
		_Loading.SetActive (true);
		Invoke ("LoadThumbnail", 0.5f);
	}

	void LoadThumbnail() {
		for (int i = 0; i < _lstDicCreatureBriefInfo.Count; i++) {
			if (_lstDicCreatureBriefInfo [i].strThumbnailUrl == "") {
				intTotalCreatureNumber--;
				CheckIsItemCreateComplete ();
				continue;
			}

			Dictionary<string, object> dicParams = new Dictionary<string, object> ();
			dicParams.Add ("intCreatureIndex", i);
			DataCenter.Instance.GetThumbnail (_lstDicCreatureBriefInfo [i].strThumbnailUrl, LoadThumbnailReturn, dicParams);
		}
	}

	void LoadThumbnailReturn(DataResult dataResult) {
		if (!dataResult.IsSuccess) {
			intTotalCreatureNumber--;
			CheckIsItemCreateComplete ();
			return;
		}

		Dictionary<string, object> dicParams = dataResult.Params;
		int intCreatureIndex = (int) dicParams["intCreatureIndex"];

		GameObject creatureListItem = GameObject.Instantiate (_CreatureListItem2);
		Dictionary <string, object> _parameters = new Dictionary<string, object> ();
		_parameters.Add ("strLabel", _lstDicCreatureBriefInfo [intCreatureIndex].strCreatureName);
		_parameters.Add ("strCreatureId", _lstDicCreatureBriefInfo [intCreatureIndex].strCreatureId);
		creatureListItem.GetComponent<BaseListItemWithThumbnail> ().InitItem (_parameters, OnCreatureListItemClick, (Sprite) dataResult.Result);
		var _floatItemHeight = creatureListItem.GetComponent<LayoutElement> ().preferredHeight;

		if (_leftColumnHeight <= _rightColumnHeight) {
			_leftColumnHeight += _floatItemHeight + 10;
			creatureListItem.GetComponent<RectTransform> ().SetParent (_LeftColumn, false);
		} else {
			_rightColumnHeight += _floatItemHeight + 10;
			creatureListItem.GetComponent<RectTransform> ().SetParent (_RightColumn, false);
		}

		_scrollRect.content.sizeDelta = new Vector2(_scrollRect.content.sizeDelta.x, (Mathf.Max (_leftColumnHeight, _rightColumnHeight) + 10.0f));
		intTotalCreatureNumber--;
		CheckIsItemCreateComplete ();
	}

	void CheckIsItemCreateComplete() {
		if (intTotalCreatureNumber == 0) {
			Invoke ("SetContentPosYAndRemoveLoading", 0.5f);
		}
	}

	void SetContentPosYAndRemoveLoading() {
		if (_sceneSetting.ContainsKey ("floatContentY")) {
			_scrollRect.content.anchoredPosition = new Vector2(_scrollRect.content.anchoredPosition.x, (float) _sceneSetting ["floatContentY"]);
		}
		RemoveLoading ();
	}

	void RemoveLoading() {
		if (_Loading != null)
			Destroy (_Loading);
	}
		
	void OnDestroy () {
		TitlebarController.Instance.RemoveBackButtonClickCallback (OnBackClick);
	}

	public void OnCreatureListItemClick(Dictionary<string, object> parameters) {
		strClickCreatureId = (string) parameters ["strCreautreId"];

		DicCreatureBriefInfo dicCreatureBriefInfo = CreatureDataManager.Instance.GetDicCreatureBriefInfoByCreatureId (strClickCreatureId);
		GoogleAnalytics.Client.SendEventHit("生物瀏覽統計", dicCreatureBriefInfo.strGenusId + " " + dicCreatureBriefInfo.strSpeciesId);
		FadeOutContentAndBg ("GoCreaturInfo");
	}

	private void GoCreaturInfo() {
		SaveCurrentScrollPosition ();

		//Debug.Log ("[CreatureListController] OnCreatureListItemClick / strCreatureId: " + strClickCreatureId);
		//設定下一個場景資訊
		Dictionary<string, object> dicSceneSetting = new Dictionary<string, object> ();
		dicSceneSetting.Add ("strBuildingName", (string) _sceneSetting ["strBuildingName"]);
		dicSceneSetting.Add ("intBuildingId", (int) _sceneSetting ["intBuildingId"]);
		dicSceneSetting.Add ("intAreaId", _dicAreaInfo.intAreaId);
		dicSceneSetting.Add ("strCreatureId", strClickCreatureId);//魚的學名(英文id)
		Debug.Log("strBuildingName= "+_sceneSetting ["strBuildingName"].ToString()+"\n"
			+"intBuildingId= "+_sceneSetting ["intBuildingId"].ToString()+"\n"
			+"intAreaId= "+_dicAreaInfo.intAreaId.ToString()+"\n"
			+"strClickCreatureId= "+strClickCreatureId);

		var dicNextSceneInfo = new DicSceneInfo ("CreatureInfo", dicSceneSetting);
		string strBuildingName = AreaInfoManager.Instance.GetStrBuildingNameByAreaId (_dicAreaInfo.intAreaId);
		DicBuildingInfo _dicBuildingInfo = AreaInfoManager.Instance.GetBuildingInfoByName (strBuildingName);
		dicNextSceneInfo.strBgImageUrl = "img/areaBg_" + _dicBuildingInfo.strBuildingCode;	//下一頁背景圖
		dicNextSceneInfo.BgColor = new Color32 (99, 99, 99, 255);

		SceneNavigator.Instance.PushScene (dicNextSceneInfo);
	}

	private void SaveCurrentScrollPosition() {
		float floatContentY = _scrollRect.content.anchoredPosition.y;

		Dictionary<string, object> _dicCurrentSceneSetting = SceneNavigator.Instance.GetCurrentSceneSetting ();
		if (_dicCurrentSceneSetting.ContainsKey ("floatContentY")) {
			_dicCurrentSceneSetting ["floatContentY"] = floatContentY;
		} else {
			_dicCurrentSceneSetting.Add ("floatContentY", floatContentY);
		}
	}

	public void OnBackClick() {
		FadeOutContentAndBg ("GoBack");
	}

	public void GoBack() {
		SceneNavigator.Instance.PopScene ();
	}

	private void FadeOutContentAndBg(string strOnCompeleteFunction) {
		string strNextSceneBgImgUrl = "";
		Color32 colorNextSceneColor = new Color32 ();
		if (strOnCompeleteFunction == "BackToHome") {
			DicSceneInfo _dicNextSceneInfo = SceneNavigator.Instance.GetPrevSceneInfo ();
			strNextSceneBgImgUrl = _dicNextSceneInfo.strBgImageUrl;
			colorNextSceneColor = _dicNextSceneInfo.BgColor;
		} else {
			string strBuildingName = AreaInfoManager.Instance.GetStrBuildingNameByAreaId (_dicAreaInfo.intAreaId);
			DicBuildingInfo _dicBuildingInfo = AreaInfoManager.Instance.GetBuildingInfoByName (strBuildingName);
			strNextSceneBgImgUrl = "img/areaBg_" + _dicBuildingInfo.strBuildingCode;
			colorNextSceneColor = new Color32 (99, 99, 99, 255);
		}

		BackgroundController.Instance.FadeBackgroundTo (strNextSceneBgImgUrl, colorNextSceneColor, 0.3f);
		iTweenRectTweener.FadeCanvasGroupAlpha (_creatureList.gameObject.GetComponent<CanvasGroup>(), 0, 0.3f, strOnCompeleteFunction, this.gameObject);
	}
}
