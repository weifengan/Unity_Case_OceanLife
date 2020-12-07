using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FunnyQuestionAreaTitle : MonoBehaviour {

	public delegate void OnColumnClickCallback (int IntAreaId);
	private OnColumnClickCallback _onColumnClickCallback = delegate { };

	public Text _areaTitle;
	public Text _questionNumber;
	private int IntAreaId;

	public void initColumn(int _intAreaId, string strTitleText, string strQuestionNumber, OnColumnClickCallback onColumnClickCallback) {
		IntAreaId = _intAreaId;
		_areaTitle.text = strTitleText;
		_questionNumber.text = strQuestionNumber;
		_onColumnClickCallback += onColumnClickCallback;
	}

	public void ColumnClick() {
		_onColumnClickCallback (IntAreaId);
	}
}
