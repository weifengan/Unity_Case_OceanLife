using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SeperatorLineAnimation : MonoBehaviour {

	public Animator _animator;
	private float _floatStartPlaySecond = 0;

	// Use this for initialization
	void Start () {
		SetPlayTime ();
	}

	void Destroy() {
		CancelInvoke ("PlayAnimation");
	}

	public void SetPlayTime() {
		_animator.SetBool ("isPlay", false);
		_floatStartPlaySecond = Random.Range (0.1f, 1.5f);
		Invoke ("PlayAnimation", _floatStartPlaySecond);
	}

	void PlayAnimation() {
		_animator.SetBool ("isPlay", true);
	}


}
