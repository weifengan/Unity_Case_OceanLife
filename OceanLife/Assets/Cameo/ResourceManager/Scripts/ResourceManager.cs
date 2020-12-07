using UnityEngine;
using System.Collections;

namespace Cameo
{
	public class ResourceManager : Singleton<ResourceManager> 
	{
		private BaseAssetLoader _resLoader;

		void Awake()
		{
			AttachLoader<ResourceAssetLoader> ();
		}

		public Object GetAsset (string assetPath, System.Type type)
		{
			Object returnObj = null;

			//Check if resource is in resource store
			returnObj =  _resLoader.GetAsset (assetPath, type);

			return returnObj;
		}

		public void AttachLoader<T>() where T:BaseAssetLoader
		{
			_resLoader = gameObject.AddComponent<T> ();
		}
	}
}
