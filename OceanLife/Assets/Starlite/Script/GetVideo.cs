using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GetVideo : MonoBehaviour
{
    public string sceneName;
	public static bool isPlayBlueVideo;


    public void PlayVido(string videoID)
    {
        GoogleAnalytics.Client.SendEventHit("藍色海洋筆記_學習影片", OceanNotesData.CourseJsonData["strTypeName"] + "_" + OceanNotesData.CourseJsonData["strTitle"]);
        VideoManagerSingleton.Instance.Play(videoID);

        var dicNextSceneInfo = new DicSceneInfo(sceneName, null);
        if (sceneName != string.Empty)
        {
            SceneNavigator.Instance.PushScene(dicNextSceneInfo);
        }
		isPlayBlueVideo = true;
    }
}
