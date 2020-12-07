using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Cameo;

public class BeaconAreaMessageBox : BaseMessageBox {

	public float FadeTime = 0.2f;
	public Vector2 MinSize = new Vector2 (0.5f, 0.5f);
	public GameObject AreaInfoColumn;
	public GameObject NoNearArea;
	public RectTransform _contentHolder;
	private List<int> _lstAreaId;

	// Use this for initialization
	void Start () {
		transform.localScale = new Vector3 (MinSize.x, MinSize.y);
	}

	public override void Open (MessagBoxEventCallback onOpenedCallback, MessagBoxEventCallback onClosedCallback, object[] values)
	{
		_lstAreaId = (List<int>)values [0];
		if (_lstAreaId.Count == 0) {
			NoNearArea.SetActive (true);
		} else {
			for (int i = 0; i < _lstAreaId.Count; i++) {
				GameObject areaColumn = GameObject.Instantiate (AreaInfoColumn);
				DicAreaInfo dicAreaInfo = AreaInfoManager.Instance.GetAreaInfoByAreaId (_lstAreaId [i]);
				areaColumn.GetComponent<AreaColumn> ().InitAreaColumn (dicAreaInfo.strAreaName, dicAreaInfo.intAreaId, OnAreaColumnClick);
				areaColumn.GetComponent<RectTransform> ().SetParent (_contentHolder, false);
			}
		}

		RectTransform rectTran = GetComponent<RectTransform> ();
		RectTweener.Scale (rectTran, MinSize, Vector2.one, FadeTime);
		Invoke ("onOpend", FadeTime);
		EventChannel.Instance.AttachListener ("CloseBeaconAreaMessageBox", Close);
		base.Open (onOpenedCallback, onClosedCallback, values);
	}

	private void OnAreaColumnClick(int intAreaId) {
		Dictionary<string, object> dicSceneSetting = new Dictionary<string, object> ();
		string strBuildingName = AreaInfoManager.Instance.GetStrBuildingNameByAreaId (intAreaId);
		int intBuildingId = AreaInfoManager.Instance.GetBuildingIdByName (strBuildingName);
		dicSceneSetting.Add ("strBuildingName", (string) strBuildingName);
		dicSceneSetting.Add ("intBuildingId", intBuildingId);
		dicSceneSetting.Add ("intAreaId", intAreaId);
		var dicNextSceneInfo = new DicSceneInfo ("AreaInfo", dicSceneSetting);
		DicBuildingInfo _dicBuildingInfo = AreaInfoManager.Instance.GetBuildingInfoByName (strBuildingName);
		dicNextSceneInfo.strBgImageUrl = "img/areaBg_" + _dicBuildingInfo.strBuildingCode;
		dicNextSceneInfo.BgColor = new Color32 (99, 99, 99, 255);

		DicAreaInfo dicAreaInfo = AreaInfoManager.Instance.GetAreaInfoByAreaId (intAreaId);
		GoogleAnalytics.Client.SendEventHit("展缸參觀次數統計", dicAreaInfo.strAreaName);

		BackgroundController.Instance.FadeBackgroundTo (dicNextSceneInfo.strBgImageUrl, dicNextSceneInfo.BgColor, 0.3f);
		SceneNavigator.Instance.PushScene (dicNextSceneInfo);
		Close ();
	}

	public override void Close ()
	{
		EventChannel.Instance.DetachListner ("CloseBeaconAreaMessageBox", Close);
		RectTransform rectTran = GetComponent<RectTransform> ();
		RectTweener.ScaleTo (rectTran, MinSize, FadeTime);
		Invoke ("onClosed", FadeTime);
		EventChannel.Instance.Invoke ("BeaconAreaMessageBoxClose");
	}
}
