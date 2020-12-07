using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Cameo;

public class DicSceneInfo {
	
	public string strSceneName;
	private Dictionary<string, object> dicSceneSettings = new Dictionary<string, object> ();
	public string strShowType = "SceneNavigator.FROM_PUSH";
	public string strBgImageUrl = "img/mainMenuBg";
	public Color32 BgColor = new Color32(255, 255, 255, 255);

	public DicSceneInfo(string _strScenename, Dictionary<string, object> _dicSceneSettings, string _strBgImageUrl = "") {
		strSceneName = _strScenename;
		dicSceneSettings = _dicSceneSettings;

		if (_strBgImageUrl != "")
			strBgImageUrl = _strBgImageUrl;
	}

	public Dictionary<string, object> GetSceneSetting() {
		return dicSceneSettings;
	}

	public void SetBgColor(Color32 _bgColor) {
		BgColor = _bgColor;
	}
}

public class SceneNavigator : Singleton<SceneNavigator> {
	public static bool IsAndroidBackClick;
	public const string FROM_PUSH = "SceneNavigator.FROM_PUSH";
	public const string FROM_POP = "SceneNavigator.FROM_POP";
	private Dictionary<string, int> _dicSceneLevel = new Dictionary<string, int> ();

	List<DicSceneInfo> _lstSceneInfo = new List<DicSceneInfo>();


	void Start() {
		_dicSceneLevel.Add ("Main", 0);
		_dicSceneLevel.Add ("BioTest",1);
		_dicSceneLevel.Add ("AreaList", 1);
		_dicSceneLevel.Add ("AreaListForBeacon", 1);
		_dicSceneLevel.Add ("MyCollection", 1);
		_dicSceneLevel.Add ("FunnyQuestionAreaList", 1);
		_dicSceneLevel.Add ("About", 1);
		_dicSceneLevel.Add ("Course", 1);
		_dicSceneLevel.Add ("FunnyQuestionList", 2);
		_dicSceneLevel.Add ("AreaInfo", 2);
		_dicSceneLevel.Add ("Question", 2);
		_dicSceneLevel.Add ("CreatureList", 3);
		_dicSceneLevel.Add ("CreatureInfo", 4);
	}

	void Update () {
		// 20160624 bigcookie: 處理 Android 系統按下 back 返回上一頁，若已到首頁則離開 App
		// 參考 http://answers.unity3d.com/questions/369198/how-to-exit-application-in-android-on-back-button.html
		if (Input.GetKeyUp(KeyCode.Escape)) 
		{
			if (_lstSceneInfo.Count > 1) 
			{
				if (GetVideo.isPlayBlueVideo) 
				{
					VideoManagerSingleton.Instance.OnEnd ();
					GetVideo.isPlayBlueVideo = false;
				}
				IsAndroidBackClick = true;
				Debug.Log ("UseAndroidBack");
				BackgroundController.Instance.gameObject.SetActive (true);
				DicSceneInfo _dicNextSceneInfo = GetPrevSceneInfo ();
				BackgroundController.Instance.FadeBackgroundTo (_dicNextSceneInfo.strBgImageUrl, _dicNextSceneInfo.BgColor, 0.3f);
				PopScene ();
			} 
			else 
			{	
				if (PhoneCam.PageNumber > 0 && PhoneCam.PageNumber < 8 ) 
				{

				}
				else
				{
					Debug.Log ("QuitAPP");
					Application.Quit();
				}
			}
		}
	}

	public void PushScene(DicSceneInfo dicSceneInfo) {
		bool isPrevScenecLevelGreaterAndEqualThanNewScene = true;

		while(isPrevScenecLevelGreaterAndEqualThanNewScene) 
		{
			DicSceneInfo _dicPrevSceneInfo = null;
			if (_lstSceneInfo.Count > 0)
				_dicPrevSceneInfo = _lstSceneInfo [_lstSceneInfo.Count - 1];

			if (_dicPrevSceneInfo != null) {
				if (_dicSceneLevel [_dicPrevSceneInfo.strSceneName] >= _dicSceneLevel [dicSceneInfo.strSceneName]) 
				{
					_lstSceneInfo.RemoveAt (_lstSceneInfo.Count-1);
					isPrevScenecLevelGreaterAndEqualThanNewScene = true;
				} 
				else 
				{
					isPrevScenecLevelGreaterAndEqualThanNewScene = false;
				}
			} 
			else 
			{
				isPrevScenecLevelGreaterAndEqualThanNewScene = false;
			}
		}

		_lstSceneInfo.Add (dicSceneInfo);
		SetTitlebarLeftButton ();
		SceneManager.LoadScene (dicSceneInfo.strSceneName);
		Resources.UnloadUnusedAssets ();
	}

	public void PopScene() {
		if (_lstSceneInfo.Count >= 2)
			_lstSceneInfo.RemoveAt (_lstSceneInfo.Count-1);
		
		SetTitlebarLeftButton ();
		_lstSceneInfo [_lstSceneInfo.Count - 1].strShowType = FROM_POP;
		SceneManager.LoadScene (_lstSceneInfo[_lstSceneInfo.Count-1].strSceneName);
		Resources.UnloadUnusedAssets ();
	}

	private void SetTitlebarLeftButton() {
		if (_lstSceneInfo.Count > 2) {
			TitlebarController.Instance.SetTitlebarLeftButton (TitlebarController.TITLEBAR_BACK_BUTTON);
		} else {
			TitlebarController.Instance.SetTitlebarLeftButton (TitlebarController.TITLEBAR_HOME_BUTTON);
		}
	}

	public DicSceneInfo GetCurrentSceneInfo() {
		return _lstSceneInfo [_lstSceneInfo.Count - 1];
	}

	public DicSceneInfo GetPrevSceneInfo() {
		return _lstSceneInfo [_lstSceneInfo.Count - 2];
	}

	public Dictionary<string, object> GetCurrentSceneSetting() {
		return _lstSceneInfo [_lstSceneInfo.Count - 1].GetSceneSetting ();
	}

	public string GetCurrentSceneShowType() {
		return _lstSceneInfo [_lstSceneInfo.Count - 1].strShowType;
	}

	public string GetStrCurrentSceneName() {
		return _lstSceneInfo [_lstSceneInfo.Count - 1].strSceneName;
	}
}
