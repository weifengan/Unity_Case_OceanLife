using UnityEngine;
using System.Collections;

namespace Cameo
{
	public class ResourceAssetLoader : BaseAssetLoader 
	{
		public override Object GetAsset (string assetPath, System.Type type)
		{
			return Resources.Load (assetPath, type);
		}
	}
}