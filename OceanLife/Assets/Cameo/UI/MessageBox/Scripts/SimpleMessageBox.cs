using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Cameo
{
	public class SimpleMessageBox : BaseMessageBox 
	{
		public Text Content;
		public float FadeTime = 0.2f;
		public Vector2 MinSize = new Vector2 (0.5f, 0.5f);

		void Start()
		{
			transform.localScale = new Vector3 (MinSize.x, MinSize.y);
		}

		public override void Open (MessagBoxEventCallback onOpenedCallback, MessagBoxEventCallback onClosedCallback, object[] values)
		{
			Content.text = values [0] as string;
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
}