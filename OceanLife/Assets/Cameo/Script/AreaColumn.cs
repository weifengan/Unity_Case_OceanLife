using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AreaColumn : MonoBehaviour {

	public delegate void OnAreaColumnClickCallback (int intAreaId);
	private OnAreaColumnClickCallback _onAreaColumnClickCallback = delegate { };
	public Text ColumnLabel;
	public int IntAreaId;

	public void InitAreaColumn(string _strLabel, int _intAreaId, OnAreaColumnClickCallback onAreaColumnClickCallback) {
		ColumnLabel.text = _strLabel;
		IntAreaId = _intAreaId;
		_onAreaColumnClickCallback += onAreaColumnClickCallback;
	}

	public void ColumnClick() {
		_onAreaColumnClickCallback (IntAreaId);
	}
		
}
