using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
using UnityEngine.UI;

public class DropdownContent : MonoBehaviour
{
    public CourseContent courseContent;
    public Text typeIntro;
	public Image ImgBtn01;
	public Image ImgBtn02;
	public Image ImgBtn03;
    private static string typeVauleTemp;


    void Awake()
    {
        if (string.IsNullOrEmpty(typeVauleTemp))
        {
            typeVauleTemp = "全部單元";

            OceanNotesData.TypeInfo.Add("全部單元", "你還未選擇任何類別喔!!");
        }
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetInit(JsonData jsonData)
    {
        for (int i = 0; i < jsonData.Count; i++)
        {
            string strTypeName = (string)jsonData[i]["strTypeName"];
            if (!OceanNotesData.TypeInfo.ContainsKey(strTypeName))
            {
                OceanNotesData.TypeInfo.Add(strTypeName, (string)jsonData[i]["strTypeNameIntro"]);
            }
        }

        Dictionary<string, string>.KeyCollection keyCol = OceanNotesData.TypeInfo.Keys;
        List<string> item = new List<string>();
        foreach (string key in keyCol)
        {
            item.Add(key);
        }
        GetComponent<Dropdown>().AddOptions(item);

        for (int i = 0; i < item.Count; i++)
        {
            if (item[i] == typeVauleTemp)
            {
                GetComponent<Dropdown>().value = i;
                if (i == 0)
                    SelectType();
                return;
            }
        }

    }

	public void SelectType01()
	{
		string typeName = "全部單元";
		courseContent.StartUpdata(typeName);
		typeIntro.text = OceanNotesData.TypeInfo[typeName];
		typeVauleTemp = typeName;
		ImgBtn01.gameObject.SetActive (true);
		ImgBtn02.gameObject.SetActive (false);
		ImgBtn03.gameObject.SetActive (false);
	}

	public void SelectType02()
	{
		string typeName = "海洋生物面面觀";
		courseContent.StartUpdata(typeName);
		typeIntro.text = OceanNotesData.TypeInfo[typeName];
		typeVauleTemp = typeName;
		ImgBtn01.gameObject.SetActive (false);
		ImgBtn02.gameObject.SetActive (true);
		ImgBtn03.gameObject.SetActive (false);
	}

	public void SelectType03()
	{
		string typeName = "海洋生物多樣性";
		courseContent.StartUpdata(typeName);
		typeIntro.text = OceanNotesData.TypeInfo[typeName];
		typeVauleTemp = typeName;
		ImgBtn01.gameObject.SetActive (false);
		ImgBtn02.gameObject.SetActive (false);
		ImgBtn03.gameObject.SetActive (true);
	}

    public void SelectType()
    {
        Dropdown dropdown = GetComponent<Dropdown>();
        string typeName = dropdown.options[dropdown.value].text;
		Debug.Log ("SelectTypeName= "+typeName);
        courseContent.StartUpdata(typeName);
        typeIntro.text = OceanNotesData.TypeInfo[typeName];
        typeVauleTemp = typeName;
    }
}
