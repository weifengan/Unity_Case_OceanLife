using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.Collections;
using System.IO;

public class PresetIOSBuildSetting {

	[PostProcessBuild]
	public static void ChangeXcodePlist(BuildTarget buildTarget, string pathToBuiltProject) {

		if (buildTarget == BuildTarget.iOS) 
		{
			// Get plist
			string plistPath = pathToBuiltProject + "/Info.plist";
			PlistDocument plist = new PlistDocument();
			plist.ReadFromString(File.ReadAllText(plistPath));

			// Get root
			PlistElementDict rootDict = plist.root;
			rootDict.SetString ("Privacy - Location Always Usage Description", "需要使用您的位置資訊");
			rootDict.SetString ("Privacy - Location Usage Description", "需要使用您的位置資訊");
			rootDict.SetString("NSBluetoothPeripheralUsageDescription", "需要使用您的藍芽");
			File.WriteAllText(plistPath, plist.WriteToString());

			// 解決預設需要相機權限的問題
			var targetfile = pathToBuiltProject + "/Classes/Preprocessor.h";
			var filecontents = System.IO.File.ReadAllText(targetfile);
			string seed = "#define UNITY_USES_WEBCAM 1";
			string repl = "#define UNITY_USES_WEBCAM 0";

			if (filecontents.Contains(seed))
			{
				filecontents = filecontents.Replace(seed, repl);
				System.IO.File.WriteAllText (targetfile, filecontents);
			}
		}
	}
}