using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using Cameo;

public class Config : Singleton<Config> {
	public bool isDeleteUserProgress;
	public bool isTestBeacon;
	public string strServerIp;
}
