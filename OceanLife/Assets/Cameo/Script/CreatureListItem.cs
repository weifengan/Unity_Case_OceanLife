using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Cameo;
using LitJson;

public class CreatureListItem : BaseListItemWithThumbnail {

	public GameObject CollectedIcon;
	private string StrCreatureId;

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
}
