using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Cameo;
using LitJson;
using UnityEngine.SceneManagement;

namespace Cameo
{
    public class DicAreaInfo
    {
        public int intBuildingId;
        public string strBuildingName;
        public int intAreaId;
        public string strAreaName;
    }

    public class MainMenu : MonoBehaviour
    {

        private DicSceneInfo _mainMenuSceneInfo;

        public CanvasGroup BeaconButton;
        public CanvasGroup TaiwanWatersButton;
        public CanvasGroup CoralKingdomButton;
        public CanvasGroup WorldWatersButton;
        public CanvasGroup BlueOceanNotesButton;
		public CanvasGroup FishRecognitionButton;
        public CanvasGroup FunctionButton;
        public CanvasGroup BiometricButton;

        private List<CanvasGroup> _lstFadeInSequence = new List<CanvasGroup>();
        private int intCurrentFadeInIndex = 0;

        private Dictionary<string, object> dicNextSceneSetting = new Dictionary<string, object>();
        private string _nextSceneType = "areaList";

        // Use this for initialization
        void Start()
        {
			SceneNavigator.Instance.gameObject.SetActive (true);
			TitlebarController.Instance.gameObject.SetActive (true);
            TitlebarController.Instance.HideTitlebar();
            BackgroundController.Instance.StartLightAnimation();

            _lstFadeInSequence.Add(BeaconButton);
            _lstFadeInSequence.Add(TaiwanWatersButton);
            _lstFadeInSequence.Add(CoralKingdomButton);
            _lstFadeInSequence.Add(WorldWatersButton);
            _lstFadeInSequence.Add(BlueOceanNotesButton);
			_lstFadeInSequence.Add(FishRecognitionButton);
            _lstFadeInSequence.Add(FunctionButton);
            _lstFadeInSequence.Add(BiometricButton);

            if (SceneNavigator.Instance.GetCurrentSceneShowType() == SceneNavigator.FROM_PUSH)
            {
                _mainMenuSceneInfo = SceneNavigator.Instance.GetCurrentSceneInfo();
                BackgroundController.Instance.SetCurrentBackgroundImage(_mainMenuSceneInfo.strBgImageUrl, _mainMenuSceneInfo.BgColor);
                //FadeInButtonBySequence ();
                //			} else {
                //				FadeInAllButtonAtSameTime ();
            }
            NewMethod1();
            Cameo.Sensor.CameoLocationChangedEvent += OnLocationChange;
        }



        private void NewMethod1()
        {
            NewMethod();
        }

        private void NewMethod()
        {
            FadeInAllButtonAtSameTime();
        }

        void OnDestroy()
        {
            Cameo.Sensor.CameoLocationChangedEvent -= OnLocationChange;
            CancelInvoke("OnLocationChange");
        }

        private void FadeInButtonBySequence()
        {
            if (intCurrentFadeInIndex < 5)
            {
                iTween.Stop(_lstFadeInSequence[intCurrentFadeInIndex].gameObject);
                iTweenRectTweener.FadeCanvasGroupAlpha(_lstFadeInSequence[intCurrentFadeInIndex], 1.0f, 1.5f, "FadeInButtonBySequence", this.gameObject);
                intCurrentFadeInIndex++;
            }
        }

        private void FadeInAllButtonAtSameTime()
        {
            iTween.Stop(BeaconButton.gameObject);
            iTween.Stop(TaiwanWatersButton.gameObject);
            iTween.Stop(CoralKingdomButton.gameObject);
            iTween.Stop(WorldWatersButton.gameObject);
            iTween.Stop(FunctionButton.gameObject);
            iTween.Stop(BlueOceanNotesButton.gameObject);
            iTween.Stop(BiometricButton.gameObject);
            iTweenRectTweener.FadeCanvasGroupAlpha(BeaconButton, 1.0f, 1.5f);
            iTweenRectTweener.FadeCanvasGroupAlpha(TaiwanWatersButton, 1.0f, 1.5f);
            iTweenRectTweener.FadeCanvasGroupAlpha(CoralKingdomButton, 1.0f, 1.5f);
            iTweenRectTweener.FadeCanvasGroupAlpha(WorldWatersButton, 1.0f, 1.5f);
            iTweenRectTweener.FadeCanvasGroupAlpha(FunctionButton, 1.0f, 1.5f);
            iTweenRectTweener.FadeCanvasGroupAlpha(BlueOceanNotesButton, 1.0f, 1.5f);
            iTweenRectTweener.FadeCanvasGroupAlpha(BiometricButton, 1.0f, 1.5f);
        }

        private void FadeOutAllButtonAndBgImage()
        {
            //iTween.Stop ();
            iTween.Stop(BeaconButton.gameObject);
            iTween.Stop(TaiwanWatersButton.gameObject);
            iTween.Stop(CoralKingdomButton.gameObject);
            iTween.Stop(WorldWatersButton.gameObject);
            iTween.Stop(FunctionButton.gameObject);
            iTween.Stop(BlueOceanNotesButton.gameObject);
            iTween.Stop(BiometricButton.gameObject);

            iTweenRectTweener.FadeCanvasGroupAlpha(BeaconButton, 0, 1.0f);
            iTweenRectTweener.FadeCanvasGroupAlpha(TaiwanWatersButton, 0, 1.0f);
            iTweenRectTweener.FadeCanvasGroupAlpha(CoralKingdomButton, 0, 1.0f);
            iTweenRectTweener.FadeCanvasGroupAlpha(WorldWatersButton, 0, 1.0f);
            iTweenRectTweener.FadeCanvasGroupAlpha(FunctionButton, 0, 1.0f);
            iTweenRectTweener.FadeCanvasGroupAlpha(BlueOceanNotesButton, 0, 1.0f);
            iTweenRectTweener.FadeCanvasGroupAlpha(BiometricButton, 0, 1.0f);
			string strBgImageSpriteFilePath ;
			if (_nextSceneType == "Course") 
			{
				BackgroundController.Instance.FadeBackgroundTo("img/areaBg_color", new Color32(255, 255, 255, 255), 1.0f, "GotoNextScene", this.gameObject);
			}	
//            string strBgImageSpriteFilePath = "img/mainMenuBg";
            else if (_nextSceneType == "AreaList")
            {
                DicBuildingInfo _dicBuildingInfo = AreaInfoManager.Instance.GetBuildingInfoByName((string)dicNextSceneSetting["strBuildingName"]);
                strBgImageSpriteFilePath = "img/areaBg_" + _dicBuildingInfo.strBuildingCode;
				BackgroundController.Instance.FadeBackgroundTo(strBgImageSpriteFilePath, new Color32(99, 99, 99, 200), 1.0f, "GotoNextScene", this.gameObject);
            }
			else
			{
				strBgImageSpriteFilePath = "img/mainMenuBg";
				BackgroundController.Instance.FadeBackgroundTo(strBgImageSpriteFilePath, new Color32(99, 99, 99, 200), 1.0f, "GotoNextScene", this.gameObject);
			}	

//            BackgroundController.Instance.FadeBackgroundTo(strBgImageSpriteFilePath, new Color32(99, 99, 99, 200), 1.0f, "GotoNextScene", this.gameObject);
        }

        public void OnBtnBeaconClick()
        {
            TitlebarController.Instance.OnBeaconButtonClick();
        }

        public void GotoTaiwanWaters()
        {
            dicNextSceneSetting.Add("strBuildingName", "臺灣水域館");
            dicNextSceneSetting.Add("intBuildingId", AreaInfoManager.Instance.GetBuildingIdByName("臺灣水域館"));
            SetNextSceneBgImage();
        }

        public void GotoCoralKingdom()
        {
            dicNextSceneSetting.Add("strBuildingName", "珊瑚王國館");
            dicNextSceneSetting.Add("intBuildingId", AreaInfoManager.Instance.GetBuildingIdByName("珊瑚王國館"));
            SetNextSceneBgImage();
        }

        public void GotoWorldWaters()
        {
            dicNextSceneSetting.Add("strBuildingName", "世界水域館");
            dicNextSceneSetting.Add("intBuildingId", AreaInfoManager.Instance.GetBuildingIdByName("世界水域館"));
            SetNextSceneBgImage();
        }

        public void GotoBiometric()
        {
			var dicMainMenuSceneInfo = new DicSceneInfo ("BioTest", null, "img/areaBg_color");
			dicMainMenuSceneInfo.SetBgColor(new Color32 (69, 162, 180, 255));
			SceneNavigator.Instance.PushScene (dicMainMenuSceneInfo);
//			SceneManager.LoadScene ("BioTest");
        }

        public void GotoTestScene()
        {
            _nextSceneType = "Course";
            FadeOutAllButtonAndBgImage();
        }

        private void SetNextSceneBgImage()
        {
            GoogleAnalytics.Client.SendEventHit("展廳參觀次數統計", (string)dicNextSceneSetting["strBuildingName"]);
            _nextSceneType = "AreaList";
            FadeOutAllButtonAndBgImage();
        }

        public void GotoMyCollection()
        {
            _nextSceneType = "MyCollection";
            FadeOutAllButtonAndBgImage();
        }

        public void GotoFunnyQuestion()
        {
            _nextSceneType = "FunnyQuestionAreaList";
            FadeOutAllButtonAndBgImage();
        }

        public void GotoAbout()
        {
            _nextSceneType = "About";
            FadeOutAllButtonAndBgImage();
        }

        public void GotoNextScene()
        {
            var dicNextSceneInfo = new DicSceneInfo(_nextSceneType, dicNextSceneSetting);

            string strBgImageSpriteFilePath = "img/mainMenuBg";
            if (_nextSceneType == "AreaList")
            {
                DicBuildingInfo _dicBuildingInfo = AreaInfoManager.Instance.GetBuildingInfoByName((string)dicNextSceneSetting["strBuildingName"]);
                strBgImageSpriteFilePath = "img/areaBg_" + _dicBuildingInfo.strBuildingCode;
            }

            dicNextSceneInfo.strBgImageUrl = strBgImageSpriteFilePath;
            dicNextSceneInfo.SetBgColor(new Color32(99, 99, 99, 200));
            SceneNavigator.Instance.PushScene(dicNextSceneInfo);
        }

        public void OnLocationChange()
        {
            List<int> _lstAreaId = Cameo.LocationManager.Instance.lstIntCurrentAreaIds;
            if (_lstAreaId.Count != 0)
            {
                BeaconButton.gameObject.GetComponent<Animator>().SetBool("isPlay", true);
                Invoke("OnLocationChange", 3.0f);
            }
            else
            {
                BeaconButton.gameObject.GetComponent<Animator>().SetBool("isPlay", false);
            }
        }
    }
}