using UnityEngine;
using System.Collections;
using Cameo;
using UnityEngine.UI;

public class VideoManagerSingleton : Singleton<VideoManagerSingleton>
{
    public MediaPlayerCtrl scrMedia;
    public GameObject videoView;
    public GameObject PlayImage;
	public GameObject PauseImage;
    public GameObject slider;
    public RawImage video;
    public GameObject warning;
//	public GameObject BtnPause;
//	public GameObject ImgPlay;
//	public GameObject ImgPause;
    private bool isPause;
    void Start()
    {
        scrMedia.OnEnd += OnEnd;
		PlayImage.SetActive(false);
        //scrMedia.OnReady += OnReady;
    }

    public void Play(string videoID)
    {
        int nStatus = CheckNetWork.ConnectionStatus();
        if (nStatus > 0)
        {
            Debug.Log("有連線狀態");
            videoView.SetActive(true);
            scrMedia.Load(videoID);
        }
        else
        {
            Debug.Log("無連線狀態");
            warning.SetActive(true);
            Invoke("CloseWarning", 3);
        }
    }
		

    private void CloseWarning()
    {
        warning.SetActive(false);
    }

    public void PauseSwitch()
    {
        if (isPause)
        {
            scrMedia.Play();
//			BtnPause.image.sprite = SptPause;
			PlayImage.SetActive(false);
			PauseImage.SetActive(true);
            isPause = false;
        }
        else
        {
            scrMedia.Pause();
//			BtnPause.image.sprite = SptPlay;
			PlayImage.SetActive(true);
			PauseImage.SetActive(false);
            isPause = true;
        }
    }

    public void OnEnd()
    {
        scrMedia.Stop();
        scrMedia.UnLoad();
        videoView.SetActive(false);
        video.texture = null;

    }

    public void SliderSwitch()
    {
        if (slider.activeInHierarchy == true)
        {
            slider.SetActive(false);
        }
        else
        {
            slider.SetActive(true);
        }

        slider.GetComponent<CanvasGroup>().alpha = 1;
        slider.GetComponent<SeekBarCtrl>().fadeTime = 0;

    }

    /*private void OnReady()
    {
        Debug.Log("XXXXXXXXXX");
        scrMedia.Play();
    }*/
}
