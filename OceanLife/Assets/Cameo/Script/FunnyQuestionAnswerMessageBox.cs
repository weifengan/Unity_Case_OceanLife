using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Cameo;

public class FunnyQuestionAnswerMessageBox : BaseMessageBox {

	public Text Content;
	public float FadeTime = 0.2f;
	public Vector2 MinSize = new Vector2 (0.5f, 0.5f);

	// Use this for initialization
	void Start () {
		transform.localScale = new Vector3 (MinSize.x, MinSize.y);
	}

	public override void Open (MessagBoxEventCallback onOpenedCallback, MessagBoxEventCallback onClosedCallback, object[] values)
	{
		Debug.Log ("[FunnyQuestionAnswerMessageBox] Open / strAnswer: " + (string)values [0]);
		Content.text = (string)values [0];

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
}
