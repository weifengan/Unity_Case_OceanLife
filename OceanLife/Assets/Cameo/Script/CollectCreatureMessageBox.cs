using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Cameo;

public class CollectCreatureMessageBox : BaseMessageBox {

	public Animator _collectCreatureAnimator;
	public Text Content;
	public float FadeTime = 0.2f;
	public Vector2 MinSize = new Vector2 (0.5f, 0.5f);

	// Use this for initialization
	void Start () {
		transform.localScale = new Vector3 (MinSize.x, MinSize.y);	
	}

	public override void Open (MessagBoxEventCallback onOpenedCallback, MessagBoxEventCallback onClosedCallback, object[] values)
	{
		string strCreatureName = (string)values [0];
		Content.text = "你收集到\n" + "「" + strCreatureName + "」";

		RectTransform rectTran = GetComponent<RectTransform> ();
		RectTweener.Scale (rectTran, MinSize, Vector2.one, FadeTime);
		Invoke ("onOpend", FadeTime);
		base.Open (onOpenedCallback, onClosedCallback, values);
	}

	public override void onOpend ()
	{
		//_collectCreatureAnimator.SetBool ("isPlay", true);
	}

	public override void onClosed()
	{
		_onClosedCallback (this);
		EventChannel.Instance.Invoke ("OnCollectedMessageClosed");
	}

	public void AutoClose() {
		RectTransform rectTran = GetComponent<RectTransform> ();
		RectTweener.ScaleTo (rectTran, MinSize, FadeTime);
		Invoke ("onClosed", FadeTime);
	}
}
