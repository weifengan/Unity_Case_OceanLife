using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Cameo;
using LitJson;

public class AreaListController : MonoBehaviour
{
    public GameObject _areaListScrollRect;
    public RectTransform _contentHolder;
    public GameObject _areaColumnAlightLeft;
    public GameObject _areaColumnAlightRight;
    public GameObject _seperatorLineAlightLeft;
    public GameObject _seperatorLineAlightRight;

    private Dictionary<string, object> _sceneSetting;
    private List<DicAreaInfo> _lstDicAreaInfo;
    private int _intClickAreaId;

    // Use this for initialization
    void Start()
    {
        _sceneSetting = SceneNavigator.Instance.GetCurrentSceneSetting();
        DicBuildingInfo _dicBuildingInfo = AreaInfoManager.Instance.GetBuildingInfoByName((string)_sceneSetting["strBuildingName"]);

        TitlebarController.Instance.SetTitleText(_dicBuildingInfo.strBuildingName);
        TitlebarController.Instance.SetBackButtonClickCallback(OnBackClick);
        TitlebarController.Instance.ShowTitlebar();

        _lstDicAreaInfo = AreaInfoManager.Instance.GetLstAreaInfoByBuildingId(_dicBuildingInfo.intBuildingId);

        createAreaList();

        if (_sceneSetting.ContainsKey("floatContentY"))
        {
            _contentHolder.anchoredPosition = new Vector2(_contentHolder.anchoredPosition.x, (float)_sceneSetting["floatContentY"]);
        }

        iTweenRectTweener.FadeCanvasGroupAlpha(_areaListScrollRect.gameObject.GetComponent<CanvasGroup>(), 1.2f, 0.3f);
    }

    void OnDestroy()
    {
        TitlebarController.Instance.RemoveBackButtonClickCallback(OnBackClick);
    }

    void createAreaList()
    {
        for (int i = 0; i < _lstDicAreaInfo.Count; i++)
        {
			Debug.Log ("_lstDicAreaInfo= "+_lstDicAreaInfo.Count.ToString());
            GameObject areaColumn = null;
            GameObject seperatorLine = null;
            if (i % 2 == 0)
            {
                areaColumn = GameObject.Instantiate(_areaColumnAlightLeft);
                seperatorLine = GameObject.Instantiate(_seperatorLineAlightLeft);
            }
            else
            {
                areaColumn = GameObject.Instantiate(_areaColumnAlightRight);
                seperatorLine = GameObject.Instantiate(_seperatorLineAlightRight);
            }
            areaColumn.GetComponent<AreaColumn>().InitAreaColumn(_lstDicAreaInfo[i].strAreaName, _lstDicAreaInfo[i].intAreaId, OnAreaColumnClick);
            areaColumn.GetComponent<RectTransform>().SetParent(_contentHolder, false);
            seperatorLine.GetComponent<RectTransform>().SetParent(_contentHolder, false);
        }
    }

    private void OnAreaColumnClick(int intAreaId)
    {
        _intClickAreaId = intAreaId;
        FadeOutScrollRectAndBg("GotoAreaInfo");
    }

    public void GotoAreaInfo()
    {
        SaveCurrentScrollPosition();

        Dictionary<string, object> dicSceneSetting = new Dictionary<string, object>();
        dicSceneSetting.Add("strBuildingName", (string)_sceneSetting["strBuildingName"]);
        dicSceneSetting.Add("intBuildingId", (int)_sceneSetting["intBuildingId"]);
        dicSceneSetting.Add("intAreaId", _intClickAreaId);
        var dicNextSceneInfo = new DicSceneInfo("AreaInfo", dicSceneSetting);
        string strBuildingName = AreaInfoManager.Instance.GetStrBuildingNameByAreaId(_intClickAreaId);
        DicBuildingInfo _dicBuildingInfo = AreaInfoManager.Instance.GetBuildingInfoByName(strBuildingName);
		dicNextSceneInfo.strBgImageUrl = "img/areaBg_color" ;
        dicNextSceneInfo.BgColor = new Color32(99, 99, 99, 255);

        DicAreaInfo dicAreaInfo = AreaInfoManager.Instance.GetAreaInfoByAreaId(_intClickAreaId);
        GoogleAnalytics.Client.SendEventHit("展缸參觀次數統計", dicAreaInfo.strAreaName);

        SceneNavigator.Instance.PushScene(dicNextSceneInfo);
    }

    private void SaveCurrentScrollPosition()
    {
        float floatContentY = _contentHolder.anchoredPosition.y;

        Dictionary<string, object> _dicCurrentSceneSetting = SceneNavigator.Instance.GetCurrentSceneSetting();
        if (_dicCurrentSceneSetting.ContainsKey("floatContentY"))
        {
            _dicCurrentSceneSetting["floatContentY"] = floatContentY;
        }
        else
        {
            _dicCurrentSceneSetting.Add("floatContentY", floatContentY);
        }
    }

    public void OnBackClick()
    {
        FadeOutScrollRectAndBg("BackToHome");
    }

    public void BackToHome()
    {
        SceneNavigator.Instance.PopScene();
    }

    private void FadeOutScrollRectAndBg(string strOnCompeleteFunction)
    {
        string strNextSceneBgImgUrl = "";
        Color32 colorNextSceneColor = new Color32();
        if (strOnCompeleteFunction == "BackToHome")
        {
            DicSceneInfo _dicNextSceneInfo = SceneNavigator.Instance.GetPrevSceneInfo();
            strNextSceneBgImgUrl = _dicNextSceneInfo.strBgImageUrl;
            colorNextSceneColor = _dicNextSceneInfo.BgColor;
        }
        else
        {
            string strBuildingName = AreaInfoManager.Instance.GetStrBuildingNameByAreaId(_intClickAreaId);
            DicBuildingInfo _dicBuildingInfo = AreaInfoManager.Instance.GetBuildingInfoByName(strBuildingName);
            strNextSceneBgImgUrl = "img/areaBg_" + _dicBuildingInfo.strBuildingCode;
            colorNextSceneColor = new Color32(99, 99, 99, 255);
        }

        BackgroundController.Instance.FadeBackgroundTo(strNextSceneBgImgUrl, colorNextSceneColor, 0.3f);
        iTweenRectTweener.FadeCanvasGroupAlpha(_areaListScrollRect.gameObject.GetComponent<CanvasGroup>(), 0, 0.3f, strOnCompeleteFunction, this.gameObject);
    }
}
