using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using Cameo;

public class Preloader : MonoBehaviour {

	public Animator SplashScreenController;

	// Use this for initialization
	void Start () {
		GoogleAnalytics.StartTracking();
		GoogleAnalytics.Client.SendScreenHit("開啟 APP");

		if (Config.Instance.isDeleteUserProgress)
			UserProgress.DeleteUserProgress ();

		AreaInfoManager.Instance.initData (CheckIsDataIsReady);
		FunnyQuestionManager.Instance.initData (CheckIsDataIsReady);
		CreatureDataManager.Instance.initData (CheckIsDataIsReady);
		BeaconController.Instance.init (CheckIsDataIsReady);

		//Invoke ("HideSplashScreen", 3.5f);
	}

	void CheckIsDataIsReady() {
		if (AreaInfoManager.Instance.IsDataReady () && FunnyQuestionManager.Instance.IsDataReady() && CreatureDataManager.Instance.IsDataReady() && BeaconController.Instance.IsDataReady()) {
			HideSplashScreen ();
		}
	}

	void HideSplashScreen() {
		SplashScreenController.SetBool ("isHideSplashScreen", true);
	}

	public void GotoMainMenu() {
		var dicMainMenuSceneInfo = new DicSceneInfo ("Main", null, "img/mainMenuBg");
		dicMainMenuSceneInfo.SetBgColor(new Color32 (69, 162, 180, 255));
		SceneNavigator.Instance.PushScene (dicMainMenuSceneInfo);
	}


}
