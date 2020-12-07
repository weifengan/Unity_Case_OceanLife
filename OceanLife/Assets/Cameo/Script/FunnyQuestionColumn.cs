using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FunnyQuestionColumn : MonoBehaviour {

	public delegate void OnColumnClickCallback (int intQuestionId);
	private OnColumnClickCallback _onColumnClickCallback = delegate { };

	public Text _questionText;
	public Text _answerText;
	public GameObject _seperatorLine;
	public GameObject ButtonViewVideo;
	public GameObject ButtonViewTextAnswer;

	private int IntQuestionId;

	public void InitColumn(int _intQuestionId, string _strQuestion, string _strAnswer, bool _isVideo, OnColumnClickCallback onColumnClickCallback) {
		IntQuestionId = _intQuestionId;
		_questionText.text = _strQuestion;
		_answerText.text = _strAnswer;
		_onColumnClickCallback += onColumnClickCallback;

		_seperatorLine.SetActive (!_isVideo);
		_answerText.gameObject.SetActive (!_isVideo);
		ButtonViewVideo.SetActive (_isVideo);
		//ButtonViewTextAnswer.SetActive (!_isVideo);
	}

	public void ColumnClick() {
		_onColumnClickCallback (IntQuestionId);
	}

}
