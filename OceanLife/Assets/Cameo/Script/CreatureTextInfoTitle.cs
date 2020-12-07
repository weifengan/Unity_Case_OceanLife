using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Cameo;

public class CreatureTextInfoTitle : MonoBehaviour {

	public Text _textHolder;

	public void initText(string strText) {
		_textHolder.text = strText;
//		Debug.Log ("*****"+strText);
	}
}
