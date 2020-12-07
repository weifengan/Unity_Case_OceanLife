using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using LitJson;

namespace Cameo
{

	public class Sensor : Singleton<Sensor>  
	{
		//Show beacon changing events
		public delegate void CameoBeaconRangeChanged();
		public static event CameoBeaconRangeChanged CameoBeaconRangeChangedEvent;

		//Show Location changing events
		public delegate void CameoLocationChanged();
		public static event CameoLocationChanged CameoLocationChangedEvent;

		//Beacond status
		public string striBeaconStatus = "";
		public int intiBeaconStatusCode = 0;

		//Roy: iBeacons Objects
		private BeaconType bt_Type;
		private BroadcastMode bm_Mode;

		// Receive
		public List<Beacon> mybeacons = new List<Beacon>();
		public string s_UUID = "ACE895D7-3E3A-4557-8F89-0366A35BCB83";
		private string s_Major= "0";
		private string s_Minor= "0";
		private string s_Region= "cameo";

		//Roy: Location manager
		public string strLocationStringAll = "";
		public string strLocationString = "";

		public bool isBeaconTestMode = false;
		private int intTestIndex = 0;

		void Start() {
			Debug.Log("[Sensor] start.");
		}

		public void Initialize(bool isBeaconTestModeIn = false)
		{
			//Init location manager here
			Debug.Log("[Sensor] Initialize");
			Cameo.LocationManager.Instance.Initialize ();

			if (isBeaconTestModeIn) 
			{
				//Test mode
				setBeaconTestMode();
			} 
			else 
			{
				//Real beacon mode
				if (mybeacons == null) {
					mybeacons = new List<Beacon> ();
				}

				initReceiver ();
			}
		}
			
		private void initReceiver()
		{
			//Start
			setBeaconPropertiesAtStart(); // please keep here!

			iBeaconReceiver.regions = new iBeaconRegion[]{new iBeaconRegion(s_Region, new Beacon(s_UUID, Convert.ToInt32(s_Major), Convert.ToInt32(s_Minor)))};		
			//call back function
			iBeaconReceiver.BeaconRangeChangedEvent += OnBeaconRangeChanged;

			BluetoothState.BluetoothStateChangedEvent += delegate(BluetoothLowEnergyState state) {
				switch (state) {
				case BluetoothLowEnergyState.TURNING_OFF:
					iBeaconReceiver.Stop();
					intiBeaconStatusCode = 0;
					break;
				case BluetoothLowEnergyState.TURNING_ON:
					intiBeaconStatusCode = 0;
					break;
				case BluetoothLowEnergyState.UNKNOWN:
					intiBeaconStatusCode = -1;
					break;
				case BluetoothLowEnergyState.RESETTING:
					striBeaconStatus = "Checking Device…";
					intiBeaconStatusCode = 0;
					break;
				case BluetoothLowEnergyState.UNAUTHORIZED:
					striBeaconStatus = "You don't have the permission to use beacons.";
					intiBeaconStatusCode = -1;
					break;
				case BluetoothLowEnergyState.UNSUPPORTED:
					striBeaconStatus = "Your device doesn't support beacons.";
					intiBeaconStatusCode = -2;
					break;
				case BluetoothLowEnergyState.POWERED_OFF:
					//_bluetoothButton.interactable = true;
					striBeaconStatus = "Enable Bluetooth";
					intiBeaconStatusCode = 1;
					break;
				case BluetoothLowEnergyState.POWERED_ON:
					//_bluetoothButton.interactable = false;
					iBeaconReceiver.Scan();
					striBeaconStatus = "Bluetooth already enabled";
					intiBeaconStatusCode = 2;
					break;
				default:
					striBeaconStatus = "Unknown Error";
					intiBeaconStatusCode = -3;
					break;
				}
				DebugLogger.Instance.LogWarnning ("[Sensor] iBeacon Status = (" + state.ToString() + ")  ");
				Debug.Log ("[Sensor] iBeacon Status = (" + intiBeaconStatusCode.ToString() + ")  " + striBeaconStatus);

			};

			try {
				if (BluetoothState.GetBluetoothLEStatus() != BluetoothLowEnergyState.UNSUPPORTED) {
					if (BluetoothState.GetBluetoothLEStatus() != BluetoothLowEnergyState.POWERED_ON) {
						BluetoothState.EnableBluetooth();
					} else {
						iBeaconReceiver.Scan();
					}
				}
			} catch (Exception e) {
				// Device Not Support Beacon
				DebugLogger.Instance.LogError ("[BeaconSensor] Initialize / Execption: " + e.ToString());
				Debug.Log ("[BeaconSensor] Initialize / Execption: " + e.ToString());
			}
		}

		private void setBeaconPropertiesAtStart() 
		{

			bm_Mode = BroadcastMode.receive;
			bt_Type = BeaconType.iBeacon;
			//s_UUID = "ACE895D7-3E3A-4557-8F89-0366A35BCB83";
			//s_Region = "cameo";
			//s_Major = "0";
			//s_Minor = "0";

			//bs_State = BroadcastState.inactive;

			Debug.Log("[Sensor] setBeaconPropertiesAtStart() finished");
		}

		public void StartReceiver()
		{
			//_lstModelBeacon = new List<ModelBeacon>();
			iBeaconReceiver.Scan();
			Debug.Log ("Listening for beacons");

			return;
		}

		private void OnBeaconRangeChanged(Beacon[] beacons) { 

			//20160628 roy added 
			mybeacons.Clear();

			foreach (Beacon b in beacons) {
				var index = mybeacons.IndexOf(b);
				if (index == -1) {
					mybeacons.Add(b);
				} else {
					mybeacons[index] = b;
				}
			}
			for (int i = mybeacons.Count - 1; i >= 0; --i) {
				if (mybeacons[i].lastSeen.AddSeconds(10) < DateTime.Now) {
					mybeacons.RemoveAt(i);
				}
			}

			UpdateLocationChangeByBeacons();

			DisplayOnBeaconFound();
		}

		//使用 window buffer 避免位置訊息震盪
		const int intWindowSize = 6;
		public List<int> lstLocationWindow = new List<int>();
		public List<int> lstLocationResult = new List<int>();
		private int intLastAreaId = -1;

		private void UpdateLocationChangeByBeacons()
		{
			//20160630 roy added
			List<ModelBeacon> lstModelbeaconTemp = new List<ModelBeacon> ();

			//Compute the new location
			foreach (Beacon b in Cameo.Sensor.Instance.mybeacons) {
				//20160630 roy added
				ModelBeacon modelbeaconTemp = new ModelBeacon ();

				if (b.type == BeaconType.iBeacon) {
					//20160630 roy added
					modelbeaconTemp.UUID = b.UUID.ToString ();
					modelbeaconTemp.Major = b.major.ToString ();
					modelbeaconTemp.Minor = b.minor.ToString ();
					modelbeaconTemp.intRssi = b.rssi;
					string strBeaconUUID = "ACE895D7-3E3A-4557-8F89-0366A35BCB83";
					if (modelbeaconTemp.UUID.ToLower () == strBeaconUUID.ToLower ()) {
						//20160901 ios may return rssi=0 error message, should be ignored
                        if(modelbeaconTemp.intRssi != 0)
                        {
                          lstModelbeaconTemp.Add (modelbeaconTemp);
                        }
					}
				
				}


			}

			DebugLogger.Instance.LogWarnning (JsonMapper.ToJson(lstLocationWindow));
			//更新地區資訊
			if (!isBeaconTestMode) {
				Cameo.LocationManager.Instance.getLocationString (lstModelbeaconTemp);
			}


			//區域ID 歷程紀錄
			if (lstLocationWindow.Count < intWindowSize) {
				lstLocationWindow.Add (Cameo.LocationManager.Instance.intCurrentAreaId);
			} else {
				lstLocationWindow.RemoveAt (0);
				lstLocationWindow.Add (Cameo.LocationManager.Instance.intCurrentAreaId);
			}

			string strDebug = "";
			foreach (int intTmp in lstLocationWindow) {
				strDebug = strDebug + intTmp.ToString() + ",";
			}
			//Debug.Log ("[Sensor] lstLocationWindow= " + strDebug);  //lstLocationWindow.ToString() 

			//選出出現最多的區域ID
			lstLocationResult.Clear ();
			var lstAreaidGrooups = lstLocationWindow.Where(x => x > 0).GroupBy(i => i);

			//出現最多的次數
			int intMyMaxValue = lstAreaidGrooups.Max (x => x.Count ());
			foreach(var itemAreaid in lstAreaidGrooups )
			{
				var intKey = itemAreaid.Key;
				var intCount = itemAreaid.Count();

				//Debug.Log ("intKey=" + intKey + ", intCount=" + intCount);

				if (intCount >= intMyMaxValue) {
					//符合出現最多的次數，把 key 記錄下來
					lstLocationResult.Add (intKey);
				}

			}

			Debug.Log ("[Sensot] lstLocationWindow= " + strDebug + "    Max=" + intMyMaxValue + "  intLastAreaId=" + intLastAreaId +  ",  lstLocationResult.Count ()=" + lstLocationResult.Count ());  //lstLocationWindow.ToString() 

			//判斷區域清單是否有改變
			if (lstLocationResult.Count () == 1) {
				//出現頻率最多的區域訊號
				if (intLastAreaId != lstLocationResult.First ()) {
					intLastAreaId = lstLocationResult.First ();

					Cameo.LocationManager.Instance.setLocationAreaId (lstLocationResult.First ());

					//這行指令在更新 location manager 內的資訊
					strLocationString = "(" + Cameo.LocationManager.Instance.intCurrentAreaId.ToString () + ")" + Cameo.LocationManager.Instance.strCurrentAreaName + "  ";

					Debug.Log ("[Sensot] !!!!!!!!!!!!!!!!!!! strLocationString=" + strLocationString);

					OnLocationChanged ();

				}

			} else if (lstLocationResult.Count () < intWindowSize) {
				//干擾區，出現多筆的狀況
				lstLocationResult.Sort ((x, y) => {
					return -x.CompareTo (y);
				});

			//判斷是否資料都一樣，只要有一筆不同，就更新清單
				bool isNotTheSame = false;
				foreach (int i in lstLocationResult) {
					if(!Cameo.LocationManager.Instance.lstIntCurrentAreaIds.Exists(x => x == i)){
						isNotTheSame = true;
					}
				}

				if (isNotTheSame) {
					Cameo.LocationManager.Instance.lstIntCurrentAreaIds.Clear();

					foreach (int i in lstLocationResult) {
						Cameo.LocationManager.Instance.lstIntCurrentAreaIds.Add (i);
					}

					//intCurrentAreaId 設為 0 代表之後只要切回一筆，那就
					intLastAreaId = 0;
					Cameo.LocationManager.Instance.intCurrentAreaId = lstLocationResult.First ();

					Cameo.LocationManager.Instance.resetGroupLocationAreaData ();
					//intLastAreaId = lstLocationResult.First ();
					OnLocationChanged ();
					Debug.Log ("[Sensot] 干擾區，出現多筆的狀況   lstLocationResult.Count ()=" + lstLocationResult.Count ());
				}
				Debug.Log ("[Sensot] 干擾區，出現多筆的狀況 但不須更新區域清單  lstLocationResult.Count ()=" + lstLocationResult.Count ());
			} else {
				//干擾區，每一筆都不同，採用最新一筆區域
				intLastAreaId = Cameo.LocationManager.Instance.intCurrentAreaId;
				OnLocationChanged ();
				Debug.Log ("[Sensot] 干擾區，每一筆都不同，採用最新一筆區域");
			}

		}

		private void DisplayOnBeaconFound() 
		{

			if (CameoBeaconRangeChangedEvent != null) 
			{
				CameoBeaconRangeChangedEvent ();

			}
		}


		private void OnLocationChanged() { 
			

			//有改變才呼叫 call back
			if (CameoLocationChangedEvent != null) 
			{
				CameoLocationChangedEvent ();
			}
		}

		public void setBeaconTestMode()
		{
			isBeaconTestMode = true;

			striBeaconStatus = "Beacon Test Mode!";

			Debug.Log ("[Sensor] setBeaconTestMode StartCoroutine");
			//StartCoroutine(WaitAndPrint(2.0F));

			InvokeRepeating ("WaitAndPrint", 5f, 1f);
		}

		void WaitAndPrint() {
			//yield return new WaitForSeconds(waitTime);
			//print("WaitAndPrint " + Time.time);

			intTestIndex = Cameo.LocationManager.Instance.setTestLocationAreaId (intTestIndex);
			Debug.Log ("[Sensor] WaitAndPrint intTestIndex=" + intTestIndex);
			//OnLocationChanged ();
			UpdateLocationChangeByBeacons();
		}

	}

}