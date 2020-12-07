using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Cameo;
using LitJson;

public class FunnyQuestionController : MonoBehaviour {
	
	public GameObject _questionListScrollRect;
	public RectTransform _contentHolder;
	public GameObject _questionColumn;
	public GameObject _seperatorLine;

	private RectTransform _questionListScrollRectTransform;
	private List<DicFunnyQuestion> _lstDicQuestion;
	private int _intAreaId;
	private int _intClickQuestionId;

	// Use this for initialization
	void Start () {
		Dictionary<string, object> _sceneSetting = SceneNavigator.Instance.GetCurrentSceneSetting ();
		_intAreaId = (int)_sceneSetting ["intAreaId"];

		TitlebarController.Instance.SetTitleText ("趣味知識");
		TitlebarController.Instance.SetBackButtonClickCallback (OnBackClick);
		TitlebarController.Instance.ShowTitlebar ();

		_lstDicQuestion = FunnyQuestionManager.Instance.GetLstFunnyQuestionByAreaAid (_intAreaId);

		CreateQuestionList ();

		DicSceneInfo _dicCurrentSceneInfo = SceneNavigator.Instance.GetCurrentSceneInfo ();
		_questionListScrollRectTransform = _questionListScrollRect.GetComponent<RectTransform> ();
		_questionListScrollRectTransform.anchoredPosition = new Vector2 (640, _questionListScrollRectTransform.anchoredPosition.y);
		_questionListScrollRectTransform.gameObject.GetComponent<CanvasGroup> ().alpha = 1;
		Vector2 _moveOutPosition = new Vector2 (0, _questionListScrollRectTransform.anchoredPosition.y);
		iTweenRectTweener.MoveRectTranformTo (_questionListScrollRectTransform, _moveOutPosition, 0.2f);
	}

	void OnDestroy() {
		TitlebarController.Instance.RemoveBackButtonClickCallback (OnBackClick);
	}

	void CreateQuestionList() {
		for (int i = 0; i < _lstDicQuestion.Count; i++) {
			GameObject questionColumn = GameObject.Instantiate (_questionColumn);
			bool isVideo = false;
			if (_lstDicQuestion [i].strVideoUrl != "") {
				isVideo = true;
			}
			string strQuestion = "Q" + (i+1).ToString () + ": " + _lstDicQuestion [i].strQuestion;
			string strAnswer = "A:\n" + _lstDicQuestion [i].strAnswer;
			questionColumn.GetComponent<FunnyQuestionColumn> ().InitColumn (_lstDicQuestion[i].intQuestionId, strQuestion, strAnswer, isVideo, OnColumnClick);
			questionColumn.GetComponent<RectTransform> ().SetParent (_contentHolder, false);

			if (i != _lstDicQuestion.Count - 1) {
				GameObject seperatorLine = GameObject.Instantiate (_seperatorLine);
				seperatorLine.GetComponent<RectTransform> ().SetParent (_contentHolder, false);
			}
		}
	}

	public void OnColumnClick(int _intQuestionId) {
		DicFunnyQuestion _dicFunnyQuestion = FunnyQuestionManager.Instance.GetDicFunnyQuestionByQuestionId (_intQuestionId);

		if (_dicFunnyQuestion.strVideoUrl != "") {
			Application.OpenURL (_dicFunnyQuestion.strVideoUrl);
		}
	}

	public void OnBackClick() {
		MoveScrollRightOut("GoBack");
	}

	public void GoBack() {
		SceneNavigator.Instance.PopScene ();
	}

	private void MoveScrollRightOut(string strOnCompeleteFunction) {
		Vector2 _moveOutPosition = new Vector2 (640, _questionListScrollRectTransform.anchoredPosition.y);
		iTweenRectTweener.MoveRectTranformTo (_questionListScrollRectTransform, _moveOutPosition, 0.2f, strOnCompeleteFunction, this.gameObject);
	}

	private void FadeOutScrollRectAndBg(string strOnCompeleteFunction) {
		string strNextSceneBgImgUrl = "";
		Color32 colorNextSceneColor = new Color32 ();
		if (strOnCompeleteFunction == "GoBack") {
			DicSceneInfo _dicNextSceneInfo = SceneNavigator.Instance.GetPrevSceneInfo ();
			strNextSceneBgImgUrl = _dicNextSceneInfo.strBgImageUrl;
			colorNextSceneColor = _dicNextSceneInfo.BgColor;
		} else {
			strNextSceneBgImgUrl = "img/mainMenuBg";
			colorNextSceneColor = new Color32 (99, 99, 99, 255);
		}

		BackgroundController.Instance.FadeBackgroundTo (strNextSceneBgImgUrl, colorNextSceneColor, 0.3f);
		iTweenRectTweener.FadeCanvasGroupAlpha (_questionListScrollRect.gameObject.GetComponent<CanvasGroup>(), 0, 0.3f, strOnCompeleteFunction, this.gameObject);
	}
}
