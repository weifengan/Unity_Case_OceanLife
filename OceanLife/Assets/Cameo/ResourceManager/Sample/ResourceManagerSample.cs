using UnityEngine;
using System.Collections;

namespace Cameo
{
	public class ResourceManagerSample : MonoBehaviour {

		// Use this for initialization
		void Start () {
			Object testObjRes = ResourceManager.Instance.GetAsset ("TestResource", typeof(Object));
			GameObject testObj = GameObject.Instantiate (testObjRes as GameObject);
			testObj.transform.position = Vector3.zero;
		}
	}
}
