using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Cameo;
using LitJson;

public class BackgroundController : Singleton<BackgroundController> {

	public Animator LightAnimator;
	public GameObject bg1Holder;
	public GameObject bg2Holder;
	private string strStandbyImageHolder = "bg2Holder";

	// Use this for initialization
	void Start () {
		GameObject.DontDestroyOnLoad (gameObject);

		bg2Holder.GetComponent<CanvasGroup> ().alpha = 0;
		bg1Holder.transform.SetSiblingIndex (0);
	}

	public void StartLightAnimation() {
		LightAnimator.SetBool ("isPlay", true);
	}

	public void SetCurrentBackgroundImage(string _strImageUrl, Color32 _imageCoverColor) {
		GameObject _currentImageHolder;

		if (strStandbyImageHolder == "bg1Holder")
			_currentImageHolder = bg2Holder;
		else
			_currentImageHolder = bg1Holder;
		
		_currentImageHolder.GetComponent<Image>().sprite = ResourceManager.Instance.GetAsset (_strImageUrl, typeof(Sprite)) as Sprite;
		_currentImageHolder.GetComponent<Image>().color = _imageCoverColor;
	}

	public void FadeBackgroundTo(string _strImageUrl, Color32 _imageCoverColor, float floatFadeTime, string strOnCompleteFunc = "", GameObject onCompleteTarget = null) {
		GameObject _currentImageHolder;
		GameObject _standbyImageHolder;

		if (strStandbyImageHolder == "bg1Holder") {
			_currentImageHolder = bg2Holder;
			_standbyImageHolder = bg1Holder;
			strStandbyImageHolder = "bg2Holder";
		} else {
			_currentImageHolder = bg1Holder;
			_standbyImageHolder = bg2Holder;
			strStandbyImageHolder = "bg1Holder";
		}

		_currentImageHolder.transform.SetSiblingIndex (1);

		_standbyImageHolder.GetComponent<Image>().sprite = ResourceManager.Instance.GetAsset (_strImageUrl, typeof(Sprite)) as Sprite;
		_standbyImageHolder.GetComponent<Image>().color = _imageCoverColor;
		_standbyImageHolder.GetComponent<CanvasGroup> ().alpha = 1;

		iTween.Stop (_currentImageHolder.GetComponent<CanvasGroup>().gameObject);
		iTween.Stop (_standbyImageHolder.GetComponent<CanvasGroup>().gameObject);
		iTweenRectTweener.FadeCanvasGroupAlpha (_currentImageHolder.GetComponent<CanvasGroup>(), 0, 1.0f, strOnCompleteFunc, onCompleteTarget);
	}
}
