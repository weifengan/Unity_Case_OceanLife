using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CreatureTextInfoContent : MonoBehaviour {

	public void initText(string strText) {
		this.GetComponent<Text> ().text = strText;
	}
}
