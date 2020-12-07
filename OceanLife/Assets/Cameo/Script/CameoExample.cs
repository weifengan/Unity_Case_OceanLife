using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


internal class CameoExample : MonoBehaviour {
	[SerializeField]
	private Text _titleText;  //20160618 added by roy
	[SerializeField]
	private Text _statusText;
	[SerializeField]
	private Button _bluetoothButton;

	private void Start() {

		//基本使用呼叫以下二行即可
		//Cameo.Sensor.Instance.Initialize (); //編譯正式版請用此
		Cameo.Sensor.Instance.Initialize(true); //參數加上 true 為測試用，每 5 秒會固定回傳一個位置，編譯正式版請把 true 拿掉
		Cameo.Sensor.CameoLocationChangedEvent += DisplayOnLocationChanged; //地點更新會呼叫此 call back function

		//20160630 roy added for display testing info
		if (_titleText != null) {
			_titleText.text = "CAMEO:Init";
		}

		//加入 手動啟動 藍芽按鈕
		if (_bluetoothButton != null) {
			_bluetoothButton.onClick.AddListener (delegate() {
				BluetoothState.EnableBluetooth ();
			});
		}

	}

	private void Update() {

		if (_statusText != null) {
			//顯示手機藍芽收取 beacon 訊號狀態 (intiBeaconStatusCode > 0 代表正常, 0: 正在掃描中, <0 代表有錯誤發生,  -2 代表手機不支援藍芽接收 beacons)
			_statusText.text = "(" + Cameo.Sensor.Instance.intiBeaconStatusCode.ToString () + ")  " + Cameo.Sensor.Instance.striBeaconStatus;
		} else {
			Debug.Log ("(" + Cameo.Sensor.Instance.intiBeaconStatusCode.ToString () + ")  " + Cameo.Sensor.Instance.striBeaconStatus);
		}
	}

	private void DisplayOnLocationChanged() {

		if (_titleText != null) {
			string strAreaIdList = "   strAreaIdList=";
			foreach (int i in Cameo.LocationManager.Instance.lstIntCurrentAreaIds) {
				strAreaIdList = strAreaIdList + i.ToString ()+ ", " ;
			}

			_titleText.text = strAreaIdList + "(" + Cameo.LocationManager.Instance.intCurrentAreaId.ToString() + ")" +  Cameo.LocationManager.Instance.strCurrentAreaName + "  " ;
		}

		//Get Location info by these two variables:
		Debug.Log(Cameo.LocationManager.Instance.intCurrentAreaId);
		Debug.Log(Cameo.LocationManager.Instance.strCurrentAreaName);

	}



}