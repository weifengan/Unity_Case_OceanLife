using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Cameo;
using LitJson;

public class FunnyQuestionAreaListContorller : MonoBehaviour {

	public GameObject _areaListScrollRect;
	public RectTransform _contentHolder;
	public GameObject _areaTitle;

	private RectTransform _areaListScrollRectTransform;
	private List<DicAreaInfo> _lstAllArea;
	private int IntClickAreaId;

	// Use this for initialization
	void Start () {
		TitlebarController.Instance.SetTitleText ("趣味知識");
		TitlebarController.Instance.SetBackButtonClickCallback (OnBackClick);
		TitlebarController.Instance.ShowTitlebar ();

		_lstAllArea = AreaInfoManager.Instance.GetAllListAreaInfo ();
		_areaListScrollRectTransform = _areaListScrollRect.GetComponent<RectTransform> ();

		DicSceneInfo _dicCurrentSceneInfo = SceneNavigator.Instance.GetCurrentSceneInfo ();

		CreateAreaList ();

		if (_dicCurrentSceneInfo.GetSceneSetting ().ContainsKey ("floatContentY")) {
			_contentHolder.anchoredPosition = new Vector2(_contentHolder.anchoredPosition.x, (float) _dicCurrentSceneInfo.GetSceneSetting () ["floatContentY"]);
		}

		if (_dicCurrentSceneInfo.strShowType == SceneNavigator.FROM_POP) {
			_areaListScrollRectTransform.anchoredPosition = new Vector2 (-640, _areaListScrollRectTransform.anchoredPosition.y);
			_areaListScrollRect.gameObject.GetComponent<CanvasGroup> ().alpha = 1;
			Vector2 _moveOutPosition = new Vector2 (0, _areaListScrollRectTransform.anchoredPosition.y);
			iTweenRectTweener.MoveRectTranformTo (_areaListScrollRectTransform, _moveOutPosition, 0.2f);
		} else {
			iTweenRectTweener.FadeCanvasGroupAlpha (_areaListScrollRect.gameObject.GetComponent<CanvasGroup> (), 1.2f, 0.3f);
		}
	}

	void OnDestroy() {
		TitlebarController.Instance.RemoveBackButtonClickCallback (OnBackClick);
	}

	void CreateAreaList() {
		for (int i = 0; i <_lstAllArea.Count; i++) {
			GameObject areaTitle = GameObject.Instantiate (_areaTitle);
			int intQuestionNumber = FunnyQuestionManager.Instance.GetIntFunnyQuestionNumberByAreaAid (_lstAllArea[i].intAreaId);
			string strQuestionNumber = intQuestionNumber.ToString () + " 題";

			areaTitle.GetComponent<FunnyQuestionAreaTitle> ().initColumn (_lstAllArea[i].intAreaId, _lstAllArea [i].strAreaName, strQuestionNumber, OnAreaColumnClick);
			areaTitle.GetComponent<RectTransform> ().SetParent (_contentHolder, false);
		}
	}

	public void OnAreaColumnClick(int _intClickAreaId) {
		IntClickAreaId = _intClickAreaId;
		MoveScrollRectOut ("GotoFunnyQuestionList");
	}

	public void GotoFunnyQuestionList() {
		SaveCurrentScrollPosition ();

		Dictionary<string, object> dicNextSceneSetting = new Dictionary<string, object> ();
		dicNextSceneSetting.Add ("intAreaId", IntClickAreaId);
		var dicNextSceneInfo = new DicSceneInfo ("FunnyQuestionList", dicNextSceneSetting);
		dicNextSceneInfo.strBgImageUrl = "img/mainMenuBg";
		dicNextSceneInfo.BgColor = new Color32 (99, 99, 99, 255);

		SceneNavigator.Instance.PushScene (dicNextSceneInfo);
	}

	private void SaveCurrentScrollPosition() {
		float floatContentY = _contentHolder.anchoredPosition.y;
		Debug.Log ("[FunnyQuestionAreaList] SaveCurrentScrollPosition / floatContentY: " + floatContentY.ToString());

		Dictionary<string, object> _dicCurrentSceneSetting = SceneNavigator.Instance.GetCurrentSceneSetting ();
		if (_dicCurrentSceneSetting.ContainsKey ("floatContentY")) {
			_dicCurrentSceneSetting ["floatContentY"] = floatContentY;
		} else {
			_dicCurrentSceneSetting.Add ("floatContentY", floatContentY);
		}
	}

	public void OnBackClick() {
		FadeOutScrollRectAndBg("GoBack");
	}

	public void GoBack() {
		SceneNavigator.Instance.PopScene ();
	}

	private void FadeOutScrollRectAndBg(string strOnCompeleteFunction) {
		DicSceneInfo _dicNextSceneInfo = SceneNavigator.Instance.GetPrevSceneInfo ();
		BackgroundController.Instance.FadeBackgroundTo (_dicNextSceneInfo.strBgImageUrl, _dicNextSceneInfo.BgColor, 0.3f);

		iTweenRectTweener.FadeCanvasGroupAlpha (_areaListScrollRect.gameObject.GetComponent<CanvasGroup>(), 0, 0.3f, strOnCompeleteFunction, this.gameObject);
	}

	private void MoveScrollRectOut(string strOnCompeleteFunction) {
		Vector2 _moveOutPosition = new Vector2 (_areaListScrollRectTransform.anchoredPosition.x - 640, _areaListScrollRectTransform.anchoredPosition.y);
		iTweenRectTweener.MoveRectTranformTo (_areaListScrollRectTransform, _moveOutPosition, 0.2f, strOnCompeleteFunction, this.gameObject);
	}
}
