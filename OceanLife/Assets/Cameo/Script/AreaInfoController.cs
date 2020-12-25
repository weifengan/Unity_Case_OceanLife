using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Cameo;
using LitJson;
using System.IO;

public class AreaInfoController : MonoBehaviour {
	
	public GameObject _content;
	public GameObject _buttonArea;
	public GameObject _btnGotoCreatureList;
	public GameObject _btnViewVideoTour;
	public GameObject _btnOpenUrl;
	public GameObject _funnyQuestionArea;
	public GameObject _btnFunnyQuestionViewAnswer;
	public GameObject _btnFunnyQuestionViewVideo;
	public GameObject _loading;

	public Button BtnVideoPlay01;
	public Button BtnVideoPlay02;
	public Button BtnVideoPlay03;
//	public Button BtnVideoPlay04;
//	public Button BtnVideoPlay05;
	public Image ImgBeaconArea;
	public Image ImgVedioSelect;
//	public Text TextVedioCount;
	public Text _funnyQuestion;
	public Text _description;


	private Dictionary<string, object> _sceneSetting;
	private DicAreaInfo _dicAreaInfo;
	private DicAreaDescription _dicAreaDescription;
	private bool isAttachViewVideoEventChannel = false;
	private bool isAttachViewAnswerEventChannel = false;
	private DicFunnyQuestion _tourVideoQuestion;
	private DicFunnyQuestion _currentFunnyQuestion;
	private GameObject _webViewGameObject;

//	public Text TextTest;

	// Use this for initialization
	void Start () {
		ImgVedioSelect.gameObject.SetActive (false);
		_sceneSetting = SceneNavigator.Instance.GetCurrentSceneSetting ();

		_dicAreaInfo = AreaInfoManager.Instance.GetAreaInfoByAreaId ((int) _sceneSetting["intAreaId"]);
		_dicAreaDescription = AreaInfoManager.Instance.GetAreaDescriptionByAreaId ((int) _sceneSetting["intAreaId"]);
		StartCoroutine(DownLoadBeaconAreaImage(ImgBeaconArea,_dicAreaDescription.strImgUrl));
		Debug.Log ("area= "+_dicAreaDescription.strImgUrl);
		List<DicCreatureBriefInfo> _lstDicCreatureBriefInfo = CreatureDataManager.Instance.GetLstDicCreatureBriefInfoByAreaId (_dicAreaInfo.intAreaId);
		TitlebarController.Instance.SetTitleText (_dicAreaInfo.strAreaName);
		TitlebarController.Instance.SetBackButtonClickCallback (OnBackClick);
		TitlebarController.Instance.ShowTitlebar ();


		_description.text = _dicAreaDescription.strDescription;
//		Debug.Log ("Count= "+_lstDicCreatureBriefInfo.Count.ToString()+"\n"+"0= "+_lstDicCreatureBriefInfo[0].strCreatureName);
		_btnGotoCreatureList.SetActive (!(_lstDicCreatureBriefInfo.Count == 0));

		_tourVideoQuestion = FunnyQuestionManager.Instance.GetAreaTourVideoQuestion (_dicAreaInfo.intAreaId);
//		Debug.Log (_tourVideoQuestion.strVideoUrl01);
		if (_tourVideoQuestion == null || _tourVideoQuestion.strVideoUrl == "") {
			_btnViewVideoTour.SetActive (false);
		} else {
			_btnViewVideoTour.SetActive (true);
		}

		if (_dicAreaDescription.strUrl == null || _dicAreaDescription.strUrl == "") {
			_btnOpenUrl.SetActive (false);
		} else {
			_btnOpenUrl.SetActive (true);
		}

		if (!_btnGotoCreatureList.activeSelf && !_btnViewVideoTour.activeSelf && !_btnOpenUrl.activeSelf) {
			_buttonArea.SetActive (false);
		}

		CheckIsHasFunnyQuesiton ();
		iTweenRectTweener.FadeCanvasGroupAlpha (_content.GetComponent<CanvasGroup>(), 1.2f, 0.3f);
	}

	void OnDestroy () {
		ScreenOrientationHelper.SetOrientationFixed (ScreenOrientation.Portrait);

		if (_webViewGameObject != null) {
			var webView = _webViewGameObject.AddComponent<WebViewObject>();
			//webView.OnLoadComplete -= OnLoadUrlComplete;
			//webView.InsetsForScreenOreitation -= InsetsForScreenOreitation;
			//webView.OnWebViewShouldClose -= WebView_OnWebViewShouldClose;
			//webView.Stop ();
			//webView.Hide ();
		}

		TitlebarController.Instance.RemoveBackButtonClickCallback (OnBackClick);

		if (isAttachViewVideoEventChannel)
			EventChannel.Instance.DetachListner ("onViewTourVideoClick", ViewTourVideo);
		
		if (isAttachViewAnswerEventChannel)
			EventChannel.Instance.DetachListner ("onViewAnswerClick", ViewAnswer);

		_webViewGameObject = null;
	}

	public void OnAreInfoFadeInComplete() {
		Invoke ("CheckIsHasFunnyQuesiton", 0.3f);
	}

	public void CheckIsHasFunnyQuesiton() {
		DicFunnyQuestion dicFunnyQuestion = FunnyQuestionManager.Instance.GetRandomUnreadFunnyQuestionByAreaId(_dicAreaInfo.intAreaId);
		if (dicFunnyQuestion != null) {
			_currentFunnyQuestion = dicFunnyQuestion;
			Debug.Log ("strQuestion= "+_currentFunnyQuestion.strQuestion);
			_funnyQuestion.text = _currentFunnyQuestion.strQuestion;
			_btnFunnyQuestionViewAnswer.SetActive(true);
			//_btnFunnyQuestionViewVideo.SetActive(_currentFunnyQuestion.strVideoUrl != "");;
		} else {
			_funnyQuestionArea.SetActive (false);
		}
	}

	public void CloseVideoSelectPanel()
	{
		ImgVedioSelect.gameObject.SetActive (false);
	}

	public void PlayViedo01()
	{
		PlayViedo (_tourVideoQuestion.strVideoUrl);
	}

	public void PlayViedo02()
	{
		PlayViedo (_tourVideoQuestion.strVideoUrl01);
	}

	public void PlayViedo03()
	{
		PlayViedo (_tourVideoQuestion.strVideoUrl02);
	}

//	public void PlayViedo04()
//	{
//		PlayViedo (_tourVideoQuestion.strVideoUrl03);
//	}
//
//	public void PlayViedo05()
//	{
//		PlayViedo (_tourVideoQuestion.strVideoUrl04);
//	}

	public void PlayViedo(string VedioPath)
	{
		bool isOpenOnWebview = true;
		#if UNITY_IOS
		if (UnityEngine.iOS.Device.generation.ToString().ToLower().Contains("ipad")) {
		isOpenOnWebview = false;
		}
		#endif

		#if UNITY_EDITOR
		isOpenOnWebview = false;
		#endif

		if (isOpenOnWebview) {
			if (_webViewGameObject == null) {
				_webViewGameObject = new GameObject ("WebView");
			}
			var webView = _webViewGameObject.AddComponent<WebViewObject> ();
			//webView.OnLoadComplete += OnLoadUrlComplete;
			//webView.InsetsForScreenOreitation += InsetsForScreenOreitation;
			//webView.OnWebViewShouldClose += WebView_OnWebViewShouldClose;
			//webView.zoomEnable = true;
			//webView.toolBarShow = true;
			//webView.url = _tourVideoQuestion.strVideoUrl;
			//webView.url = VedioPath;

			webView.CallOnStarted(VedioPath);

			_loading.SetActive (true);
			//webView.Load ();
		} 
		else 
		{
//			Application.OpenURL (_tourVideoQuestion.strVideoUrl);
			Application.OpenURL (VedioPath);
		}
	}

	public void ViewTourVideo() {
		//FunnyQuestionManager.Instance.SetFunnyQuestionIsRead (_tourVideoQuestion.intQuestionId);
		GoogleAnalytics.Client.SendEventHit("導覽影片統計", _tourVideoQuestion.strQuestion);
		//GoogleAnalytics.Client.SendEventHit("趣味知識統計", _tourVideoQuestion.strQuestion);
		bool isOpenOnWebview = true;
		#if UNITY_IOS
		if (UnityEngine.iOS.Device.generation.ToString().ToLower().Contains("ipad")) {
			isOpenOnWebview = false;
		}
		#endif

		#if UNITY_EDITOR
		isOpenOnWebview = false;
		#endif

		if (isOpenOnWebview) {
			if (_webViewGameObject == null) {
				_webViewGameObject = new GameObject ("WebView");
			}
			var webView = _webViewGameObject.AddComponent<WebViewObject> ();
			//webView.OnLoadComplete += OnLoadUrlComplete;
			//webView.InsetsForScreenOreitation += InsetsForScreenOreitation;
			//webView.OnWebViewShouldClose += WebView_OnWebViewShouldClose;
			//webView.zoomEnable = true;
			//webView.toolBarShow = true;
			if (_tourVideoQuestion.strVideoUrl != "" && _tourVideoQuestion.strVideoUrl01 == null && _tourVideoQuestion.strVideoUrl02 == null) 
			{
				//webView.url = _tourVideoQuestion.strVideoUrl;
				webView.CallOnStarted(_tourVideoQuestion.strVideoUrl);

				_loading.SetActive (true);
				//webView.Load ();
			}
			else if(_tourVideoQuestion.strVideoUrl != "" && _tourVideoQuestion.strVideoUrl01 != null  && _tourVideoQuestion.strVideoUrl02 == null )
			{
				ImgVedioSelect.gameObject.SetActive (true);
//				TextVedioCount.text = "(2)";
				BtnVideoPlay01.gameObject.SetActive (true);
				BtnVideoPlay02.gameObject.SetActive (true);
				BtnVideoPlay03.gameObject.SetActive (false);
			}	
			else if(_tourVideoQuestion.strVideoUrl != "" && _tourVideoQuestion.strVideoUrl01 != null  && _tourVideoQuestion.strVideoUrl02 != null)
			{
				ImgVedioSelect.gameObject.SetActive (true);
//				TextVedioCount.text = "(3)";
				BtnVideoPlay01.gameObject.SetActive (true);
				BtnVideoPlay02.gameObject.SetActive (true);
				BtnVideoPlay03.gameObject.SetActive (true);
			}
		}
		else 
		{
			if (_tourVideoQuestion.strVideoUrl != "" && _tourVideoQuestion.strVideoUrl01 == null) 
			{
				Application.OpenURL (_tourVideoQuestion.strVideoUrl);
				Debug.Log ("strVideoUrl"+_tourVideoQuestion.strVideoUrl);
			}
			else if(_tourVideoQuestion.strVideoUrl != "" && _tourVideoQuestion.strVideoUrl01 != null  && _tourVideoQuestion.strVideoUrl02 == null )
			{
				ImgVedioSelect.gameObject.SetActive (true);
//				TextVedioCount.text = "(2)";
				BtnVideoPlay01.gameObject.SetActive (true);
				BtnVideoPlay02.gameObject.SetActive (true);
				BtnVideoPlay03.gameObject.SetActive (false);

			}	
			else if(_tourVideoQuestion.strVideoUrl != "" && _tourVideoQuestion.strVideoUrl01 != null  && _tourVideoQuestion.strVideoUrl02 != null )
			{
				ImgVedioSelect.gameObject.SetActive (true);
//				TextVedioCount.text = "(3)";
				BtnVideoPlay01.gameObject.SetActive (true);
				BtnVideoPlay02.gameObject.SetActive (true);
				BtnVideoPlay03.gameObject.SetActive (true);
			}
		}
	}

	bool WebView_OnWebViewShouldClose (WebViewObject webView)
	{
		ScreenOrientationHelper.SetOrientationFixed (ScreenOrientation.Portrait);
		Destroy (_webViewGameObject);
		_webViewGameObject = null;
		return true;
	}

	void OnLoadUrlComplete(WebViewObject webView, bool success, string errorMessage) {
		_loading.SetActive (false);
		if (success) {
			ScreenOrientationHelper.SetOrientationFixed (ScreenOrientation.Landscape);
			//ScreenOrientationHelper.SetOrientationFree ();
			//webView.Show();
		} else {
			Debug.Log("Something wrong in webview loading: " + errorMessage);
		}
	}

	//UniWebViewEdgeInsets InsetsForScreenOreitation(UniWebView webView, UniWebViewOrientation orientation) {

	//	if (orientation == UniWebViewOrientation.Portrait) {
	//		return new UniWebViewEdgeInsets (0, 0, 0, 0);
	//	} else {
	//		return new UniWebViewEdgeInsets(0, 0, 0, 0);
	//	}
	//}

	public void ViewAnswer() {
		MessageBoxManager.Instance.BackgroundColor = new Color (0, 0, 0, 0.8f);
		MessageBoxManager.Instance.ShowMessageBox("FunnyQuestionAnswer", _currentFunnyQuestion.strAnswer);
		FunnyQuestionManager.Instance.SetFunnyQuestionIsRead (_currentFunnyQuestion.intQuestionId);
		GoogleAnalytics.Client.SendEventHit("趣味知識統計", _currentFunnyQuestion.strQuestion);
	}

	public void OnBtnGotoCreatureListClick() {
		FadeOutContentAndBg ("GoCreatureList");
	}

	public void OnOpenUrlClick() {
		Application.OpenURL (_dicAreaDescription.strUrl);
	}

	private void GoCreatureList() {
		DicSceneInfo dicNextSceneInfo;
		Dictionary<string, object> dicSceneSetting = new Dictionary<string, object> ();
		dicSceneSetting.Add ("strBuildingName", (string)_sceneSetting ["strBuildingName"]);
		dicSceneSetting.Add ("intBuildingId", (int)_sceneSetting ["intBuildingId"]);
		dicSceneSetting.Add ("intAreaId", _dicAreaInfo.intAreaId);

		if (CreatureDataManager.Instance.GetIntCreatureNumberByAreaId (_dicAreaInfo.intAreaId) > 1) {
			dicNextSceneInfo = new DicSceneInfo ("CreatureList", dicSceneSetting);
		} else {
			List<DicCreatureBriefInfo> _lstDicCreatureBriefInfo = CreatureDataManager.Instance.GetLstDicCreatureBriefInfoByAreaId (_dicAreaInfo.intAreaId);
			dicSceneSetting.Add ("strCreatureId", _lstDicCreatureBriefInfo[0].strCreatureId);
			GoogleAnalytics.Client.SendEventHit("生物瀏覽統計", _lstDicCreatureBriefInfo[0].strGenusId + " " + _lstDicCreatureBriefInfo[0].strSpeciesId);
			dicNextSceneInfo = new DicSceneInfo ("CreatureInfo", dicSceneSetting);
		}
		string strBuildingName = AreaInfoManager.Instance.GetStrBuildingNameByAreaId (_dicAreaInfo.intAreaId);
		DicBuildingInfo _dicBuildingInfo = AreaInfoManager.Instance.GetBuildingInfoByName (strBuildingName);
		dicNextSceneInfo.strBgImageUrl = "img/areaBg_" + _dicBuildingInfo.strBuildingCode;
		dicNextSceneInfo.BgColor = new Color32 (99, 99, 99, 255);

		SceneNavigator.Instance.PushScene (dicNextSceneInfo);
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
		if (strOnCompeleteFunction == "GoBack") {
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
		iTweenRectTweener.FadeCanvasGroupAlpha (_content.GetComponent<CanvasGroup>(), 0, 0.3f, strOnCompeleteFunction, this.gameObject);
	}

	//下載區域圖片
	IEnumerator DownLoadBeaconAreaImage(Image image, string url)
	{
		WWW www = new WWW(url);
		yield return www;

		if (www.error == null)
		{
			Debug.Log("DownloadImage OK!");
			Texture2D tex2d = www.texture;
			byte[] pngData = tex2d.EncodeToPNG();
			#if UNITY_EDITOR
			File.WriteAllBytes(Application.dataPath + "/Starlite/ImageCache/" + url.GetHashCode(), pngData);
//			#else
//			File.WriteAllBytes(Application.persistentDataPath + "/ImageCache/" +url.GetHashCode(),pngData);
			#endif
			Sprite spr = Sprite.Create(tex2d, new Rect(0, 0, tex2d.width, tex2d.height), new Vector2(0, 0));
			image.sprite = spr;
		}
		else
		{
			Debug.Log("Error:" + www.error);
		}
	}
}
