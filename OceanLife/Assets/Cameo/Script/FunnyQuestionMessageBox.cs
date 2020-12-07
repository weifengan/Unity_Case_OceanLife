using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Cameo;

public class FunnyQuestionMessageBox : BaseMessageBox {
	
	public Text Content;
	public float FadeTime = 0.2f;
	public GameObject BtnViewTourVideo;
	public GameObject BtnViewAnswer;
	public Vector2 MinSize = new Vector2 (0.5f, 0.5f);
	private DicFunnyQuestion _dicFunnyQuestion;
	private bool isVideo = false;

	// Use this for initialization
	void Start () {
		transform.localScale = new Vector3 (MinSize.x, MinSize.y);
	}

	public override void Open (MessagBoxEventCallback onOpenedCallback, MessagBoxEventCallback onClosedCallback, object[] values)
	{
		_dicFunnyQuestion = (DicFunnyQuestion)values [0];
		isVideo = (bool)values [1];
		Content.text = _dicFunnyQuestion.strQuestion;

		if (_dicFunnyQuestion.strVideoUrl != "") {
			Content.text += "\n我們有精彩的導覽影片，快來看看！";
			BtnViewTourVideo.SetActive (true);
			BtnViewAnswer.SetActive (false);
		} else {
			BtnViewTourVideo.SetActive (false);
			BtnViewAnswer.SetActive (true);
		}

		RectTransform rectTran = GetComponent<RectTransform> ();
		RectTweener.Scale (rectTran, MinSize, Vector2.one, FadeTime);
		Invoke ("onOpend", FadeTime);
		base.Open (onOpenedCallback, onClosedCallback, values);
	}

	public override void Close ()
	{
		RectTransform rectTran = GetComponent<RectTransform> ();
		RectTweener.ScaleTo (rectTran, MinSize, FadeTime);
		Invoke ("onClosed", FadeTime);
	}

	public void ViewAnswer() {
		//Debug.Log ("[FunnyQuestionMessageBox] ViewAnswer.");
		if (isVideo) {
			EventChannel.Instance.Invoke ("onViewTourVideoClick");
		} else {
			EventChannel.Instance.Invoke ("onViewAnswerClick");
			Debug.Log ("[FunnyQuestionMessageBox] ViewAnswer / strAnswer: " + _dicFunnyQuestion.strAnswer);
			MessageBoxManager.Instance.ShowMessageBox("FunnyQuestionAnswer", _dicFunnyQuestion.strAnswer);
		}

		Close ();
	}
}
