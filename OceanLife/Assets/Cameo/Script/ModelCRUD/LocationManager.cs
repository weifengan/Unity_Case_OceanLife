using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using LitJson;

namespace Cameo
{

	public class LocationManager : Singleton<LocationManager> 
	{
		[SerializeField]
		private List<ModelLocation> _lstModelLocation = new List<ModelLocation>();

		//private List<int> lstIntHistoryAreaId = new List<int>();

		[SerializeField]
		public List<int> lstIntCurrentAreaIds = new List<int>();
		public int intCurrentAreaId = 0;
		public string strCurrentAreaName = "";

		public void Initialize()
		{
			if(_lstModelLocation == null)
			{
				_lstModelLocation = new List<ModelLocation>();

			}

			resetLocation ();
			Init();
		}

		public void resetLocation()
		{
			_lstModelLocation.Clear ();
		}

		public void setLocationSingleBeaconTmp(int MajorIn, int MinorIn, int intBuildingIdIn, string strBuildingNameIn, int intAreaIdIn, string strAreaNameIn)
		{
			setLocationSingleBeacon(intBuildingIdIn,  strBuildingNameIn,  intAreaIdIn,  strAreaNameIn,  MajorIn.ToString(),  MinorIn.ToString());
		}

		private void setLocationSingleBeacon (int intBuildingIdIn, string strBuildingNameIn, int intAreaIdIn, string strAreaNameIn, string MajorIn, string MinorIn, int intRssiIn=-50 )
		{
			ModelLocation modelTemp = new ModelLocation ();
			modelTemp.intBuildingId = intBuildingIdIn;
			modelTemp.strBuildingName = strBuildingNameIn;
			modelTemp.intAreaId = intAreaIdIn;
			modelTemp.strAreaName = strAreaNameIn;
			//add beacon list
			List<ModelBeacon> lstModelbeaconTemp = new List<ModelBeacon>();
			ModelBeacon modelBeaconTemp = new ModelBeacon();
			modelBeaconTemp.UUID = "ACE895D7-3E3A-4557-8F89-0366A35BCB83";
			modelBeaconTemp.Major = MajorIn;
			modelBeaconTemp.Minor = MinorIn;
			modelBeaconTemp.intRssi = intRssiIn;
			lstModelbeaconTemp.Add (modelBeaconTemp);
			modelTemp.lstModelbeacon = lstModelbeaconTemp;
				
			_lstModelLocation.Add (modelTemp);
		}

		private void setLocationMultiBeacons (int intBuildingIdIn, string strBuildingNameIn, int intAreaIdIn, string strAreaNameIn, List<ModelBeacon> lstModelbeaconTempIn )
		{
			ModelLocation modelTemp = new ModelLocation ();
			modelTemp.intBuildingId = intBuildingIdIn;
			modelTemp.strBuildingName = strBuildingNameIn;
			modelTemp.intAreaId = intAreaIdIn;
			modelTemp.strAreaName = strAreaNameIn;

			//add beacon list
			List<ModelBeacon> lstModelbeaconTemp = new List<ModelBeacon>();

			foreach (ModelBeacon modelBeaconItem in lstModelbeaconTempIn) {

				ModelBeacon modelBeaconTemp = new ModelBeacon ();
				modelBeaconTemp.UUID = "ACE895D7-3E3A-4557-8F89-0366A35BCB83";
				modelBeaconTemp.Major = modelBeaconItem.Major;
				modelBeaconTemp.Minor = modelBeaconItem.Minor;
				modelBeaconTemp.intRssi = modelBeaconItem.intRssi;
				lstModelbeaconTemp.Add (modelBeaconTemp);
				modelTemp.lstModelbeacon = lstModelbeaconTemp;
			}

			_lstModelLocation.Add (modelTemp);
		}

		public void Init()
		{
			setBeaconAreaData ();

			setTestBeaconAreaData ();

			showDebugBeaconAreaData ();
		}

		public string getLocationString(List<ModelBeacon> lstModelbeaconIn)
		{
			//List<string> lstLocationString = new List<string> ();
			string strTempLocation = "";

			foreach (ModelLocation locationItem in _lstModelLocation) 
			{
				int intTmp = locationItem.setSimilarity (lstModelbeaconIn);

				strTempLocation = strTempLocation + intTmp.ToString () + ",";
			}

			List<ModelLocation> lstLocationMatched = new List<ModelLocation> ();



			foreach (ModelLocation locationItem in _lstModelLocation) 
			{
				if (locationItem.intSimilarity > 0) {
					lstLocationMatched.Add (locationItem);

                }
            }

			//選出可能的地點
			//Sort the Location list by the intSimilarity desc
			//var lstLocationMatched = LocationMatched.ToList();
			lstLocationMatched.Sort((x, y) => { return -x.intSimilarity.CompareTo(y.intSimilarity); });

			intCurrentAreaId = -1;
			lstIntCurrentAreaIds.Clear ();

			//Output result
			string strTempresult = "[" + lstLocationMatched.Count.ToString() + "]" ;
			foreach (ModelLocation modelItem in lstLocationMatched) 
			{
				//Set first area as current Location 
				if (intCurrentAreaId == -1) {
					intCurrentAreaId = modelItem.intAreaId;
					strCurrentAreaName = modelItem.strAreaName;
					lstIntCurrentAreaIds.Add (modelItem.intAreaId);
                    //下面這行為測試用
                  //  lstIntCurrentAreaIds.Add(3);
                    resetGroupLocationAreaData ();
				}

				string strTemp = modelItem.intAreaId.ToString() + modelItem.strAreaName + "(" + modelItem.intSimilarity.ToString() + ")  ";

				//lstLocationString.Add (strTemp);
				strTempresult = strTempresult + strTemp;

			}

			if (strTempresult == "")
				strTempresult = "Nothing";
			
			return strTempresult;
			//return lstLocationString;
		}

		//設定一筆 Area Id 為目前的展區地點
		public void setLocationAreaId(int intLocationAreaIdIn = 0)
		{
			string strTempLocation = "";

			ModelLocation modelItem = _lstModelLocation.Find (x => x.intAreaId == intLocationAreaIdIn);

			if (modelItem != null) {

				intCurrentAreaId = modelItem.intAreaId;
				strCurrentAreaName = modelItem.strAreaName;
				lstIntCurrentAreaIds.Clear ();
				lstIntCurrentAreaIds.Add (modelItem.intAreaId);
                //下面這行為測試用
                //lstIntCurrentAreaIds.Add(3);
                resetGroupLocationAreaData();
			}

		}

		//PC測試版使用，參數 intIndexTestIn 為流水號
		public int setTestLocationAreaId(int intIndexTestIn = 0)
		{
			string strTempLocation = "";

			ModelLocation modelItem = _lstModelLocation.ElementAt (intIndexTestIn);

			if (modelItem != null) {

				intCurrentAreaId = modelItem.intAreaId;
				strCurrentAreaName = modelItem.strAreaName;
				lstIntCurrentAreaIds.Clear ();
				lstIntCurrentAreaIds.Add (modelItem.intAreaId);
                //下面這行為測試用
                //lstIntCurrentAreaIds.Add(3);
                resetGroupLocationAreaData();
			}

			intIndexTestIn = (intIndexTestIn + 1) % _lstModelLocation.Count;

			return intIndexTestIn;
		}

		public void addBeacons(int intBuildingIdIn, string strBuildingNameIn, int intAreaIdIn, string strAreaNameIn, List<ModelBeacon> lstModelbeaconIn)
		{
			//區域內有多個 beacons
			setLocationMultiBeacons (intBuildingIdIn, strBuildingNameIn, intAreaIdIn, strAreaNameIn, lstModelbeaconIn);

		}

		public string saveJsonFile()
		{
			Debug.Log ("[LocationManager] saveJsonFile  START!!!!!!");
			List<ModelLocation> lstLocationMatched = new List<ModelLocation> ();

			string strJsonText = "";
			foreach (ModelLocation locationItem in _lstModelLocation) 
			{
				if (locationItem.lstModelbeacon.Count() > 1) {
					strJsonText += locationItem.intBuildingId.ToString() + "," + locationItem.intAreaId + ", \"" + locationItem.strAreaName + "\"";

					strJsonText += ", \"[";
					foreach (ModelBeacon beaconItem in  locationItem.lstModelbeacon) {
						strJsonText += beaconItem.Major + "," + beaconItem.Minor + "," + beaconItem.intRssi + "@";
					}
					strJsonText += "]\"";

					lstLocationMatched.Add (locationItem);
				}
			}

			//string strJsonText =  JsonMapper.ToJson(lstLocationMatched);
			Debug.Log (strJsonText);

			//PlayerPrefs.SetString("jsonMultiBeaconArea", strJsonText);

			return strJsonText;

			/*
			string destinationPath = Application.persistentDataPath + "/jsonMultiBeaconArea.json";
			Debug.Log  (destinationPath);
			FileInfo file = new FileInfo(destinationPath);
			StreamWriter streamWriter = file.CreateText();
			streamWriter.Write(strJsonText);
			streamWriter.Flush();
			streamWriter.Close();
			streamWriter.Dispose();
			*/

		}

		private void showDebugBeaconAreaData()
		{

			foreach (ModelLocation locationItem in _lstModelLocation) 
			{
//				Debug.Log (locationItem.strAreaName);

				foreach (ModelBeacon modelbeaconMy in locationItem.lstModelbeacon) {
//					Debug.Log (modelbeaconMy.Major + "   " + modelbeaconMy.Minor);
				}
			}
		}

		private void setTestBeaconAreaData()
		{
			//setLocationSingleBeacon (4, "Roy的家", 101, "左邊", "9999", "2");
			//setLocationSingleBeacon (4, "Roy的家", 102, "右邊", "9999", "3");
			setLocationSingleBeaconTmp(9999, 2, 1, "台灣水域館", 1, "從山到海 - 高山的溪流");
			setLocationSingleBeaconTmp(9999, 3, 2, "珊瑚王國館", 35, "珊瑚礁預覽 - 珊瑚礁頂");
//
//			//區域內有多個 beacons
//			List<ModelBeacon> lstModelbeaconTemp = new List<ModelBeacon>();
//			ModelBeacon modelBeaconTemp = new ModelBeacon ();
//			modelBeaconTemp.Major = "9999";
//			modelBeaconTemp.Minor = "2";
//			modelBeaconTemp.intRssi = -65;
//			lstModelbeaconTemp.Add (modelBeaconTemp);
//
//			modelBeaconTemp = new ModelBeacon ();
//			modelBeaconTemp.Major = "9999";
//			modelBeaconTemp.Minor = "3";
//			modelBeaconTemp.intRssi = -65;
//			lstModelbeaconTemp.Add (modelBeaconTemp);
//			//setLocationMultiBeacons (4, "Roy的家", 103, "中間", lstModelbeaconTemp);
//			setLocationMultiBeacons (3, "世界水域館", 72, "亞洲龍魚", lstModelbeaconTemp);
		}

		public bool resetGroupLocationAreaData()
		{
            //群組
            int[] lstIntAreaId1 = { 10, 11, 12, 13, 14, 15 };
            int[] lstIntAreaId2 = { 17, 18, 19, 20, 21, 22 };
            int[] lstIntAreaId3 = { 35, 36, 37 };
            int[] lstIntAreaId4 = { 31, 32 };

            if (Array.Exists(lstIntAreaId1, element => element == intCurrentAreaId))
            {
                setGroupLocationAreaIdToList(lstIntAreaId1, intCurrentAreaId);
                return true;
            }
            if (Array.Exists(lstIntAreaId2, element => element == intCurrentAreaId))
            {
                setGroupLocationAreaIdToList(lstIntAreaId2, intCurrentAreaId);
                return true;
            }
            if (Array.Exists(lstIntAreaId3, element => element == intCurrentAreaId))
            {
                setGroupLocationAreaIdToList(lstIntAreaId3, intCurrentAreaId);
                return true;
            }
            if (Array.Exists(lstIntAreaId4, element => element == intCurrentAreaId))
            {
                setGroupLocationAreaIdToList(lstIntAreaId4, intCurrentAreaId);
                return true;
            }

            return false;
        }

		private void setGroupLocationAreaIdToList(int[] lstIntAreaId, int intCurrentAreaIdIn)
		{
			lstIntCurrentAreaIds.Remove (intCurrentAreaIdIn);

			foreach(int intItem in lstIntAreaId)
			{
				if(lstIntCurrentAreaIds.Exists(x => x == intItem) == false)
				{
					lstIntCurrentAreaIds.Add (intItem);
                  //  lstIntCurrentAreaIds.Add(3);
                }
            }
		}

		private void setBeaconAreaData()
		{
            setLocationSingleBeaconTmp(1001, 1, 1, "海生館", 1, "從山到海 - 高山的溪流");
            setLocationSingleBeaconTmp(1001, 2, 1, "海生館", 1, "從山到海 - 高山的溪流");
            setLocationSingleBeaconTmp(1001, 3, 1, "海生館", 1, "從山到海 - 高山的溪流");
            setLocationSingleBeaconTmp(1001, 4, 1, "海生館", 1, "從山到海 - 高山的溪流");
            setLocationSingleBeaconTmp(1001, 5, 1, "海生館", 2, "從山到海 - 河流的中游");
            setLocationSingleBeaconTmp(1001, 6, 1, "海生館", 2, "從山到海 - 河流的中游");
            setLocationSingleBeaconTmp(1001, 7, 1, "海生館", 2, "從山到海 - 河流的中游");
            setLocationSingleBeaconTmp(1001, 8, 1, "海生館", 3, "從山到海 - 河邊溪緣");
            setLocationSingleBeaconTmp(1001, 9, 1, "海生館", 4, "從山到海 - 水庫");
            setLocationSingleBeaconTmp(1001, 10, 1, "海生館", 5, "從山到海 - 流水域魚類的適應");
            setLocationSingleBeaconTmp(1001, 43, 1, "海生館", 6, "從山到海 - 靜水域魚類的適應");
            setLocationSingleBeaconTmp(1001, 11, 1, "海生館", 7, "河口區 - 河口泥灘");
            setLocationSingleBeaconTmp(1001, 12, 1, "海生館", 8, "河口區 - 河口生態環境");
            setLocationSingleBeaconTmp(1001, 13, 1, "海生館", 8, "河口區 - 河口生態環境");//// update
            setLocationSingleBeaconTmp(1001, 15, 1, "海生館", 9, "河口區 - 牡蠣養殖");/////  update
            setLocationSingleBeaconTmp(1001, 14, 1, "海生館", 10, "河口區 - 河口的銀鱗鯧"); ////update
            setLocationSingleBeaconTmp(1001, 14, 1, "海生館", 11, "河口區 - 河口的四絲馬鮁"); // 文字有?
            setLocationSingleBeaconTmp(1001, 44, 1, "海生館", 12, "河口區 - 河口的黃金鰺");
            setLocationSingleBeaconTmp(1001, 45, 1, "海生館", 13, "河口區 - 河口的仙后水母");
            setLocationSingleBeaconTmp(1001, 46, 1, "海生館", 14, "河口區 - 河口的細尾雙邊魚");
            setLocationSingleBeaconTmp(1001, 47, 1, "海生館", 15, "河口區 - 河口的黑邊鰏"); // 文字有?
            setLocationSingleBeaconTmp(1001, 48, 1, "海生館", 16, "潮間帶 - 岩岸潮間帶");
            setLocationSingleBeaconTmp(1001, 16, 1, "海生館", 16, "潮間帶 - 岩岸潮間帶");
            setLocationSingleBeaconTmp(1001, 17, 1, "海生館", 16, "潮間帶 - 岩岸潮間帶");
            setLocationSingleBeaconTmp(1001, 18, 1, "海生館", 18, "潮間帶 - 海膽");
            setLocationSingleBeaconTmp(1001, 19, 1, "海生館", 19, "潮間帶 - 海星");
            setLocationSingleBeaconTmp(1001, 20, 1, "海生館", 20, "潮間帶 - 海參");
            setLocationSingleBeaconTmp(1001, 21, 1, "海生館", 21, "潮間帶 - 清潔蝦");
            setLocationSingleBeaconTmp(1001, 22, 1, "海生館", 22, "潮間帶 - 海葵");
            setLocationSingleBeaconTmp(1001, 26, 1, "海生館", 23, "珊瑚礁谷");
            setLocationSingleBeaconTmp(1001, 24, 1, "海生館", 23, "珊瑚礁谷");
            setLocationSingleBeaconTmp(1001, 25, 1, "海生館", 23, "珊瑚礁谷");
            setLocationSingleBeaconTmp(1001, 27, 1, "海生館", 24, "亞潮帶 - 軟珊瑚");
            setLocationSingleBeaconTmp(1001, 28, 1, "海生館", 25, "亞潮帶 - 狗鮫");
            setLocationSingleBeaconTmp(1001, 29, 1, "海生館", 26, "亞潮帶 - 章魚");
            setLocationSingleBeaconTmp(1001, 30, 1, "海生館", 27, "亞潮帶 - 龍蝦");
            setLocationSingleBeaconTmp(1001, 33, 1, "海生館", 28, "亞潮帶 - 南灣珊瑚礁");
            setLocationSingleBeaconTmp(1001, 32, 1, "海生館", 28, "亞潮帶 - 南灣珊瑚礁");
            setLocationSingleBeaconTmp(1001, 31, 1, "海生館", 28, "亞潮帶 - 南灣珊瑚礁");
            setLocationSingleBeaconTmp(1001, 34, 1, "海生館", 29, "亞潮帶 - 魚類的生存之道");
            setLocationSingleBeaconTmp(1001, 35, 1, "海生館", 30, "亞潮帶 - 群游");
            setLocationSingleBeaconTmp(1001, 36, 1, "海生館", 30, "亞潮帶 - 群游");
            setLocationSingleBeaconTmp(1001, 37, 1, "海生館", 31, "大洋區 - 大西洋海刺水母");
            setLocationSingleBeaconTmp(1001, 38, 1, "海生館", 32, "大洋區 - 珍珠水母");
            setLocationSingleBeaconTmp(1001, 39, 1, "海生館", 33, "大洋區 - 大洋池");
            setLocationSingleBeaconTmp(1001, 40, 1, "海生館", 33, "大洋區 - 大洋池");
            setLocationSingleBeaconTmp(1001, 41, 1, "海生館", 33, "大洋區 - 大洋池");
            setLocationSingleBeaconTmp(1003, 4, 1, "海生館", 33, "大洋區 - 大洋池");
            setLocationSingleBeaconTmp(1001, 42, 1, "海生館", 34, "保育水的世界");
            setLocationSingleBeaconTmp(1002, 1, 1, "海生館", 35, "珊瑚礁預覽 - 珊瑚礁頂");
            setLocationSingleBeaconTmp(1002, 2, 1, "海生館", 36, "珊瑚礁預覽 - 珊瑚礁緣");
            setLocationSingleBeaconTmp(1002, 3, 1, "海生館", 37, "珊瑚礁預覽 - 珊瑚礁壁");
            setLocationSingleBeaconTmp(1002, 4, 1, "海生館", 38, "珊瑚礁預覽 - 生物的城堡");
            setLocationSingleBeaconTmp(1002, 5, 1, "海生館", 38, "珊瑚礁預覽 - 生物的城堡");
            setLocationSingleBeaconTmp(1002, 6, 1, "海生館", 38, "珊瑚礁預覽 - 生物的城堡");
            setLocationSingleBeaconTmp(1002, 7, 1, "海生館", 39, "珊瑚礁之旅 - 礁頂的生物");
            setLocationSingleBeaconTmp(1002, 8, 1, "海生館", 40, "水下工作站");
            setLocationSingleBeaconTmp(1002, 9, 1, "海生館", 41, "珊瑚礁之旅 - 礁緣的生物");
            setLocationSingleBeaconTmp(1002, 10, 1, "海生館", 41, "珊瑚礁之旅 - 礁緣的生物");
            setLocationSingleBeaconTmp(1002, 11, 1, "海生館", 42, "珊瑚礁之旅 - 礁穴的鰻鯰");
            setLocationSingleBeaconTmp(1002, 12, 1, "海生館", 43, "珊瑚礁之旅 - 礁穴的糬鰻");
            setLocationSingleBeaconTmp(1002, 14, 1, "海生館", 44, "珊瑚礁之旅 - 礁壁的生物");
            setLocationSingleBeaconTmp(1002, 13, 1, "海生館", 44, "珊瑚礁之旅 - 礁壁的生物");
            setLocationSingleBeaconTmp(1002, 15, 1, "海生館", 44, "珊瑚礁之旅 - 礁壁的生物");
            setLocationSingleBeaconTmp(1002, 16, 1, "海生館", 45, "沉船探幽");
            setLocationSingleBeaconTmp(1002, 17, 1, "海生館", 46, "沉船探幽 - 附著生物");
            setLocationSingleBeaconTmp(1002, 18, 1, "海生館", 47, "沉船探幽 - 空間利用");
            setLocationSingleBeaconTmp(1002, 19, 1, "海生館", 47, "沉船探幽 - 空間利用");
            setLocationSingleBeaconTmp(1002, 20, 1, "海生館", 48, "沉船探幽 - 船艙");
            setLocationSingleBeaconTmp(1002, 21, 1, "海生館", 49, "沉船探幽 - 船長室");
            setLocationSingleBeaconTmp(1002, 22, 1, "海生館", 50, "沉船探幽 - 巡狩掠食");
            setLocationSingleBeaconTmp(1002, 23, 1, "海生館", 50, "沉船探幽 - 巡狩掠食");
            setLocationSingleBeaconTmp(1002, 24, 1, "海生館", 51, "沉船探幽 - 逃避敵害");
            setLocationSingleBeaconTmp(1002, 25, 1, "海生館", 52, "珊瑚礁之旅 - 生物的共生");
            setLocationSingleBeaconTmp(1002, 26, 1, "海生館", 53, "花園鰻");
            setLocationSingleBeaconTmp(1002, 27, 1, "海生館", 53, "花園鰻");
            setLocationSingleBeaconTmp(1002, 28, 1, "海生館", 54, "小白鯨");
            setLocationSingleBeaconTmp(1002, 29, 1, "海生館", 54, "小白鯨");
            setLocationSingleBeaconTmp(1002, 31, 1, "海生館", 54, "小白鯨");
            setLocationSingleBeaconTmp(1002, 30, 1, "海生館", 54, "小白鯨");
            setLocationSingleBeaconTmp(1003, 32, 1, "海生館", 55, "歡迎進入世界海洋館");
            setLocationSingleBeaconTmp(1003, 35, 1, "海生館", 55, "歡迎進入世界海洋館");
            setLocationSingleBeaconTmp(1003, 49, 1, "海生館", 55, "歡迎進入世界海洋館");
            setLocationSingleBeaconTmp(1003, 1, 1, "海生館", 56, "古代海洋");
            setLocationSingleBeaconTmp(1003, 2, 1, "海生館", 57, "古代海洋 - 海洋");
            setLocationSingleBeaconTmp(1003, 3, 1, "海生館", 58, "古代海洋 - 水母");
            setLocationSingleBeaconTmp(1003, 5, 1, "海生館", 59, "古代海洋 - 埃迪卡拉海");
            setLocationSingleBeaconTmp(1003, 47, 1, "海生館", 60, "古代海洋 - 澄江生物群");
            setLocationSingleBeaconTmp(1003, 6, 1, "海生館", 61, "古代海洋 - 寒武紀大滅絕");
            setLocationSingleBeaconTmp(1003, 7, 1, "海生館", 62, "古代海洋 - 魚類王朝");
            setLocationSingleBeaconTmp(1003, 48, 1, "海生館", 63, "古代海洋 - 二疊紀大滅絕");
            setLocationSingleBeaconTmp(1003, 8, 1, "海生館", 64, "古代海洋 - 海中霸主");
            setLocationSingleBeaconTmp(1003, 9, 1, "海生館", 65, "古代海洋 - 鯨類演化");
            setLocationSingleBeaconTmp(1003, 10, 1, "海生館", 66, "中國鱟");
            setLocationSingleBeaconTmp(1003, 11, 1, "海生館", 67, "卡式江魟");
            setLocationSingleBeaconTmp(1003, 12, 1, "海生館", 68, "裸臀魚");
            setLocationSingleBeaconTmp(1003, 18, 1, "海生館", 69, "鸚鵡螺");
            setLocationSingleBeaconTmp(1003, 13, 1, "海生館", 70, "斑紋異齒鮫");
            setLocationSingleBeaconTmp(1003, 14, 1, "海生館", 71, "拉氏多鰭魚");
            setLocationSingleBeaconTmp(1003, 15, 1, "海生館", 72, "象魚");
            setLocationSingleBeaconTmp(1003, 16, 1, "海生館", 73, "亞洲龍魚");
            setLocationSingleBeaconTmp(1003, 17, 1, "海生館", 74, "長吻火箭魚");
            setLocationSingleBeaconTmp(1003, 19, 1, "海生館", 75, "海藻森林 -  觀景窗");
            setLocationSingleBeaconTmp(1003, 20, 1, "海生館", 75, "海藻森林 -  觀景窗");
            setLocationSingleBeaconTmp(1003, 31, 1, "海生館", 76, "海藻森林 - 海豹");
            setLocationSingleBeaconTmp(1003, 30, 1, "海生館", 76, "海藻森林 - 海豹");
            setLocationSingleBeaconTmp(1003, 21, 1, "海生館", 77, "海藻森林 - 弧形坡道");
            setLocationSingleBeaconTmp(1003, 22, 1, "海生館", 77, "海藻森林 - 弧形坡道");
            setLocationSingleBeaconTmp(1003, 23, 1, "海生館", 77, "海藻森林 - 弧形坡道");
            setLocationSingleBeaconTmp(1003, 24, 1, "海生館", 77, "海藻森林 - 弧形坡道");
            setLocationSingleBeaconTmp(1003, 25, 1, "海生館", 77, "海藻森林 - 弧形坡道");
            setLocationSingleBeaconTmp(1003, 26, 1, "海生館", 77, "海藻森林 - 弧形坡道");
            setLocationSingleBeaconTmp(1003, 27, 1, "海生館", 78, "海藻森林 - 巨藻森林缸");
            setLocationSingleBeaconTmp(1003, 28, 1, "海生館", 78, "海藻森林 - 巨藻森林缸");
            setLocationSingleBeaconTmp(1003, 29, 1, "海生館", 78, "海藻森林 - 巨藻森林缸");
            setLocationSingleBeaconTmp(1003, 33, 1, "海生館", 79, "深海水域 - 深海熱泉");
            setLocationSingleBeaconTmp(1003, 34, 1, "海生館", 80, "深海水域 - 深海生物");
            setLocationSingleBeaconTmp(1003, 36, 1, "海生館", 81, "深海水域 - 發光生物");
            setLocationSingleBeaconTmp(1003, 37, 1, "海生館", 82, "深海水域 - 浮游生物");
            setLocationSingleBeaconTmp(1003, 38, 1, "海生館", 83, "極地水域 - 極光");
            setLocationSingleBeaconTmp(1003, 39, 1, "海生館", 84, "極地水域 - 北極");
            setLocationSingleBeaconTmp(1003, 40, 1, "海生館", 85, "極地水域 - 南極");
            setLocationSingleBeaconTmp(1003, 41, 1, "海生館", 86, "極地水域 - 企鵝");
            setLocationSingleBeaconTmp(1003, 42, 1, "海生館", 86, "極地水域 - 企鵝");
            setLocationSingleBeaconTmp(1003, 43, 1, "海生館", 87, "極地水域 - 海鸚鵡");
            setLocationSingleBeaconTmp(1003, 44, 1, "海生館", 88, "特展 - 鯊魚");
            setLocationSingleBeaconTmp(1003, 45, 1, "海生館", 88, "特展 - 鯊魚");

        }


	}

}