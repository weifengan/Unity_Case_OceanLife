using UnityEngine;
using System.Collections;
using Cameo;

public class ScreenOrientationHelper : MonoBehaviour {

	public static void SetOrientationFree() {
		Screen.orientation = ScreenOrientation.AutoRotation;
	}

	public static void SetOrientationFixed(ScreenOrientation screenOrientation) {
		Screen.orientation = screenOrientation;
	}
}
