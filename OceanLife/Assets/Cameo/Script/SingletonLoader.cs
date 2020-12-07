using UnityEngine;
using System.Collections;

public class SingletonLoader : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameObject.DontDestroyOnLoad (gameObject);
	}
}
