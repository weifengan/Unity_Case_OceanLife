using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Cameo;
using LitJson;

public class CreatureListItem2 : BaseListItemWithThumbnail {

	public GameObject ThumbnailAndTextHolder;
	public GameObject CollectedIcon;
	public GameObject TextHolder;
	private string StrCreatureId;

	private float _imgTargetHeight;

	override protected void SetSize() {
		ThumbnailImage.GetComponent<LayoutElement> ().preferredWidth = 305;
		_imgTargetHeight = ThumbnailImage.GetComponent<LayoutElement> ().preferredWidth / AspectRatioFitter.aspectRatio;
		ThumbnailAndTextHolder.GetComponent<LayoutElement> ().preferredHeight = _imgTargetHeight + TextHolder.GetComponent<LayoutElement> ().preferredHeight;
		this.GetComponent<LayoutElement> ().preferredHeight = _imgTargetHeight + TextHolder.GetComponent<LayoutElement> ().preferredHeight;
	}

	override protected void SetParameter (Dictionary<string, object> parameters) {
		StrCreatureId = (string) parameters["strCreatureId"];

		CheckIsCreatureCreated ();
	}

	private void CheckIsCreatureCreated() {
		if (CreatureDataManager.Instance.CheckIsCreatureCollected (StrCreatureId)) {
			CollectedIcon.SetActive (true);
		}
	}

	override public void ColumnClick() {
		Dictionary <string, object> _parameters = new Dictionary<string, object> ();
		_parameters.Add ("strCreautreId", StrCreatureId);
		_onColumnClickCallback (_parameters);
	}

	public float GetPreferredHeight() {
		return this.GetComponent<LayoutElement> ().preferredHeight;
	}
}
