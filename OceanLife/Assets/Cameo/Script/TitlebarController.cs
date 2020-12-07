using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Cameo;
using LitJson;

public class TitlebarController : Singleton<TitlebarController> {

	public delegate void OnBackButtonClickCallback();
	private OnBackButtonClickCallback _onBackButtonClickCallback = delegate { };

	public const string TITLEBAR_HOME_BUTTON = "TitlebarController.TITLEBAR_HOME_BUTTON";
	public const string TITLEBAR_BACK_BUTTON = "TitlebarController.TITLEBAR_BACK_BUTTON";
	public const string TITLEBAR_BEACON_BUTTON = "TitlebarController.TITLEBAR_BEACON_BUTTON";

	public GameObject _titlebar;
	public GameObject _homeButton;
	public GameObject _backButton;
	public GameObject _beaconButton;
	public Text _titlebarText;

	private Vector2 _showPosition;
	private Vector2 _hidePosition;
	private RectTransform _titlebarRectTransform;
	private bool isShowBeaconAreaMessageBox = false;

	// Use this for initialization
	void Start () {
		GameObject.DontDestroyOnLoad (gameObject);
		_titlebarRectTransform = _titlebar.GetComponent<RectTransform> ();
		_showPosition = new Vector2 (_titlebarRectTransform.anchoredPosition.x, -80);
		_hidePosition = new Vector2 (_titlebarRectTransform.anchoredPosition.x, 0);
		Cameo.Sensor.CameoLocationChangedEvent += OnLocationChange;
	}

	public void SetTitleText(string strTitle) {
		_titlebarText.text = strTitle;
	}

	public void SetTitlebarLeftButton(string strLeftButtonType) {
		switch (strLeftButtonType)
		{
			case TitlebarController.TITLEBAR_HOME_BUTTON:
				if (!_homeButton.activeSelf)
					ChangeLeftFromBackButtonToHome ();
				break;
			case TitlebarController.TITLEBAR_BACK_BUTTON:
				if (!_backButton.activeSelf)
					ChangeLeftFromHomeButtonToBack ();
				break;
		}
	}

	private void ChangeLeftFromBackButtonToHome() {
		Hashtable backButtonRotateHt = new Hashtable ();
		backButtonRotateHt.Add ("z", -180);
		backButtonRotateHt.Add ("time", 0.4);

		Hashtable backButtonScaleHt = new Hashtable ();
		backButtonScaleHt.Add ("x", 0);
		backButtonScaleHt.Add ("y", 0);
		backButtonScaleHt.Add ("time", 0.4);
		backButtonScaleHt.Add ("oncompletetarget", this.gameObject);
		backButtonScaleHt.Add ("oncomplete", "SetBackButtonActiveFalse");

		Hashtable homeButtonRotateHt = new Hashtable ();
		homeButtonRotateHt.Add ("z", 0);
		homeButtonRotateHt.Add ("time", 0.4);
		
		Hashtable homeButtonScaleHt = new Hashtable ();
		homeButtonScaleHt.Add ("x", 1);
		homeButtonScaleHt.Add ("y", 1);
		homeButtonScaleHt.Add ("time", 0.4);
		
		_homeButton.SetActive (true);

		iTween.ScaleTo (_homeButton, homeButtonScaleHt);
		iTween.RotateTo (_homeButton, homeButtonRotateHt);
		
		iTween.ScaleTo (_backButton, backButtonScaleHt);
		iTween.RotateTo (_backButton, backButtonRotateHt);
	}

	private void ChangeLeftFromHomeButtonToBack() {
		Hashtable homeButtonRotateHt = new Hashtable ();
		homeButtonRotateHt.Add ("z", -180);
		homeButtonRotateHt.Add ("time", 0.4);

		Hashtable homeButtonScaleHt = new Hashtable ();
		homeButtonScaleHt.Add ("x", 0);
		homeButtonScaleHt.Add ("y", 0);
		homeButtonScaleHt.Add ("time", 0.4);
		homeButtonScaleHt.Add ("oncomplete", "SetHomeButtonActiveFalse");
		homeButtonScaleHt.Add ("oncompletetarget", this.gameObject);

		Hashtable backButtonRotateHt = new Hashtable ();
		backButtonRotateHt.Add ("z", 0);
		backButtonRotateHt.Add ("time", 0.4);

		Hashtable backButtonScaleHt = new Hashtable ();
		backButtonScaleHt.Add ("x", 1);
		backButtonScaleHt.Add ("y", 1);
		backButtonScaleHt.Add ("time", 0.4);

		_backButton.SetActive (true);

		iTween.ScaleTo (_backButton, backButtonScaleHt);
		iTween.RotateTo (_backButton, backButtonRotateHt);

		iTween.ScaleTo (_homeButton, homeButtonScaleHt);
		iTween.RotateTo (_homeButton, homeButtonRotateHt);
	}

	public void SetHomeButtonActiveFalse() {
		_homeButton.SetActive (false);
	}

	public void SetBackButtonActiveFalse() {
		_backButton.SetActive (false);
	}

	public void ShowTitlebar() {
		Hashtable htMove = new Hashtable ();
		htMove.Add ("from", _titlebarRectTransform.anchoredPosition);
		htMove.Add ("to", _showPosition);
		htMove.Add ("time", 0.3);
		htMove.Add ("easeType", iTween.EaseType.easeOutQuad);
		htMove.Add ("onupdatetarget", this.gameObject);
		htMove.Add ("onupdate", "SetTitlebarAnchoredPosition");
		iTween.ValueTo (_titlebar, htMove);
	}

	public void HideTitlebar() {
		Hashtable htMove = new Hashtable ();
		htMove.Add ("from", _titlebarRectTransform.anchoredPosition);
		htMove.Add ("to", _hidePosition);
		htMove.Add ("time", 0.3);
		htMove.Add ("easeType", iTween.EaseType.easeOutQuad);
		htMove.Add ("onupdatetarget", this.gameObject);
		htMove.Add ("onupdate", "SetTitlebarAnchoredPosition");
		iTween.ValueTo (_titlebar, htMove);
	}

	public void SetTitlebarAnchoredPosition(Vector2 _position) {
		_titlebarRectTransform.anchoredPosition = _position;
	}

	public void SetBackButtonClickCallback(OnBackButtonClickCallback onBackButtonClickCallback) {
		_onBackButtonClickCallback += onBackButtonClickCallback;
	}

	public void RemoveBackButtonClickCallback(OnBackButtonClickCallback onBackButtonClickCallback) {
		_onBackButtonClickCallback -= onBackButtonClickCallback;
	}

	public void OnBackButtonClick() {
		_onBackButtonClickCallback ();
	}

	public void OnHomeButtionClick() {
		_onBackButtonClickCallback ();
	}

	public void OnBeaconButtonClick() {
		if (isShowBeaconAreaMessageBox)
			return;

        List<int> _lstAreaId = Cameo.LocationManager.Instance.lstIntCurrentAreaIds;

        //測試更改AreaId
     
//        List<int> _lstAreaId = new List<int>();
//		Debug.Log ("intCurrentAreaId = ");
//		for (int i = 0; i < _lstAreaId.Count; i++) 
//		{
//			_lstAreaId.Add(_lstAreaId[i]);
//		}	
//		_lstAreaId.Add(3);

        isShowBeaconAreaMessageBox = true;
		EventChannel.Instance.AttachListener ("BeaconAreaMessageBoxClose", OnBeaconAreaMessageBoxClosed);
		MessageBoxManager.Instance.BackgroundColor = new Color (0, 0, 0, 0);
		MessageBoxManager.Instance.ShowMessageBox("BeaconAreaMessageBox", _lstAreaId, false);
		Invoke ("IsBeaconAreaMessageBoxShow", 3.0f);
	}

	void OnBeaconAreaMessageBoxClosed() {
		CancelInvoke ("IsBeaconAreaMessageBoxShow");
		EventChannel.Instance.DetachListner("BeaconAreaMessageBoxClose", OnBeaconAreaMessageBoxClosed);
		isShowBeaconAreaMessageBox = false;
	}

	public void OnLocationChange() {
		List<int> _lstAreaId = Cameo.LocationManager.Instance.lstIntCurrentAreaIds;
		if (_lstAreaId.Count != 0) {
			_beaconButton.GetComponent<Animator> ().SetBool ("isPlay", true);
			Invoke ("OnLocationChange", 3.0f);
		} else {
			_beaconButton.GetComponent<Animator> ().SetBool ("isPlay", false);
		}
	}

	void IsBeaconAreaMessageBoxShow() {
		if (isShowBeaconAreaMessageBox) {
			EventChannel.Instance.Invoke ("CloseBeaconAreaMessageBox");
		}
	}
}
