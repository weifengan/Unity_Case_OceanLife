using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cameo;
using LitJson;

public class DicFunnyQuestion {
	public int intQuestionId;
	public int intBuildingId;
	public int intAreaId;
	public string strVideoUrl;
	public string strQuestion;
	public string strAnswer;
	public string strVideoUrl01;
	public string strVideoUrl02;
	public string strVideoUrl03;
	public string strVideoUrl04;
}

public class FunnyQuestionManager : Singleton<FunnyQuestionManager> {
	private string strJsonLstDicFunnyQuestionUrl = "http://" + Config.Instance.strServerIp + "/JsonData/jsonLstDicFunnyQuestion.json";
	private System.Action dataReadyCallback;
	private bool isDataReady = false;

	public void initData(System.Action dataReadyCallBackIn) {
		dataReadyCallback = dataReadyCallBackIn;

		List<int> lstReadFunnyQuestionId = UserProgress.LoadLstReadFunnyQuestionId ();
		TempDataHolder.Instance.SaveTempData ("lstReadFunnyQuestionId", lstReadFunnyQuestionId);

		DataCenter.Instance.GetJson (strJsonLstDicFunnyQuestionUrl, OnLoadLstDicFunnyQuestionReturn);
	}

	void OnLoadLstDicFunnyQuestionReturn(DataResult dataResult) {
		if (!dataResult.IsSuccess)
			return;

		var lstDicFunnyQuestionData = JsonMapper.ToObject<List<DicFunnyQuestion>> ((string) dataResult.Result);
		TempDataHolder.Instance.SaveTempData ("lstDicFunnyQuestion", lstDicFunnyQuestionData);

		isDataReady = true;
		if (dataReadyCallback != null) {
			dataReadyCallback ();
		}
	}

	public bool IsDataReady() {
		return isDataReady;
	}

	public List<DicFunnyQuestion> GetLstFunnyQuestionByAreaAid(int intAreaId) {
		var _lstAllFunnyQuestion = (List<DicFunnyQuestion>) TempDataHolder.Instance.GetTempData ("lstDicFunnyQuestion");
		var _lstFunnyQuestionReturn = new List<DicFunnyQuestion> ();

		for (int i = 0; i < _lstAllFunnyQuestion.Count; i++) {
			if (_lstAllFunnyQuestion [i].intAreaId == intAreaId) {
				_lstFunnyQuestionReturn.Add (_lstAllFunnyQuestion [i]);
			}
		}

		return _lstFunnyQuestionReturn;
	}

	public int GetIntFunnyQuestionNumberByAreaAid(int intAreaId) {
		return GetLstFunnyQuestionByAreaAid (intAreaId).Count;
	}

	public List<DicFunnyQuestion> GetLstUnreadFunnyQuestionByAreaId(int intAreaId) {
		var _lstAllFunnyQuestion = (List<DicFunnyQuestion>) TempDataHolder.Instance.GetTempData ("lstDicFunnyQuestion");
		var _lstFunnyQuestionReturn = new List<DicFunnyQuestion> ();

		for (int i = 0; i < _lstAllFunnyQuestion.Count; i++) {
			if (_lstAllFunnyQuestion [i].intAreaId == intAreaId && _lstAllFunnyQuestion [i].strQuestion != null && _lstAllFunnyQuestion [i].strQuestion != "" && !CheckIsFunnyQuestionRead(_lstAllFunnyQuestion [i].intQuestionId)) {
				_lstFunnyQuestionReturn.Add (_lstAllFunnyQuestion [i]);
			}
		}

		return _lstFunnyQuestionReturn;
	}

	public DicFunnyQuestion GetDicFunnyQuestionByQuestionId(int intQuestionId) {
		var _lstAllFunnyQuestion = (List<DicFunnyQuestion>) TempDataHolder.Instance.GetTempData ("lstDicFunnyQuestion");

		for (int i = 0; i < _lstAllFunnyQuestion.Count; i++) {
			if (_lstAllFunnyQuestion [i].intQuestionId == intQuestionId) {
				return _lstAllFunnyQuestion [i];
			}
		}

		return null;
	}

	public bool CheckIsFunnyQuestionRead(int intFunnyQuestionId) {
		bool isRead = false;
		List<int> lstReadFunnyQuestionId = (List<int>) TempDataHolder.Instance.GetTempData ("lstReadFunnyQuestionId");

		if (lstReadFunnyQuestionId.Contains(intFunnyQuestionId)) {
			isRead = true;
		}

		return isRead;
	}

	public void SetFunnyQuestionIsRead(int intFunnyQuestionId) {
		List<int> lstReadFunnyQuestionId = (List<int>) TempDataHolder.Instance.GetTempData ("lstReadFunnyQuestionId");
		if (lstReadFunnyQuestionId.Contains (intFunnyQuestionId))
			return;
		
		lstReadFunnyQuestionId.Add (intFunnyQuestionId);
		UserProgress.SaveLstReadFunnyQuestionId (lstReadFunnyQuestionId);
		TempDataHolder.Instance.SaveTempData ("lstReadFunnyQuestionId", lstReadFunnyQuestionId);
	}

	public DicFunnyQuestion GetAreaTourVideoQuestion(int intAreaId) {
		var _lstAllFunnyQuestion = (List<DicFunnyQuestion>) TempDataHolder.Instance.GetTempData ("lstDicFunnyQuestion");
		DicFunnyQuestion dicTourVideoQuestion = null;

		for (int i = 0; i < _lstAllFunnyQuestion.Count; i++) {
			if (_lstAllFunnyQuestion [i].intAreaId == intAreaId && _lstAllFunnyQuestion [i].strVideoUrl != "" ) {
				dicTourVideoQuestion = _lstAllFunnyQuestion [i];
				break;
			}
			//有第二支影片
//			if (_lstAllFunnyQuestion [i].intAreaId == intAreaId && _lstAllFunnyQuestion [i].strVideoUrl != "" && _lstAllFunnyQuestion [i].strVideoUrl01 != "") {
//			}	
		}

		return dicTourVideoQuestion;
	}

	public DicFunnyQuestion GetRandomUnreadFunnyQuestionByAreaId(int intAreaId) {
		var _lstUnreadFunnyQuestionByArea = GetLstUnreadFunnyQuestionByAreaId (intAreaId);
//		Debug.Log ("_lstUnreadFunnyQuestionByArea.Count= "+_lstUnreadFunnyQuestionByArea.Count.ToString());
		if (_lstUnreadFunnyQuestionByArea.Count == 0)
			return null;

//		int intVideoTourId = -1;
//
//		for (int i=0; i<_lstUnreadFunnyQuestionByArea.Count; i++) {
//			if (_lstUnreadFunnyQuestionByArea [i].strVideoUrl != "") {
//				intVideoTourId = i;
//				break;
//			}
//		}
//
//		if (intVideoTourId != -1) 
//			return _lstUnreadFunnyQuestionByArea [intVideoTourId];
		
		int intRandomIndex = UnityEngine.Random.Range (0, _lstUnreadFunnyQuestionByArea.Count);

		return _lstUnreadFunnyQuestionByArea [intRandomIndex];
	}
}
