using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cameo;
using LitJson;

public class TempDataHolder : Singleton<TempDataHolder> {
	
	private Dictionary<string, object> DicTempDate = new Dictionary<string, object>();

	public void SaveTempData(string strKey, object value) {
		DicTempDate[strKey] = value;
	}

	public object GetTempData(string strKey) {
		if (DicTempDate.ContainsKey (strKey)) {
			return DicTempDate [strKey];
		} else {
			return null;
		}
	}
}
