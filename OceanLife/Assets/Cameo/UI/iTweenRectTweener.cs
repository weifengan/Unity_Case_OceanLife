using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Cameo;

public class iTweenRectTweener {
	public static void FadeCanvasGroupAlpha (CanvasGroup _canvasGroup, float floatFadeTo, float floatFadeTime, string strOnCompleteFunc = "", GameObject onCompleteTarget = null) {
		float floatFadeFrom = _canvasGroup.alpha;

		Hashtable alphaHt = new Hashtable();
		alphaHt.Add ("from", floatFadeFrom);
		alphaHt.Add ("to", floatFadeTo);
		alphaHt.Add ("time",floatFadeTime);
		alphaHt.Add ("onupdate", (System.Action<object>) (floatAlpha => _canvasGroup.alpha = (float) floatAlpha));

		if (strOnCompleteFunc != "") {
			alphaHt.Add ("oncomplete", strOnCompleteFunc);
			alphaHt.Add ("oncompletetarget", onCompleteTarget);
		}

		iTween.ValueTo (_canvasGroup.gameObject, alphaHt);
	}

	public static void MoveRectTranformTo(RectTransform _rectTransform, Vector2 _moveTo, float _floatMoveTime, string strOnCompleteFunc = "", GameObject onCompleteTarget = null) {
		Hashtable htMove = new Hashtable ();
		htMove.Add ("from", _rectTransform.anchoredPosition);
		htMove.Add ("to", _moveTo);
		htMove.Add ("time", _floatMoveTime);
		htMove.Add ("easeType", iTween.EaseType.easeOutQuad);
		htMove.Add ("onupdate", (System.Action<object>) (newPos => _rectTransform.anchoredPosition = (Vector2) newPos));

		if (strOnCompleteFunc != "") {
			htMove.Add ("oncomplete", strOnCompleteFunc);
			htMove.Add ("oncompletetarget", onCompleteTarget);
		}

		iTween.ValueTo (_rectTransform.gameObject, htMove);

	}
}
