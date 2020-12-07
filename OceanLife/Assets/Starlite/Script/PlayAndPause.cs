using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Cameo;


public class PlayAndPause : Singleton<VideoManagerSingleton> {
	public MediaPlayerCtrl scrMedia;
	public Button BtnPause;
	public GameObject ImgPlay;
	public GameObject ImgPause;
	private bool isPause;
	// Use this for initialization
	void Start () {
		ImgPlay.SetActive(false);
		ImgPause.SetActive(true);
	}

	public void PauseSwitch()
	{
		if (isPause)
		{
			scrMedia.Play();
			ImgPlay.SetActive(false);
			ImgPause.SetActive(true);
			isPause = false;
		}
		else
		{
			scrMedia.Pause();
			ImgPlay.SetActive(true);
			ImgPause.SetActive(false);
			isPause = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
