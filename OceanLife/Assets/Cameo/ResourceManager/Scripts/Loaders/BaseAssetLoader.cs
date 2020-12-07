using UnityEngine;
using System;
using System.Collections;

namespace Cameo
{
	public class BaseAssetLoader : MonoBehaviour
	{
		public delegate void DownloadCallback(UnityEngine.Object downloadedObj);

		public virtual UnityEngine.Object GetAsset (string assetPath, Type type)
		{
			Debug.LogWarning("[BaseResourceLoader.GetAsset] Not implemented!");
			return null;
		}

		public virtual void GetAssetAsync(string assetPath, Type type, DownloadCallback callBack)
		{
			Debug.LogWarning("[BaseResourceLoader.GetAsset] Not implemented!");
		}
	}
}