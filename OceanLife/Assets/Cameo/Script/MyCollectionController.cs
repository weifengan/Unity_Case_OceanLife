using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Cameo;
using LitJson;

public class MyCollectionController : MonoBehaviour {

	public GameObject _areaColumn;
	public Text _nameTitleHolder;
	public Text _collectNumberHolder;
	public Image _medalIcon;
	public ScrollRect _collectionInfoScrollRect;

	private List<DicAreaInfo> _lstDicAreaInfo;
	private Dictionary<string, object> dicNextSceneSetting;
	private int intAllCreatureNumber;
	private int intCollectedNumner;

	// Use this for initialization
	void Start () {
		TitlebarController.Instance.SetTitleText ("我的收藏");
		TitlebarController.Instance.SetBackButtonClickCallback (OnBackClick);
		TitlebarController.Instance.ShowTitlebar ();

		intAllCreatureNumber = CreatureDataManager.Instance.GetIntAllCreatureNumber ();
		intCollectedNumner = CreatureDataManager.Instance.GetCollectionNumber ();
		float floatLevel = (float) intCollectedNumner / (float) intAllCreatureNumber;
		int intLevel = Mathf.FloorToInt (floatLevel * 7f) + 1;

		Debug.Log ("[MyCollectionController] Start / intCollectedNumner: " + intCollectedNumner);
		Debug.Log ("[MyCollectionController] Start / intAllCreatureNumber: " + intAllCreatureNumber);
		Debug.Log ("[MyCollectionController] Start / floatLevel: " + floatLevel.ToString());
		Debug.Log ("[MyCollectionController] Start / intLevel: " + intLevel.ToString());

		_nameTitleHolder.text = "你目前的海洋生物知識為等級 " + intLevel.ToString();

		if (intCollectedNumner <= 0) {
			_collectNumberHolder.text = "還沒收集到任何生物，加油！";
		} else {
			_collectNumberHolder.text = "你已收集 " + intCollectedNumner.ToString () + " 生物！";
		}

		string _strImageUrl = "img/ui/medal_level_" + (intLevel-1).ToString();
		_medalIcon.sprite = ResourceManager.Instance.GetAsset (_strImageUrl, typeof(Sprite)) as Sprite;

		_lstDicAreaInfo = AreaInfoManager.Instance.GetAllListAreaInfo ();
		CreateCollectionInfoList ();

		iTweenRectTweener.FadeCanvasGroupAlpha (_collectionInfoScrollRect.gameObject.GetComponent<CanvasGroup>(), 1.2f, 0.3f);
	}

	void OnDestroy() {
		TitlebarController.Instance.RemoveBackButtonClickCallback (OnBackClick);
	}

	private void CreateCollectionInfoList() {
		for (int i = 0; i < _lstDicAreaInfo.Count; i++) {
			int intAreaCollectedNumber = CreatureDataManager.Instance.GetIntAreaCollectedNumberByAreaId (_lstDicAreaInfo [i].intAreaId);

			if (intAreaCollectedNumber == 0)
				continue;
			
			string strColumnLabel = _lstDicAreaInfo [i].strAreaName + " (" + intAreaCollectedNumber.ToString() + ")";
			GameObject areaColumn = GameObject.Instantiate (_areaColumn);
			areaColumn.GetComponent<AreaColumn> ().InitAreaColumn (strColumnLabel, _lstDicAreaInfo [i].intAreaId, OnAreaColumnClick);
			areaColumn.GetComponent<RectTransform> ().SetParent (_collectionInfoScrollRect.content, false);
		}
	}

	private void OnAreaColumnClick(int intAreaId) {
		string strBuildingName = AreaInfoManager.Instance.GetStrBuildingNameByAreaId (intAreaId);
		DicBuildingInfo _dicBuildingInfo = AreaInfoManager.Instance.GetBuildingInfoByName (strBuildingName);

		dicNextSceneSetting = new Dictionary<string, object> ();
		dicNextSceneSetting.Add ("strBuildingName", strBuildingName);
		dicNextSceneSetting.Add ("intBuildingId", _dicBuildingInfo.intBuildingId);
		dicNextSceneSetting.Add ("intAreaId", intAreaId);

		FadeOutScrollRectAndBg("GotoCreatureList");
	}

	public void GotoCreatureList() {
		var dicNextSceneInfo = new DicSceneInfo ("CreatureList", dicNextSceneSetting);
		string strBuildingName = AreaInfoManager.Instance.GetStrBuildingNameByAreaId ((int) dicNextSceneSetting["intAreaId"]);
		DicBuildingInfo _dicBuildingInfo = AreaInfoManager.Instance.GetBuildingInfoByName (strBuildingName);
		dicNextSceneInfo.strBgImageUrl = "img/areaBg_" + _dicBuildingInfo.strBuildingCode;
		dicNextSceneInfo.BgColor = new Color32 (99, 99, 99, 255);

		SceneNavigator.Instance.PushScene (dicNextSceneInfo);
	}

	public void OnBackClick() {
		FadeOutScrollRectAndBg("BackToHome");
	}

	private void FadeOutScrollRectAndBg(string strOnCompeleteFunction) {
		string strNextSceneBgImgUrl = "";
		Color32 colorNextSceneColor = new Color32 ();
		if (strOnCompeleteFunction == "BackToHome") {
			DicSceneInfo _dicNextSceneInfo = SceneNavigator.Instance.GetPrevSceneInfo ();
			strNextSceneBgImgUrl = _dicNextSceneInfo.strBgImageUrl;
			colorNextSceneColor = _dicNextSceneInfo.BgColor;
		} else {
			string strBuildingName = AreaInfoManager.Instance.GetStrBuildingNameByAreaId ((int) dicNextSceneSetting["intAreaId"]);
			DicBuildingInfo _dicBuildingInfo = AreaInfoManager.Instance.GetBuildingInfoByName (strBuildingName);
			strNextSceneBgImgUrl = "img/areaBg_" + _dicBuildingInfo.strBuildingCode;
			colorNextSceneColor = new Color32 (99, 99, 99, 255);
		}

		BackgroundController.Instance.FadeBackgroundTo (strNextSceneBgImgUrl, colorNextSceneColor, 0.3f);
		iTweenRectTweener.FadeCanvasGroupAlpha (_collectionInfoScrollRect.gameObject.GetComponent<CanvasGroup>(), 0, 0.3f, strOnCompeleteFunction, this.gameObject);
	}

	public void BackToHome() {
		SceneNavigator.Instance.PopScene ();
	}
}
