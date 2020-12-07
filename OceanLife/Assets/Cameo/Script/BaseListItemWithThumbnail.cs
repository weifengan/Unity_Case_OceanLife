using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Cameo;
using LitJson;

public class BaseListItemWithThumbnail : MonoBehaviour {

	public delegate void OnColumnClickCallback (Dictionary<string, object> parameters);
	protected OnColumnClickCallback _onColumnClickCallback = delegate { };

	public Text ItemLabel;
	public Image ThumbnailImage;
	public AspectRatioFitter AspectRatioFitter;

	protected float _imgWidth;
	protected float _imgHeight;

	public void InitItem(Dictionary<string, object> parameters, OnColumnClickCallback onCreatureListItemClickCallbackIn, Sprite iconSprite = null) {
		SetLabel (parameters);
		SetThumbnail (iconSprite);
		SetSize ();
		_onColumnClickCallback += onCreatureListItemClickCallbackIn;
		SetParameter (parameters);
	}

	private void SetLabel(Dictionary<string, object> parameters) {
		if (ItemLabel == null)
			return;

		ItemLabel.text = (string) parameters["strLabel"];
	}

	private void SetThumbnail(Sprite spriteToDisplay) {
		if (spriteToDisplay == null)
			return;
		ThumbnailImage.overrideSprite = spriteToDisplay;
		_imgWidth = spriteToDisplay.bounds.size.x;
		_imgHeight = spriteToDisplay.bounds.size.y;
		AspectRatioFitter.aspectRatio = _imgWidth / _imgHeight;
	}

	virtual protected void SetSize() {
	}

	virtual protected void SetParameter (Dictionary<string, object> parameters) {
	}

	virtual public void ColumnClick() {
	}
}
