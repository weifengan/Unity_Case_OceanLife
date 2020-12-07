using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using Cameo;


public class TestDownloadCacheRead : MonoBehaviour {

	public Text _textHolder;
	public Image _imageHolder;

	// Use this for initialization
	void Start () {
		//TextAsset _test = (TextAsset) ResourceManager.Instance.GetAsset ("json/3d6555cb2f793d459f9a40f1ae7dc13d", typeof(TextAsset));

		//Debug.Log (_test);
		//Debug.Log(MD5.Instance.getStrMd5String("http://image.nmmba.gov.tw/upload/archive/20901/thumbnail_20160318101345.jpg"));
		DataCenter.Instance.GetJson ("http://findit.org.tw/api/getLstNewsForIndex", OnLoadJsonDataCallback, null);
		DataCenter.Instance.GetThumbnail ("http://image.nmmba.gov.tw/upload/archive/20901/thumbnail_20160318101345.jpg", onLoadImageReturn, null);

//		DownloadHelper.Instance.DownloadData ("http://findit.org.tw/imgAssets/banner.png", DownloadHelper.DataType.IMAGE, onLoadImageReturn, null);
	}

	private void OnLoadJsonDataCallback(DataResult dataResult) {
		Debug.Log ("[TestDownloadCacheRead] OnLoadJsonDataCallback / dataResult: " + JsonMapper.ToJson(dataResult));
		_textHolder.text = (string)dataResult.Result;
	}

	private void onLoadImageReturn(DataResult dataResult) {
		Debug.Log ("[TestDownloadCacheRead] onLoadJsonReturn / loadResult.IsSuccess: " + dataResult.IsSuccess.ToString());
		//Texture2D downloadTexture = (Texture2D) result.Result;

		_imageHolder.sprite = (Sprite)dataResult.Result;
	}
}
