using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Cameo;
using LitJson;

public class AboutController : MonoBehaviour {

	public CanvasGroup ContentHolder;
	public Text AppName;
	public Text Version;

	// Use this for initialization
	void Start () {
		AppName.text = Application.productName;
		Version.text = "v " + Application.version;

		TitlebarController.Instance.SetTitleText ("說明");
		TitlebarController.Instance.SetBackButtonClickCallback (OnBackClick);
		TitlebarController.Instance.ShowTitlebar ();

		iTweenRectTweener.FadeCanvasGroupAlpha (ContentHolder, 1, 0.3f);
	}

	void OnDestroy() {
		TitlebarController.Instance.RemoveBackButtonClickCallback (OnBackClick);
	}

	public void OnBackClick() {
		FadeOutContent("BackToHome");
	}

	public void BackToHome() {
		SceneNavigator.Instance.PopScene ();
	}

	private void FadeOutContent(string strOnCompeleteFunction) {
		DicSceneInfo _dicPrevSceneInfo = SceneNavigator.Instance.GetPrevSceneInfo ();
		BackgroundController.Instance.FadeBackgroundTo (_dicPrevSceneInfo.strBgImageUrl, _dicPrevSceneInfo.BgColor, 0.3f);
		iTweenRectTweener.FadeCanvasGroupAlpha (ContentHolder, 0, 0.3f, strOnCompeleteFunction, this.gameObject);
	}
}
