using UnityEngine;
using System.Collections;
using UnityEngine.UI;
[ExecuteInEditMode]
public class TestScript : MonoBehaviour {
	public RectTransform text;
	public LayoutElement box;
	public RectTransform boxsize;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		StartCoroutine (UpdataSize ());
		//LayoutRebuilder.ForceRebuildLayoutImmediate (boxsize);
	}

	IEnumerator UpdataSize(){
		yield return new WaitForEndOfFrame ();
		box.preferredHeight = text.sizeDelta.y;
		Debug.Log(text.sizeDelta);
		yield return new WaitForEndOfFrame ();
	}
}
