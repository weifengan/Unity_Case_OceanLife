using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ModelLocation {

	//== Json data ============================================================
	public List<ModelBeacon> lstModelbeacon { get; set; } 
	public int intBuildingId { get; set; } 
	public string strBuildingName  { get; set; }
	public int  intAreaId  { get; set; }
	public string  strAreaName  { get; set; }
	public int  intSimilarity  { get; set; }  // value from 0 to 100


	public int setSimilarity(List<ModelBeacon> lstModelbeaconIn)
	{
		int intTempSimilarity = 0;
		intTempSimilarity = getSimilarity(lstModelbeaconIn);

		intSimilarity = intTempSimilarity;
		return intTempSimilarity;
	}

	public int getSimilarity(List<ModelBeacon> lstModelbeaconIn)
	{
		int intTempSimilarity = 0;
		foreach(ModelBeacon modelbeaconIn in lstModelbeaconIn)
		{
			intTempSimilarity = intTempSimilarity + getSimilaritySingle (modelbeaconIn);

		}

		return intTempSimilarity;
	}

	private int getSimilaritySingle(ModelBeacon modelBeaconIn)
	{
		int intTempSimilarity = 0;

		if (lstModelbeacon.Count == 0)
			return 0;


		foreach(ModelBeacon modelbeaconItem in lstModelbeacon)
		{
			//UUID need to lower
			if (modelBeaconIn.UUID.ToLower() == modelbeaconItem.UUID.ToLower() && modelBeaconIn.Major == modelbeaconItem.Major && modelBeaconIn.Minor == modelbeaconItem.Minor) 
			{
				//todo: 更精細的依照 RSSI value 來計算相似度
				if (lstModelbeacon.Count == 1) 
				{

					//intTempSimilarity = 140 + modelBeaconIn.intRssi;

					if (modelBeaconIn.intRssi >= -60) {
						//intTempSimilarity = 100;
						intTempSimilarity = 140 + modelBeaconIn.intRssi;
					}
					else if (modelBeaconIn.intRssi < -60 && modelBeaconIn.intRssi >= -70) {
						//intTempSimilarity = 80;
						intTempSimilarity = 130 + modelBeaconIn.intRssi;
					}
					else if (modelBeaconIn.intRssi < -70 && modelBeaconIn.intRssi >= -80) {
						//intTempSimilarity = 60;
						intTempSimilarity = 120 + modelBeaconIn.intRssi;
					}
					else if (modelBeaconIn.intRssi < -80 && modelBeaconIn.intRssi >= -90) {
						//intTempSimilarity = 50;
						intTempSimilarity = 120 + modelBeaconIn.intRssi;
					}
					else if (modelBeaconIn.intRssi <= -90) {
						//intTempSimilarity = 10;
						intTempSimilarity = 100 + modelBeaconIn.intRssi;

						if (intTempSimilarity < 0)
							intTempSimilarity = 0;
					}

				}

				// Multi beacons
				if (lstModelbeacon.Count > 1) 
				{
					int intUnit = 120;
					intUnit = 120 / lstModelbeacon.Count;

					if (Math.Abs( modelbeaconItem.intRssi -  modelBeaconIn.intRssi ) <= 5 ) 
					{
						intTempSimilarity = intTempSimilarity + intUnit;
					}
					else if (Math.Abs( modelbeaconItem.intRssi -  modelBeaconIn.intRssi ) <= 10 ) 
					{
						intTempSimilarity = intTempSimilarity + (intUnit - 5);
					}
					else if (Math.Abs( modelbeaconItem.intRssi -  modelBeaconIn.intRssi ) <= 15 ) 
					{
						intTempSimilarity = intTempSimilarity + (intUnit -10);
					}
				}

			}
		}

		return intTempSimilarity;
	}

}


