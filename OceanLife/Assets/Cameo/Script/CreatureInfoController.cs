using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Cameo;
using LitJson;
using UnityEngine.SceneManagement;

public class CreatureInfoController : MonoBehaviour {

	public Canvas _canvas;
	public GameObject TittleFromBio;
	public Text TextTittleFromBio;
	public GameObject _madiaInfo;
	public ScrollRect _mediaInfoScrollRect;
	public GameObject _mainColumnContainer;
	public GameObject _twoRowContainer;
	public GameObject _creatureInfoContainer;
	public GameObject _creatureTextInfoTitle;
	public GameObject _creatureTextInfoContent;
	public GameObject _finalColumn;
	public GameObject _textInfoPanel;
	public GameObject _textInfoPanelIndicator;
	public ScrollRect _textInfoScrollRect;
	public Button _textInfoPannelSwitch;
	public GameObject _loading;


	private Dictionary<string, object> _sceneSetting;
	private DicCreatureBriefInfo _dicCreatureBriefInfo;
	private List<DicCreatureData> _lstDicCreatureMediaData = new List<DicCreatureData> ();
	private List<DicCreatureData> _lstDicCreatureTextData = new List<DicCreatureData>();
	private bool isTextInfoShow = false;
	private Vector2 _textInfoPanelHidePosition;
	private Vector2 _textInfoPanelShowPosition;
	private int _intItemIndex = 0;
	private int _intTotalIemNumber = 0;
	private GameObject _currentMainColumn = null;
	private GameObject _currentTwoRowColumn = null;
	private string _strUrlAfterInvoke = "";
	private string _strDataTypeAfterInvoke = "";
	private bool isAttachEvent = false;
	private GameObject _webViewGameObject;

	public static bool isBackFromCreatureInfo = false;
	public RectTransform RectCreatureTextInfoTitle;

	// Use this for initialization
	void Start () {
		// Set Text Info Panel Init Position, End Position and ScrollRect Height
		float floatPanelShowAnchoredPositionY = Screen.height - 120 * _canvas.scaleFactor - 110 * _canvas.scaleFactor;
		float floatPanelHeight = floatPanelShowAnchoredPositionY - 20 * _canvas.scaleFactor;
		_textInfoPanelHidePosition = _textInfoPanel.GetComponent<RectTransform>().anchoredPosition;
		_textInfoPanelShowPosition = new Vector2 (_textInfoPanel.GetComponent<RectTransform>().anchoredPosition.x, floatPanelShowAnchoredPositionY/_canvas.scaleFactor);
		_textInfoScrollRect.GetComponent<RectTransform> ().sizeDelta = new Vector2 (_textInfoScrollRect.GetComponent<RectTransform> ().rect.width, floatPanelHeight/_canvas.scaleFactor);
		//讀上一頁的設定
		_sceneSetting = SceneNavigator.Instance.GetCurrentSceneSetting ();
		_dicCreatureBriefInfo = CreatureDataManager.Instance.GetDicCreatureBriefInfoByCreatureId ((string) _sceneSetting ["strCreatureId"]);
		CreatureDataManager.Instance.GetLstDicCreatureDataByCreatureId ((string)_sceneSetting ["strCreatureId"], OnLoadCreatureDataReturn);
		if (PhoneCam.PageNumber == 7) 
		{
			TitlebarController.Instance.gameObject.SetActive(false);
			TittleFromBio.SetActive (true);
			TextTittleFromBio.text = FishTempData.OnClickFishName;
		}
		else
		{
			TitlebarController.Instance.gameObject.SetActive(true);
			TittleFromBio.SetActive (false);
			TitlebarController.Instance.SetTitleText (_dicCreatureBriefInfo.strCreatureName);
			TitlebarController.Instance.SetBackButtonClickCallback (OnBackClick);
			TitlebarController.Instance.ShowTitlebar ();
		}	
	}
	void Update()
	{
		if (PhoneCam.PageNumber == 7 && Input.GetKeyUp(KeyCode.Escape)) 
		{
			SceneNavigator.Instance.gameObject.SetActive (false);
			isBackFromCreatureInfo = true;
			SceneManager.LoadScene ("BioTest");
		}	
	}

	public void BackToBio()
	{
		SceneNavigator.Instance.gameObject.SetActive (false);
		isBackFromCreatureInfo = true;
		SceneManager.LoadScene ("BioTest");
	}

	void OnLoadCreatureDataReturn(List<DicCreatureData> _lstDicCreatureData) {
		for (int i = 0; i < _lstDicCreatureData.Count; i++) {
			if (_lstDicCreatureData [i].strDataType != "text") {
				_lstDicCreatureMediaData.Add (_lstDicCreatureData [i]);
			} else {
				_lstDicCreatureTextData.Add (_lstDicCreatureData [i]);
			}
		}

		_intTotalIemNumber = _lstDicCreatureMediaData.Count;
		Debug.Log ("[CreatureInfotController] OnLoadCreatureDataReturn / _intTotalIemNumber: " + _intTotalIemNumber);
		Invoke ("RemoveLoading", 1.0f);
		LoadThumbnail ();
		CreateTextInfo ();
	}

	void OnDestroy () {
		ScreenOrientationHelper.SetOrientationFixed (ScreenOrientation.Portrait);

		if (_webViewGameObject != null) {
			var webView = _webViewGameObject.AddComponent<UniWebView>();
			webView.OnLoadComplete -= OnLoadUrlComplete;
			webView.InsetsForScreenOreitation -= InsetsForScreenOreitation;
			webView.OnWebViewShouldClose -= WebView_OnWebViewShouldClose;
			webView.Stop ();
			webView.Hide ();
		}

		TitlebarController.Instance.RemoveBackButtonClickCallback (OnBackClick);

		if (isAttachEvent)
			EventChannel.Instance.DetachListner ("OnCollectedMessageClosed", OpenMediaUrl);

		_webViewGameObject = null;
	}

	void LoadThumbnail() {
		for (int i = 0; i < _lstDicCreatureMediaData.Count; i++) {
			string strThumbnailUrl = "";
			if (_lstDicCreatureMediaData [i].strThumbnailUrl != "")
				strThumbnailUrl = _lstDicCreatureMediaData [i].strThumbnailUrl;
			else 
				if (_lstDicCreatureMediaData [i].strDataType == "image")
					strThumbnailUrl = _lstDicCreatureMediaData [i].strValue;
				else
					strThumbnailUrl = _dicCreatureBriefInfo.strThumbnailUrl;

			if (strThumbnailUrl == "") {
				_intTotalIemNumber--;
				Debug.Log ("[CreatureInfotController] LoadThumbnail / _intTotalIemNumber: " + _intTotalIemNumber);
				CheckIsItemCreateComplete ();
				continue;
			}

			Dictionary<string, object> dicParams = new Dictionary<string, object> ();
			dicParams.Add ("intMediaIndex", i);
			DataCenter.Instance.GetThumbnail (strThumbnailUrl, LoadThumbnailReturn, dicParams);
		}
	}

	void LoadThumbnailReturn(DataResult dataResult) {
		Debug.Log ("[CreatureListController] LoadThumbnailReturn / dataResult: " + dataResult.IsSuccess.ToString());
		_intTotalIemNumber--;
		Debug.Log ("[CreatureInfotController] LoadThumbnailReturn / _intTotalIemNumber: " + _intTotalIemNumber);

		if (!dataResult.IsSuccess) {
			CheckIsItemCreateComplete();
			return;
		}

		Dictionary<string, object> dicParams = dataResult.Params;
		int intMediaIndex = (int) dicParams["intMediaIndex"];

		if ((_intItemIndex % 4) == 0 || (_intItemIndex % 4) == 1) {
			_currentMainColumn = CreateMainColumnContainer ();
		}

		if ((_intItemIndex % 8) == 2 || (_intItemIndex % 8) == 5) {
			_currentTwoRowColumn = CreateTwoRowColumnContainer (_currentMainColumn);
		}

		var creatureInfo = GameObject.Instantiate (_creatureInfoContainer);
		Dictionary <string, object> _parameters = new Dictionary<string, object> ();
		_parameters.Add ("intMediaIndex", intMediaIndex);
		_parameters.Add ("strDataType", _lstDicCreatureMediaData [intMediaIndex].strDataType);

		creatureInfo.GetComponent<BaseListItemWithThumbnail> ().InitItem (_parameters, OnCreatureInfoItemClick, (Sprite) dataResult.Result);

		if ((_intItemIndex % 8) == 0 || (_intItemIndex % 8) == 1 || (_intItemIndex % 8) == 4 || (_intItemIndex % 8) == 7) {
			creatureInfo.GetComponent<RectTransform> ().SetParent (_currentMainColumn.transform, false);
		}

		if ((_intItemIndex % 8) == 2 || (_intItemIndex % 8) == 3 || (_intItemIndex % 8) == 5 || (_intItemIndex % 8) == 6) {
			creatureInfo.GetComponent<RectTransform> ().SetParent (_currentTwoRowColumn.transform, false);
		}

		_intItemIndex++;
		CheckIsItemCreateComplete ();
	}

	void CheckIsItemCreateComplete() {
		Debug.Log ("[CreatureInfotController] CheckIsItemCreateComplete / _intTotalIemNumber: " + _intTotalIemNumber);
		if (_intTotalIemNumber == 0) {
			CreateFinalColumnAndRemoveLoading();
		}
	}

	void CreateFinalColumnAndRemoveLoading() {
		CreateFinalColumn ();
		RemoveLoading();
	}

	void CreateFinalColumn() {
		GameObject finalColumn = GameObject.Instantiate (_finalColumn);
		finalColumn.GetComponent<RectTransform> ().SetParent (_mediaInfoScrollRect.content, false);
	}

	void RemoveLoading() {
		_loading.SetActive (false);
	}

	GameObject CreateMainColumnContainer() {
		GameObject _mainColumn = GameObject.Instantiate (_mainColumnContainer);
		_mainColumn.GetComponent<RectTransform> ().SetParent (_mediaInfoScrollRect.content, false);
		return _mainColumn;
	}

	GameObject CreateTwoRowColumnContainer(GameObject _currentMainColumn) {
		GameObject _twoRowColumn = GameObject.Instantiate (_twoRowContainer);
		_twoRowColumn.GetComponent<RectTransform> ().SetParent (_currentMainColumn.transform, false);
		return _twoRowColumn;
	}
	//**************************
	public void CreateTextInfo() {
		float NowY = -40;
		for (int i = 0; i < _lstDicCreatureTextData.Count; i++) {
			if (_lstDicCreatureTextData [i].strName.Length == 2) 
			{
				RectCreatureTextInfoTitle.sizeDelta = new Vector2 (130,60);
			}
			else if(_lstDicCreatureTextData [i].strName.Length == 3) 
			{
				RectCreatureTextInfoTitle.sizeDelta = new Vector2 (165,60);
			}	
			else if(_lstDicCreatureTextData [i].strName.Length == 4) 
			{
				RectCreatureTextInfoTitle.sizeDelta = new Vector2 (205,60);
			}
			else if(_lstDicCreatureTextData [i].strName.Length == 5) 
			{
				RectCreatureTextInfoTitle.sizeDelta = new Vector2 (235,60);
			}
//			else if(_lstDicCreatureTextData [i].strName.Length == 6) 
//			{
//				RectCreatureTextInfoTitle.sizeDelta = new Vector2 (300,60);
//			}
			var creatureInfoTitle = GameObject.Instantiate (_creatureTextInfoTitle);
			creatureInfoTitle.GetComponent<CreatureTextInfoTitle> ().initText(_lstDicCreatureTextData[i].strName);
//			Debug.Log (_lstDicCreatureTextData[i].strName+"***"+_lstDicCreatureTextData[i].strName.Length.ToString());

			creatureInfoTitle.GetComponent<RectTransform> ().SetParent (_textInfoScrollRect.content, false);
			creatureInfoTitle.transform.localPosition = new Vector3 (0,NowY-60*i*2,0);
			NowY = NowY - 60 * i * 2;
			var creatureInfoContent = GameObject.Instantiate (_creatureTextInfoContent);
			creatureInfoContent.GetComponent<CreatureTextInfoContent> ().initText(_lstDicCreatureTextData[i].strValue);
			Debug.Log (_lstDicCreatureTextData[i].strValue+"***"+_lstDicCreatureTextData[i].strValue.Length.ToString());
			creatureInfoContent.GetComponent<RectTransform> ().SetParent (_textInfoScrollRect.content, false);
			creatureInfoContent.transform.localPosition = new Vector3 (30,-40*(i*3+1),0);
		}
	}

	public void OnCreatureInfoItemClick(Dictionary<string, object> parameters) {
		int intMediaIndex = (int) parameters ["intMediaIndex"];
		string strDataType = (string) parameters ["strDataType"];

		Debug.Log ("[CreatureListController] OnCreatureListItemClick / intMediaIndex: " + intMediaIndex.ToString());
		DicCreatureData dicCreatureData = _lstDicCreatureMediaData [intMediaIndex];

		string strEventName = (strDataType == "image") ? "生物圖片瀏覽統計" : "生物影片瀏覽統計";
		GoogleAnalytics.Client.SendEventHit(strEventName, _dicCreatureBriefInfo.strGenusId + " " + _dicCreatureBriefInfo.strSpeciesId, dicCreatureData.strValue);

		if (CreatureDataManager.Instance.CheckIsCreatureCollected (_dicCreatureBriefInfo.strCreatureId)) {
			//Application.OpenURL (dicCreatureData.strValue);
			OpenUrlWithWebView(dicCreatureData.strValue, strDataType);
		} else {
			CreatureDataManager.Instance.SetCreatureCollected (_dicCreatureBriefInfo.strCreatureId);
			MessageBoxManager.Instance.ShowMessageBox("CollectCreature", _dicCreatureBriefInfo.strCreatureName);
			_strUrlAfterInvoke = dicCreatureData.strValue;
			_strDataTypeAfterInvoke = strDataType;

			isAttachEvent = true;
			EventChannel.Instance.AttachListener ("OnCollectedMessageClosed", OpenMediaUrl);
			//Invoke ("OpenMediaUrl", 0.3f);
		}
	}

	void OpenUrlWithWebView(string strUrl, string strDataType) {
		bool isOpenOnWebview = true;

		#if UNITY_IOS
		if (UnityEngine.iOS.Device.generation.ToString().ToLower().Contains("ipad")) {
			isOpenOnWebview = false;
		}
		#endif

		if (strDataType == "image") {
			isOpenOnWebview = true;
		}

		#if UNITY_EDITOR
		isOpenOnWebview = false;
		#endif

		if (isOpenOnWebview) {
			if (_webViewGameObject == null) {
				_webViewGameObject = new GameObject ("WebView");
			}
			var webView = _webViewGameObject.AddComponent<UniWebView> ();
			webView.OnLoadComplete += OnLoadUrlComplete;
			webView.InsetsForScreenOreitation += InsetsForScreenOreitation;
			webView.OnWebViewShouldClose += WebView_OnWebViewShouldClose;
			webView.toolBarShow = true;
			webView.zoomEnable = true;
			webView.url = strUrl;
			_loading.SetActive (true);
			webView.Load ();
		} else {
			Application.OpenURL (strUrl);
		}
	}

	bool WebView_OnWebViewShouldClose (UniWebView webView)
	{
		Destroy (_webViewGameObject);
		_webViewGameObject = null;
		ScreenOrientationHelper.SetOrientationFixed (ScreenOrientation.Portrait);
		return true;
	}

	void OnLoadUrlComplete(UniWebView webView, bool success, string errorMessage) {
		_loading.SetActive (false);
		if (success) {
			ScreenOrientationHelper.SetOrientationFixed (ScreenOrientation.Landscape);
			//ScreenOrientationHelper.SetOrientationFree ();
			webView.Show();
		} else {
			Debug.Log("Something wrong in webview loading: " + errorMessage);
		}
	}

	UniWebViewEdgeInsets InsetsForScreenOreitation(UniWebView webView, UniWebViewOrientation orientation) {
		if (orientation == UniWebViewOrientation.Portrait) {
			return new UniWebViewEdgeInsets (0, 0, 0, 0);
		} else {
			return new UniWebViewEdgeInsets(0, 0, 0, 0);
		}
	}

	void OpenMediaUrl() {
		if (_strUrlAfterInvoke != "") {
			//Application.OpenURL (_strUrlAfterInvoke);
			OpenUrlWithWebView (_strUrlAfterInvoke, _strDataTypeAfterInvoke);
		}
	}

	public void OnBackClick() {
		FadeOutContentAndBg ("GoBack");
	}

	public void GoBack() {
		SceneNavigator.Instance.PopScene ();
	}

	public void OnTextInfoSwitchClick() {
		Hashtable ht = new Hashtable ();
		ht.Add ("from", _textInfoPanel.GetComponent<RectTransform>().anchoredPosition);
		ht.Add ("time", 0.8f);
		ht.Add ("easeType", iTween.EaseType.easeOutQuad);
		ht.Add ("onupdatetarget", this.gameObject);
		ht.Add ("onupdate", "SetTextInfoPanelAnchoredPosition");

		Hashtable indicatorRotateHt = new Hashtable ();
		indicatorRotateHt.Add ("time", 0.3);

		if (isTextInfoShow) {
			ht.Add ("to", _textInfoPanelHidePosition);
			indicatorRotateHt.Add ("z", 90);
			isTextInfoShow = false;
		} else {
			ht.Add ("to", _textInfoPanelShowPosition);
			indicatorRotateHt.Add ("z", 270);
			isTextInfoShow = true;
		}

		iTween.RotateTo (_textInfoPanelIndicator, indicatorRotateHt);
		iTween.ValueTo (_textInfoPanel, ht);
	}

	private void SetTextInfoPanelAnchoredPosition(Vector2 position) {
		_textInfoPanel.GetComponent<RectTransform>().anchoredPosition = position;
	}

	private void FadeOutContentAndBg(string strOnCompeleteFunction) {
		string strNextSceneBgImgUrl = "";
		Color32 colorNextSceneColor = new Color32 ();

		DicSceneInfo _dicNextSceneInfo = SceneNavigator.Instance.GetPrevSceneInfo ();
		strNextSceneBgImgUrl = _dicNextSceneInfo.strBgImageUrl;
		colorNextSceneColor = _dicNextSceneInfo.BgColor;

		BackgroundController.Instance.FadeBackgroundTo (strNextSceneBgImgUrl, colorNextSceneColor, 0.3f);
		iTweenRectTweener.FadeCanvasGroupAlpha (_madiaInfo.gameObject.GetComponent<CanvasGroup>(), 0, 0.3f, strOnCompeleteFunction, this.gameObject);
		iTweenRectTweener.FadeCanvasGroupAlpha (_textInfoPanel.gameObject.GetComponent<CanvasGroup>(), 0, 0.3f);
	}
}
