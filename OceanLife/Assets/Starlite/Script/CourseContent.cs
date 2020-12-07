using UnityEngine;
using System.Collections;
using LitJson;
using System.IO;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CourseContent : MonoBehaviour
{
    public GetVideo getVideo;
    public GameObject loading;
    public string url;
    public GameObject cloneObj;
    public GameObject parentsObj;
    public DropdownContent dropdownContent;
    private JsonData jsonData;

    IEnumerator Start()
    {

        TitlebarController.Instance.SetTitleText("藍色海洋筆記");
        TitlebarController.Instance.SetBackButtonClickCallback(OnBackClick);
        TitlebarController.Instance.ShowTitlebar();

        yield return StartCoroutine("DownLoadJsonData", url);
        LoadJsonData();
        // 產生下拉式單元列表
        dropdownContent.SetInit(jsonData);
    }

    public void StartUpdata(string selectValue)
    {
        StartCoroutine(Filter(selectValue));
    }

    public IEnumerator Filter(string selectValue)
    {
        Debug.Log(selectValue);
        loading.SetActive(true);

        Cleared();

        for (int i = 0; i < jsonData.Count; i++)
        {
            Debug.Log(selectValue);
            string typeName = (string)jsonData[i]["strTypeName"];
			Debug.Log("typeName= "+typeName);
            if ((selectValue != typeName) && (selectValue != "全部單元"))
                continue;

            if ((i == jsonData.Count - 1) || (i % 6 == 0))
            {
                yield return StartCoroutine(GenerateList(jsonData[i]));
            }
            else
            {
                StartCoroutine(GenerateList(jsonData[i]));
            }
        }

        loading.SetActive(false);
    }

    private void Cleared()
    {
        int count = parentsObj.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            GameObject deleteObj = parentsObj.transform.GetChild(i).gameObject;
            Destroy(deleteObj);
        }
    }

    void OnDestroy()
    {
        TitlebarController.Instance.RemoveBackButtonClickCallback(OnBackClick);
    }
    // 取得Json資料
    IEnumerator DownLoadJsonData(string url)
    {
        WWW www = new WWW(url);
        yield return www;

        if (www.error == null)
        {
            Debug.Log("CourseContent:" + www.text);
            SaveJsonData(www.text);
        }
        else
        {
            Debug.Log("Error:" + www.error);
        }
    }
    void SaveJsonData(string jsonText)
    {
#if UNITY_EDITOR
        string path = Application.dataPath + "/Starlite/JsonCache/";
#else
		string path = Application.persistentDataPath + "/JsonCache/";
#endif
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        File.WriteAllText(path + "OceanNotesData.json", jsonText);
    }

    void LoadJsonData()
    {
#if UNITY_EDITOR
        string jsonText = File.ReadAllText(Application.dataPath + "/Starlite/JsonCache/OceanNotesData.json");
#else
		string jsonText = File.ReadAllText (Application.persistentDataPath + "/JsonCache/OceanNotesData.json");
#endif

        jsonData = JsonMapper.ToObject(jsonText);
    }

    // 產生列表
    IEnumerator GenerateList(JsonData jsonData)
    {
        GameObject item = Instantiate(cloneObj) as GameObject;
        item.transform.parent = parentsObj.transform;
        item.transform.localScale = Vector3.one;

        string imageUrl = (string)jsonData["strImageUrl"];
        Image image = item.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
        yield return StartCoroutine(GetImage(image, imageUrl));
        GameObject imageMask = item.transform.GetChild(0).GetChild(0).gameObject;
        //調整圖片大小
        image.SetNativeSize();
        float frameWidth = imageMask.GetComponent<RectTransform>().sizeDelta.x;
        float frameHeight = imageMask.GetComponent<RectTransform>().sizeDelta.y;
        float width = image.GetComponent<RectTransform>().sizeDelta.x;
        float height = image.GetComponent<RectTransform>().sizeDelta.y;
        float setSize = 0;
        if (width >= height)
        {
            setSize = frameHeight / height;
        }
        else
        {
            setSize = frameWidth / width;
        }
        image.GetComponent<RectTransform>().sizeDelta *= setSize;

        Text VideoTitle = item.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>();
        VideoTitle.text = (string)jsonData["strTitle"];

        string iconUrl = (string)jsonData["strTypeIconUrl"];
        Image iconImage = item.transform.GetChild(0).GetChild(2).GetComponent<Image>();
        StartCoroutine(GetImage(iconImage, iconUrl));

        string videoId = (string)jsonData["strVideoUrl"];

        item.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
        {
            OceanNotesData.CourseJsonData.Clear();
            OceanNotesData.CourseJsonData.Add("strTypeName", (string)jsonData["strTypeName"]);
            OceanNotesData.CourseJsonData.Add("strTitle", (string)jsonData["strTitle"]);
            OceanNotesData.CourseJsonData.Add("strIntro", (string)jsonData["strIntro"]);
            OceanNotesData.CourseJsonData.Add("strImageUrl", (string)jsonData["strImageUrl"]);
            OceanNotesData.CourseJsonData.Add("strVideoUrl", (string)jsonData["strVideoUrl"]);
            OceanNotesData.CourseJsonData.Add("strQuestion", (string)jsonData["strQuestion"]);
            OceanNotesData.CourseJsonData.Add("strAnswer", (string)jsonData["strAnswer"]);
          getVideo.PlayVido(videoId);
//			Application.OpenURL (videoId);
			
        });
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

    public void OnBackClick()
    {
        SceneNavigator.Instance.PopScene();
    }

    // Update is called once per frame
    void Update()
    {
    }
}


