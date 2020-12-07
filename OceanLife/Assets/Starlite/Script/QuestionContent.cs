using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
public class QuestionContent : MonoBehaviour
{
    public GameObject imageButton;
    public Text tiTle;
    public Text content;
    public GameObject question;
    public Text answerText;
    public GetVideo getVideo;
    public GameObject loading;
    public RectTransform layoutGroup;
    public VerticalLayoutGroup VerticalLG;
    // Use this for initialization
    void Start()
    {
        GoogleAnalytics.Client.SendEventHit("藍色海洋筆記_動動腦", OceanNotesData.CourseJsonData["strQuestion"]);

        TitlebarController.Instance.SetTitleText(OceanNotesData.CourseJsonData["strTypeName"]);
        TitlebarController.Instance.SetBackButtonClickCallback(OnBackClick);
        TitlebarController.Instance.ShowTitlebar();
		BackgroundController.Instance.SetCurrentBackgroundImage ("img/areaBg_color",new Color32 (255, 255, 255, 255));

        Button btn = imageButton.GetComponent<Button>();
        string videoID = OceanNotesData.CourseJsonData["strVideoUrl"];
        btn.onClick.AddListener(() =>
        {
            getVideo.PlayVido(videoID);
        });

        Image image = imageButton.GetComponent<Image>();
        string url = OceanNotesData.CourseJsonData["strImageUrl"];
        //url = url.Replace("https://www.youtube.com/watch?v=", string.Empty);
        //url = "http://img.youtube.com/vi/" + url + "/0.jpg";
        StartCoroutine(GetImage(image, url));

        tiTle.text = OceanNotesData.CourseJsonData["strTitle"];

        content.text = OceanNotesData.CourseJsonData["strIntro"];
		Debug.Log ("="+OceanNotesData.CourseJsonData["strIntro"]);
        Text questionText = question.transform.GetChild(0).GetComponent<Text>();
        questionText.text = OceanNotesData.CourseJsonData["strQuestion"];
        StartCoroutine(UpdataLayoutGroup());

        answerText.text = OceanNotesData.CourseJsonData["strAnswer"];
        if (answerText.text.Length > 120)
        {
            VerticalLG.enabled = true;
        }
    }

    void OnDestroy()
    {
        TitlebarController.Instance.RemoveBackButtonClickCallback(OnBackClick);
    }

    //獲取圖片
    IEnumerator GetImage(Image image, string url)
    {
#if UNITY_EDITOR
        string path = Application.dataPath + "/Starlite/ImageCache/";
#else
		string path = Application.persistentDataPath + "/ImageCache/";
#endif

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        if (!File.Exists(path + url.GetHashCode()))
        {
            yield return StartCoroutine(DownLoadImage(image, url));
        }
        else
        {
            yield return StartCoroutine(LoadImage(image, url));
        }

        loading.SetActive(false);
    }

    //下載圖片
    IEnumerator DownLoadImage(Image image, string url)
    {
        WWW www = new WWW(url);
        yield return www;

        if (www.error == null)
        {
            Debug.Log("DownloadImage OK!");
            Texture2D tex2d = www.texture;
            byte[] pngData = tex2d.EncodeToPNG();
#if UNITY_EDITOR
            File.WriteAllBytes(Application.dataPath + "/Starlite/ImageCache/" + url.GetHashCode(), pngData);
#else
			File.WriteAllBytes(Application.persistentDataPath + "/ImageCache/" +url.GetHashCode(),pngData);
#endif
            Sprite spr = Sprite.Create(tex2d, new Rect(0, 0, tex2d.width, tex2d.height), new Vector2(0, 0));
            image.sprite = spr;
        }
        else
        {
            Debug.Log("Error:" + www.error);
        }
    }

    // 載入圖片
    IEnumerator LoadImage(Image image, string url)
    {
#if UNITY_EDITOR
        string filePath = "file:///" + Application.dataPath + "/Starlite/ImageCache/" + url.GetHashCode();
#else
		string filePath = "file:///" + Application.persistentDataPath + "/ImageCache/" + url.GetHashCode();
#endif

        WWW loadFile = new WWW(filePath);
        yield return loadFile;

        if (loadFile.error == null)
        {
            Debug.Log("loadImage OK!");
            Texture2D tex2d = loadFile.texture;
            Sprite spr = Sprite.Create(tex2d, new Rect(0, 0, tex2d.width, tex2d.height), new Vector2(0, 0));
            image.sprite = spr;
        }
        else
        {
            Debug.Log(loadFile.error);
        }

    }

    IEnumerator UpdataLayoutGroup()
    {
        yield return new WaitForEndOfFrame();
        LayoutElement questionSize = question.GetComponent<LayoutElement>();
        questionSize.preferredHeight = question.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.y;
    }

    public void OnBackClick()
    {
        SceneNavigator.Instance.PopScene();
    }

    public void GoogleAnalyticsPost()
    {
        GoogleAnalytics.Client.SendEventHit("藍色海洋筆記_動動腦解答", OceanNotesData.CourseJsonData["strAnswer"]);
    }

    // Update is called once per frame
    void Update()
    {
    }


}
