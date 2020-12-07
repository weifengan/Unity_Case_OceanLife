using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Cameo;
using System.IO;
using LitJson;

public class CrossMatch : MonoBehaviour {
    private List<int> _lstAreaId;
    //private string jsonString;
    private JsonData jsonData;
    private string jsonString;
    private List<JsonData> _CreaturesKeyValuePairs = new List<JsonData>();
    public List<JsonData> AreaInfoList;
    public JsonData recordsCollection;
    public int CurrentAreaId;
    void Start()

    {


    }



    public void LoadJsonData()
    {
#if UNITY_EDITOR
        string jsonText = File.ReadAllText(Application.dataPath + "/Resources/jsonLstDicCreature.json");
#else
		string jsonText = File.ReadAllText (Application.persistentDataPath + "/JsonCache/OceanNotesData.json");
#endif
        

        jsonData = JsonMapper.ToObject(jsonText);
        
        
        //用list存成陣列
        recordsCollection = new JsonData();
        AreaInfoList = new List<JsonData>();


        //Debug.Log((int)jsonData[i]["intAreaId"]);

        for (int i = 0; i < jsonData.Count; i++)
        {
            //避開i==0的例外狀況
            if (i == 0)
            {
                //continue;
            }
            //避開區域88的例外狀況
            else if ((int)jsonData[i]["intAreaId"] == 88)
            {
                recordsCollection.Add(jsonData[i]);
                AreaInfoList.Add(recordsCollection);
                recordsCollection = new JsonData();
            }
            else if ((int)jsonData[i]["intAreaId"] == (int)jsonData[i - 1]["intAreaId"])
            {
                recordsCollection.Add(jsonData[i]);

            }
            else if ((int)jsonData[i]["intAreaId"] == (int)jsonData[i - 1]["intAreaId"])
            {
                recordsCollection.Add(jsonData[i]);

            }
            else if ((int)jsonData[i]["intAreaId"] != (int)jsonData[i - 1]["intAreaId"])
            {
                AreaInfoList.Add(recordsCollection);
                recordsCollection = new JsonData();
                recordsCollection.Add(jsonData[i]);
            }
            
        }

       // Debug.Log(jsonData.Count);

     //   Debug.Log(AreaInfoList[87][0]["strCreatureId"]);






    }

    public void OnBiometricClick()
    {

        List<int> _lstAreaId = new List<int>();
        //暫時設定讀到的區域為最多生物的40號，等有beacon設備再把此行改為_lstAreaId = Cameo.LocationManager.Instance.lstIntCurrentAreaIds;
        _lstAreaId.Add(40);

        //  string jsonString = File.ReadAllText(Application.dataPath + "/Starlite/JsonCache/OceanNotesData.json");//(1)

        //   jsonString = File.ReadAllText(Application.dataPath + "/Resources/jsonLstDicCreature.json");//(1)

        CurrentAreaId = _lstAreaId[0]-1;


        LoadJsonData();
        for (int i=0; i< AreaInfoList[CurrentAreaId].Count; i++)
        {
            Debug.Log(AreaInfoList[CurrentAreaId][i]["strCreatureId"]);
            Debug.Log(AreaInfoList[CurrentAreaId][i]["strGenusId"]);
            Debug.Log(AreaInfoList[CurrentAreaId][i]["strSpeciesId"]);
            Debug.Log(AreaInfoList[CurrentAreaId][i]["strCreatureName"]);
            Debug.Log(AreaInfoList[CurrentAreaId][i]["strThumbnailUrl"]);
        }




        // Debug.Log(jsonData["age"]);

        //    MessageBoxManager.Instance.BackgroundColor = new Color(0, 0, 0, 0);
        //   MessageBoxManager.Instance.ShowMessageBox("BeaconAreaMessageBox", _lstAreaId);
    }

    public void ForPhoneCam()
    {

        List<int> _lstAreaId = new List<int>();
        _lstAreaId.Add(40);
        //  string jsonString = File.ReadAllText(Application.dataPath + "/Starlite/JsonCache/OceanNotesData.json");//(1)

        //   jsonString = File.ReadAllText(Application.dataPath + "/Resources/jsonLstDicCreature.json");//(1)

        CurrentAreaId = _lstAreaId[0] - 1;


        LoadJsonData();
    }

    }

