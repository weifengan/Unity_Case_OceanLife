using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using Cameo;

public enum SaveDataType {
	TEXT,
	IMAGE
}

public class SaveResult {
	public bool IsSuccess;
	public string Error = "";
	public Dictionary<string, object> Params = null;
}

public class SaveMission {
	public string StrFilePath;
	public string StrTextToSave;
	public byte[] BytesToSave;
	public SaveDataType DataType;
	public System.Action<SaveResult> CallBack;
	public Dictionary<string, object> Params;
}

public class SaveFileHelper : Singleton<SaveFileHelper> {
	
	private List<SaveMission> _lstQueue = new List<SaveMission> ();
	private int intCount = 0;
	private int intMax = 10;

	public void SaveTextData(string strFilePath, string strSaveData, System.Action<SaveResult> callBack, Dictionary<string, object> dicParams = null) {
		SaveMission saveMission = new SaveMission ();
		saveMission.StrFilePath = strFilePath;
		saveMission.DataType = SaveDataType.TEXT;
		saveMission.CallBack = callBack;
		saveMission.Params = dicParams;
		saveMission.StrTextToSave = strSaveData;

		_lstQueue.Add (saveMission);

		GoSave ();
	}

	public void SaveImageData(string strFilePath, byte[] bytesToSave, System.Action<SaveResult> callBack, Dictionary<string, object> dicParams = null) {
		SaveMission saveMission = new SaveMission ();
		saveMission.StrFilePath = strFilePath;
		saveMission.DataType = SaveDataType.IMAGE;
		saveMission.CallBack = callBack;
		saveMission.Params = dicParams;
		saveMission.BytesToSave = bytesToSave;

		_lstQueue.Add (saveMission);

		GoSave ();
	}

	private void GoSave() {
		if (_lstQueue.Count != 0) {
			if (intCount < intMax) {
				SaveMission saveMission = _lstQueue [0];
				switch (saveMission.DataType) 
				{
				case SaveDataType.TEXT:
					SaveTextFile (saveMission);
					break;
				case SaveDataType.IMAGE:
					SaveImageFile (saveMission);
					break;
				}
				_lstQueue.RemoveAt (0);
			}

			if (_lstQueue.Count !=0)
				Invoke ("GoSave", 0.05f);
		}
	}

	private void SaveTextFile(SaveMission saveMission) {
		intCount++;

		System.IO.File.WriteAllText (saveMission.StrFilePath, saveMission.StrTextToSave);
		intCount--;

		SaveResult saveResult = new SaveResult ();
		saveResult.IsSuccess = true;
		saveResult.Params = saveMission.Params;
		if (saveMission.CallBack != null)
			saveMission.CallBack (saveResult);

		saveMission = null;
	}

	private void SaveImageFile(SaveMission saveMission) {
		intCount++;

		System.IO.File.WriteAllBytes (saveMission.StrFilePath, saveMission.BytesToSave);
		intCount--;

		SaveResult saveResult = new SaveResult ();
		saveResult.IsSuccess = true;
		saveResult.Params = saveMission.Params;
		saveMission.CallBack (saveResult);

		saveMission = null;
	}

}
