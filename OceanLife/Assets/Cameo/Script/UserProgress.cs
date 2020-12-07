using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using LitJson;
using Cameo;

public static class UserProgress {

	private static string strUserProgressFolderPath = Application.persistentDataPath + "/userProgress";
	private static string strReadFunnyQuestionIdFilePath = strUserProgressFolderPath + "/funnyQuestion.gd";
	private static string strCollectedCreatureIdFilePath = strUserProgressFolderPath + "/collectedCreature.gd";

	public static void DeleteUserProgress() {
		if (File.Exists (UserProgress.strReadFunnyQuestionIdFilePath))
			File.Delete (UserProgress.strReadFunnyQuestionIdFilePath);
		
		if (File.Exists (UserProgress.strCollectedCreatureIdFilePath))
			File.Delete (UserProgress.strCollectedCreatureIdFilePath);
	}

	public static void SaveLstReadFunnyQuestionId(List<int> _lstReadFunnyQuesitonId) {
		if (!System.IO.File.Exists (UserProgress.strUserProgressFolderPath)) {
			System.IO.Directory.CreateDirectory (UserProgress.strUserProgressFolderPath);
		}

		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (UserProgress.strReadFunnyQuestionIdFilePath);

		bf.Serialize (file, _lstReadFunnyQuesitonId);
		file.Close ();
	}

	public static List<int> LoadLstReadFunnyQuestionId() {
		List<int> _lstReadFunnyQuestionId = new List<int>();

		if (!File.Exists (UserProgress.strReadFunnyQuestionIdFilePath))
			return _lstReadFunnyQuestionId;

		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.OpenRead (UserProgress.strReadFunnyQuestionIdFilePath);
		_lstReadFunnyQuestionId = (List<int>)bf.Deserialize (file);
		file.Close ();

		return _lstReadFunnyQuestionId;
	}

	public static List<string> LoadLstCollectedCreatureId() {
		List<string> _lstCollectedCreatureId = new List<string>();

		if (!File.Exists (UserProgress.strCollectedCreatureIdFilePath))
			return _lstCollectedCreatureId;

		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.OpenRead (UserProgress.strCollectedCreatureIdFilePath);
		_lstCollectedCreatureId = (List<string>)bf.Deserialize (file);
		file.Close ();

		return _lstCollectedCreatureId;
	}

	public static void SaveLstCollectedCreatureId(List<string> _lstCollectedCreatureId) {
		if (!System.IO.File.Exists (UserProgress.strUserProgressFolderPath)) {
			System.IO.Directory.CreateDirectory (UserProgress.strUserProgressFolderPath);
		}

		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (UserProgress.strCollectedCreatureIdFilePath);

		bf.Serialize (file, _lstCollectedCreatureId);
		file.Close ();
	}
}
