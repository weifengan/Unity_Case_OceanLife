using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Cameo;
using LitJson;

public class CreatureInfoMediaItem : BaseListItemWithThumbnail {

	public Image PlayVideoIcon;
	private int IntMediaIndex;
	private string StrDataType;

	override protected void SetParameter (Dictionary<string, object> parameters) {
		IntMediaIndex = (int) parameters["intMediaIndex"];
		StrDataType = (string) parameters["strDataType"];

		if (StrDataType == "video")
			PlayVideoIcon.gameObject.SetActive (true);
	}

	override public void ColumnClick() {
		Dictionary <string, object> _parameters = new Dictionary<string, object> ();
		_parameters.Add ("intMediaIndex", IntMediaIndex);
		_parameters.Add ("strDataType", StrDataType);
		_onColumnClickCallback (_parameters);
	}

}
