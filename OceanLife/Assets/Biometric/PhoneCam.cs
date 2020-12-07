using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using LitJson;
using Cameo;
using UnityEngine.SceneManagement;


public class PhoneCam : MonoBehaviour {

    private bool camAvailable;
    private WebCamTexture backCam;
    private Texture deafultBackground;
    public string deviceName;
    public string screenShotURL = "203.66.83.170:11000/predict";
    private string localFileName = @"D:\user\Documents\GitHub\nmmba\NMMBA\Screenshot.png";
    private string m_URL = "203.66.83.170:11000/predict";
    public string BioResult;
	private JsonData jsonData;
    private JsonData jsonDataTemp;
	//舊系統
	private JsonData jsonDataSever;
	private JsonData jsonDataLocal;
	//

	private JsonData jsonDataApi;
	private JsonData jsonFishData;
	private JsonData jsonFishRecognition;
    private List<JsonData> BioResultJson = new List<JsonData>();
    private List<int> _lstAreaId;
    public CrossMatch crossmatch;

    

    string FirstOne;
    string SecondOne;

    public RawImage background;
    public AspectRatioFitter fit;
	public RawImage RawImgPicQ1;
	public RawImage RawImgPicQ2;
	public RawImage RawImgPicQ3;
	public RawImage RawImgPicQ4;
	public RawImage RawImgPicQ5;

    public GameObject buttom;
    public GameObject click;
	public GameObject PanelTakePic;
	public GameObject PanelQ1;
	public GameObject PanelQ2;
	public GameObject PanelQ3;
	public GameObject PanelQ4;
	public GameObject PanelQ5;
	public GameObject PanelResult ;
	public GameObject LoadingPage;

	public RawImage RawImgFish01;
	public RawImage RawImgFish02;
	public RawImage RawImgFish03;
	public RawImage RawImgFish04;
	public RawImage RawImgFish05;

	public Text TextFishName01;
	public Text TextFishName02;
	public Text TextFishName03;
	public Text TextFishName04;
	public Text TextFishName05;
	public Text TextFishPos01;
	public Text TextFishPos02;
	public Text TextFishPos03;
	public Text TextFishPos04;
	public Text TextFishPos05;


//	public Text TextTest;
	private string FishNameEn01 = "Parapercis_tetracantha";
	private string FishNameEn02 = "Lutjanus_vitta";
	private string FishNameEn03 = "Halichoeres_hortulanus";
	private string FishNameEn04 = "Halichoeres_hortulanus";
	private string FishNameEn05 = "Halichoeres_hortulanus";

	private int MaxPosFishIndex;
	private int MinPosFishIndex;
	private int NowBeaconAreaId = 0;
	public static int PageNumber = 0;

	private float FishPos01;
	private float FishPos02;
	private float FishPos03;
	private float FishPos04;
	private float FishPos05;

	private bool isQ1Finish = false;
	private bool isQ2Finish = false;
	private bool isQ3Finish = false;
	private bool isQ4Finish = false;
	private bool isQ5Finish = false;

	public ScrollRect _scrollRect;
	private Dictionary<string, object> _sceneSetting;
	private DicAreaInfo _dicAreaInfo;

	private Texture2D tmpTex;

	List<string> FishNameEn = new List<string>();
	List<string> FishNameCn = new List<string>();
	List<string> FishPicPath = new List<string>();
	List<float> FishPos = new List<float>();
	List<int> FishAreaId = new List<int>();
//	List<string> FishSize = new List<string> ();
//	List<string> FishColor = new List<string> ();
//	List<string> FishTail = new List<string> ();
//	List<string> FishShape = new List<string> ();
//	List<string> FishMarking = new List<string> ();

	List<string> LastFishNameEn = new List<string>();
	List<string> LastFishNameCn = new List<string>();
	List<string> LastFishPicPath = new List<string>();
	List<float> LastFishPos = new List<float>();
	List<int> LastFishAreaId = new List<int>();

    private void Start()
    {
		SceneNavigator.Instance.gameObject.SetActive (false);
		PanelQ1.gameObject.SetActive (false);
		PanelQ2.gameObject.SetActive (false);
		PanelQ3.gameObject.SetActive (false);
		PanelQ4.gameObject.SetActive (false);
		PanelQ5.gameObject.SetActive (false);
		LoadingPage.SetActive (false);
		PanelResult.gameObject.SetActive (false);

        //用於處理相機畫面
        deafultBackground = background.texture;
        WebCamDevice[]devices = WebCamTexture.devices;
        deviceName = devices[0].name;


        if (devices.Length == 0)
        {
            Debug.Log("No Camera Detected");
            camAvailable = false;
            return;
        }

        for (int i=0; i < devices.Length; i++)
        {
            //電腦無後鏡頭，先強制給予devices[0]
          //  if(!devices[i].isFrontFacing)
            {
                backCam = new WebCamTexture(devices[0].name,Screen.width , Screen.height);
            }
        }

        if (backCam == null)
        {
            Debug.Log("Unable to find back Camera");
            return;
        }

        backCam.Play();
        background.texture = backCam;

        camAvailable = true;

		_sceneSetting = SceneNavigator.Instance.GetCurrentSceneSetting ();
		if (CreatureInfoController.isBackFromCreatureInfo) 
		{
			PageNumber = 6;
			backCam.Stop();
			CreatureInfoController.isBackFromCreatureInfo = false;
			PanelQ1.gameObject.SetActive (false);
			PanelQ2.gameObject.SetActive (false);
			PanelQ3.gameObject.SetActive (false);
			PanelQ4.gameObject.SetActive (false);
			PanelQ5.gameObject.SetActive (false);
			LoadingPage.SetActive (false);
			PanelResult.gameObject.SetActive (true);

			TextFishName01.text = FishTempData.FishName01;
			TextFishName02.text = FishTempData.FishName02 ;
			TextFishName03.text = FishTempData.FishName03;
			TextFishName04.text = FishTempData.FishName04 ;
			TextFishName05.text = FishTempData.FishName05;

			FishNameEn01 = FishTempData.FishNameEn01;
			FishNameEn02 = FishTempData.FishNameEn02;
			FishNameEn03 = FishTempData.FishNameEn03;
			FishNameEn04 = FishTempData.FishNameEn04;
			FishNameEn05 = FishTempData.FishNameEn05;

			TextFishPos01.text = FishTempData.FishPos01 ;
			TextFishPos02.text = FishTempData.FishPos02 ;
			TextFishPos03.text = FishTempData.FishPos03 ;
			TextFishPos04.text = FishTempData.FishPos04 ;
			TextFishPos05.text = FishTempData.FishPos05 ;


			StartCoroutine(DownLoadFishImage (RawImgFish01, FishTempData.LastPicPathFish01));
			StartCoroutine(DownLoadFishImage (RawImgFish02, FishTempData.LastPicPathFish02));
			StartCoroutine(DownLoadFishImage (RawImgFish03, FishTempData.LastPicPathFish03));
			StartCoroutine(DownLoadFishImage (RawImgFish04, FishTempData.LastPicPathFish04));
			StartCoroutine(DownLoadFishImage (RawImgFish05, FishTempData.LastPicPathFish05));
		}	
    }


    private void Update()
    {
        if (!camAvailable)
            return;

        float ratio = (float)backCam.width / (float)backCam.height;
        fit.aspectRatio = ratio;

        float scaleY = backCam.videoVerticallyMirrored ? -1f : 1f;
        background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

        int orient = -backCam.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
		//檢測用
//		TextTest.text = 
		Debug.Log("PageNumber= "+PageNumber.ToString()+"\t"+"IsAndroidBackClick= "+SceneNavigator.IsAndroidBackClick.ToString());
		if (Input.GetKeyUp(KeyCode.Escape))  
		{
			switch (PageNumber)
			{
			case 0:
				backCam.Stop ();
				SceneManager.LoadScene ("Main");
				break;
			case 1:
				GoToTakePicPage ();
				break;
			case 2:
				PanelQ1.gameObject.SetActive (true);
				PanelQ2.gameObject.SetActive (false);
				RawImgPicQ1.texture = (Texture)FishTempData.FishPic;
				PageNumber = 1;
				break;
			case 3:
				PanelQ2.gameObject.SetActive (true);
				PanelQ3.gameObject.SetActive (false);
				RawImgPicQ2.texture = (Texture)FishTempData.FishPic;
				PageNumber = 2;
				break;
			case 4:
				PanelQ3.gameObject.SetActive (true);
				PanelQ4.gameObject.SetActive (false);
				RawImgPicQ3.texture = (Texture)FishTempData.FishPic;
				PageNumber = 3;
				break;
			case 5:
				PanelQ4.gameObject.SetActive (true);
				PanelQ5.gameObject.SetActive (false);
				RawImgPicQ4.texture = (Texture)FishTempData.FishPic;
				PageNumber = 4;
				break;
			case 6:
				PanelQ5.gameObject.SetActive (true);
				PanelResult.gameObject.SetActive (false);
				RawImgPicQ5.texture = (Texture)FishTempData.FishPic;
				PageNumber = 5;
				break;
			}
			SceneNavigator.IsAndroidBackClick = false;
		}	
    }

//    public Texture2D heightmap;
    public Vector3 size = new Vector3(100, 10, 100);


    //若要改以拍照方式存取，改用以下程式碼產生按鈕
  /*  void OnGUI()
    {
        if (GUI.Button(new Rect(10, 70, 50, 30), "Click"))
        {
            TakeSnapshot();
            print(Application.persistentDataPath);
            ScreenShot();
        }

    }*/

    // For saving to the _savepath
    private string _SavePath = "C:/WebcamSnaps/"; //Change the path here!
    int _CaptureCounter = 0;

//    //目前不是使用拍照存圖
//    void TakeSnapshot()
//    {
//        Texture2D snap = new Texture2D(backCam.width, backCam.height);
//        snap.SetPixels(backCam.GetPixels());
//        snap.Apply();
//
////		Sprite spr = Sprite.Create(snap, new Rect(0, 0, snap.width, snap.height), new Vector2(0, 0));
////		ImgPic.sprite = spr;
//		Debug.Log("path= "+_SavePath + _CaptureCounter.ToString());
//        System.IO.File.WriteAllBytes(_SavePath + _CaptureCounter.ToString() + ".png", snap.EncodeToPNG());
//        ++_CaptureCounter;
//    }


    //截圖時會等候一秒讓UI消失
    IEnumerator WaitForShot()
    {
        yield return new WaitForSeconds(1);
//        buttom.SetActive(true);
//        click.SetActive(true);
		background.gameObject.SetActive(false);
		PanelQ1.gameObject.SetActive (true);
		RawImgPicQ1.texture = (Texture)tmpTex;
		FishTempData.FishPic=(Texture)tmpTex;
    }
		
    public void ScreenShot()
    {
        buttom.SetActive (false);
        click.SetActive (false);
		PanelTakePic.gameObject.SetActive(false);
        //  Application.CaptureScreenshot("Screenshot.png");
        StartCoroutine(UploadPNG());
        StartCoroutine(WaitForShot()); 
    }








	//////////////////////////////////////////////////////////////// 
	public void Q1ClickAnswerA()
	{
		for (int i = 0; i < FishNameEn.Count; i++) 
		{
			if(FishNameEn[i] == "Labroides_dimidiatus" || FishNameEn[i] ==  "Periophthalmus_modestus" || FishNameEn[i] == "Pterapogon_kauderni" || FishNameEn[i] == "Dascyllus_melanurus" || FishNameEn[i] =="Sphaeramia_nematoptera")
			{
				AddFishPos(FishPos,i,0.05f);
			}	
		}	
		GoogleAnalytics.Client.SendEventHit("生物辨識_功能頁", "展示生物大小為XS (<10cm)");
		isQ1Finish = true;
	}

	public void Q1ClickAnswerB()
	{
		for (int i = 0; i < FishNameEn.Count; i++) 
		{
			if(FishNameEn[i] == "Acrossocheilus_paradoxus" || FishNameEn[i] ==  "Microcanthus_strigatus" || FishNameEn[i] ==  "Halichoeres_marginatus" || FishNameEn[i] ==  "Monodactylus_argenteus" || FishNameEn[i] ==  "Abudefduf_vaigiensis")
			{
				AddFishPos(FishPos,i,0.05f);
			}	
		}	
		GoogleAnalytics.Client.SendEventHit("生物辨識_功能頁", "展示生物大小為S (10-30cm)");
		isQ1Finish = true;
	}

	public void Q1ClickAnswerC()
	{
		for (int i = 0; i < FishNameEn.Count; i++) 
		{
			if(FishNameEn[i] == "Lutjanus_russelli" || FishNameEn[i] ==  "Lutjanus_stellatus" || FishNameEn[i] ==  "Acanthurus_dussumieri" || FishNameEn[i] ==  "Acanthurus_mata" || FishNameEn[i] ==  "Calotomus_carolinus")
			{
				AddFishPos(FishPos,i,0.05f);
			}	
		}
		GoogleAnalytics.Client.SendEventHit("生物辨識_功能頁", "展示生物大小為M (30-60cm)");
		isQ1Finish = true;
	}

	public void Q1ClickAnswerD()
	{
		for (int i = 0; i < FishNameEn.Count; i++) 
		{
			if(FishNameEn[i] == "Lutjanus_erythropterus" || FishNameEn[i] ==  "Plectorhinchus_pictus" || FishNameEn[i] ==  "Bodianus_perditio" || FishNameEn[i] ==  "Lethrinus_nebulosus" || FishNameEn[i] ==  "Oplegnathus_fasciatus")
			{
				AddFishPos(FishPos,i,0.05f);
			}	
		}
		GoogleAnalytics.Client.SendEventHit("生物辨識_功能頁", "展示生物大小為L (60-100cm)");
		isQ1Finish = true;
	}

	public void Q1ClickAnswerE()
	{
		for (int i = 0; i < FishNameEn.Count; i++) 
		{
			if(FishNameEn[i] == "Rhinoptera_javanica" || FishNameEn[i] ==  "Aluterus_scriptus" || FishNameEn[i] ==  "Mylopharyngodon_piceus" || FishNameEn[i] ==  "Elagatis_bipinnulata" || FishNameEn[i] ==  "Chanos_chanos")
			{
				AddFishPos(FishPos,i,0.05f);
			}	
		}
		GoogleAnalytics.Client.SendEventHit("生物辨識_功能頁", "展示生物大小為XL (>1m)");
		isQ1Finish = true;
	}

	public void Q2ClickAnswerA()
	{
		for (int i = 0; i < FishNameEn.Count; i++) 
		{
			if(FishNameEn[i] == "Chromis_viridis" || FishNameEn[i] ==  "Thalassoma_lunare" || FishNameEn[i] ==  "Halichoeres_hortulanus" || FishNameEn[i] ==  "Hemigymnus_melapterus" || FishNameEn[i] ==  "Hologymnosus_annulatus")
			{
				AddFishPos(FishPos,i,0.05f);
			}	
		}
		GoogleAnalytics.Client.SendEventHit("生物辨識_功能頁", "展示生物的顏色為藍、綠");
		isQ2Finish = true;
	}

	public void Q2ClickAnswerB()
	{
		for (int i = 0; i < FishNameEn.Count; i++) 
		{
			if(FishNameEn[i] == "Dascyllus_trimaculatus" || FishNameEn[i] ==  "Acanthurus_maculiceps" || FishNameEn[i] ==  "Chaetodon_baronessa" || FishNameEn[i] ==  "Parupeneus_ciliatus" || FishNameEn[i] ==  "Carcharhinus_melanopterus")
			{
				AddFishPos(FishPos,i,0.05f);
			}	
		}
		GoogleAnalytics.Client.SendEventHit("生物辨識_功能頁", "展示生物的顏色為深 (黑、灰)");
		isQ2Finish = true;
	}

	public void Q2ClickAnswerC()
	{
		for (int i = 0; i < FishNameEn.Count; i++) 
		{
			if(FishNameEn[i] == "Carcharhinus_leucas" || FishNameEn[i] ==  "Acrossocheilus_paradoxus" || FishNameEn[i] ==  "Spinibarbus_hollandi" || FishNameEn[i] ==  "Rhinecanthus_verrucosus" || FishNameEn[i] ==  "Terapon_jarbua")
			{
				AddFishPos(FishPos,i,0.05f);
			}	
		}
		GoogleAnalytics.Client.SendEventHit("生物辨識_功能頁", "展示生物的顏色為銀 (銀、白)");
		isQ2Finish = true;
	}

	public void Q2ClickAnswerD()
	{
		for (int i = 0; i < FishNameEn.Count; i++) 
		{
			if(FishNameEn[i] == "Scarus_psittacus" || FishNameEn[i] ==  "Plotosus_lineatus" || FishNameEn[i] ==  "Gymnothorax_flavimarginatus" || FishNameEn[i] ==  "Cantherhines_dumerilii" || FishNameEn[i] ==  "Chrysiptera_brownriggii")
			{
				AddFishPos(FishPos,i,0.05f);
			}	
		}
		GoogleAnalytics.Client.SendEventHit("生物辨識_功能頁", "展示生物的顏色為黃 (黃、棕)");
		isQ2Finish = true;
	}

	//q3
	public void Q3ClickAnswerA()
	{
		for (int i = 0; i < FishNameEn.Count; i++) 
		{
			if(FishNameEn[i] == "Halichoeres_hortulanus" || FishNameEn[i] ==  "Heniochus_acuminatus" || FishNameEn[i] ==  "Melichthys_vidua" || FishNameEn[i] ==  "Siganus_spinus" || FishNameEn[i] ==  "Sufflamen_bursa")
			{
				AddFishPos(FishPos,i,0.05f);
			}	
		}
		GoogleAnalytics.Client.SendEventHit("生物辨識_功能頁", "展示生物的尾鰭是截形");
		isQ3Finish = true;
	}

	public void Q3ClickAnswerB()
	{
		for (int i = 0; i < FishNameEn.Count; i++) 
		{
			if(FishNameEn[i] == "Carcharhinus_melanopterus" || FishNameEn[i] ==  "Triaenodon_obesus" || FishNameEn[i] ==  "Chromis_analis" || FishNameEn[i] ==  "Dascyllus_aruanus" || FishNameEn[i] ==  "Sphaeramia_nematoptera")
			{
				AddFishPos(FishPos,i,0.05f);
			}	
		}
		GoogleAnalytics.Client.SendEventHit("生物辨識_功能頁", "展示生物的尾鰭是叉形");
		isQ3Finish = true;
	}

	public void Q3ClickAnswerC()
	{
		for (int i = 0; i < FishNameEn.Count; i++) 
		{
			if(FishNameEn[i] == "Naso_lituratus" || FishNameEn[i] ==  "Seriola_dumerili" || FishNameEn[i] ==  "Thunnus_albacares" || FishNameEn[i] ==  "Acanthurus_nigrofuscus" || FishNameEn[i] ==  "Acanthurus_xanthopterus")
			{
				AddFishPos(FishPos,i,0.05f);
			}	
		}
		GoogleAnalytics.Client.SendEventHit("生物辨識_功能頁", "展示生物的尾鰭是月形");
		isQ3Finish = true;
	}

	public void Q3ClickAnswerD()
	{
		for (int i = 0; i < FishNameEn.Count; i++) 
		{
			if(FishNameEn[i] == "Labroides_dimidiatus" || FishNameEn[i] ==  "Amphiprion_clarkii" || FishNameEn[i] ==  "Amphiprion_frenatus" || FishNameEn[i] ==  "Amphiprion_ocellaris" || FishNameEn[i] ==  "Centropyge_bicolor")
			{
				AddFishPos(FishPos,i,0.05f);
			}	
		}
		GoogleAnalytics.Client.SendEventHit("生物辨識_功能頁", "展示生物的尾鰭是圓形");
		isQ3Finish = true;
	}

	public void Q3ClickAnswerE()
	{
		for (int i = 0; i < FishNameEn.Count; i++) 
		{
			if(FishNameEn[i] == "Microcanthus_strigatus" || FishNameEn[i] ==  "Lutjanus_quinquelineatus" || FishNameEn[i] ==  "Scarus_ghobban" || FishNameEn[i] ==  "Lutjanus_stellatus" || FishNameEn[i] ==  "Siganus_fuscescens")
			{
				AddFishPos(FishPos,i,0.05f);
			}	
		}
		GoogleAnalytics.Client.SendEventHit("生物辨識_功能頁", "展示生物的尾鰭是凹型");
		isQ3Finish = true;
	}

	public void Q3ClickAnswerF()
	{
		for (int i = 0; i < FishNameEn.Count; i++) 
		{
			if(FishNameEn[i] == "Aetobatus_narinari" || FishNameEn[i] ==  "Dasyatis_akajei" || FishNameEn[i] ==  "Gymnothorax_favagineus" || FishNameEn[i] ==  "Plotosus_lineatus" || FishNameEn[i] ==  "Gymnothorax_flavimarginatus")
			{
				AddFishPos(FishPos,i,0.05f);
			}	
		}
		GoogleAnalytics.Client.SendEventHit("生物辨識_功能頁", "展示生物的尾鰭是尖形");
		isQ3Finish = true;
	}

	public void Q3ClickAnswerG()
	{
		for (int i = 0; i < FishNameEn.Count; i++) 
		{
			if(FishNameEn[i] == "Naso_thynnoides")
			{
				AddFishPos(FishPos,i,0.2f);
			}	
		}
		GoogleAnalytics.Client.SendEventHit("生物辨識_功能頁", "展示生物的尾鰭是彎月形");
		isQ3Finish = true;
	}

	public void Q3ClickAnswerH()
	{
		for (int i = 0; i < FishNameEn.Count; i++) 
		{
			if(FishNameEn[i] == "Plectorhinchus_lessonii" || FishNameEn[i] ==  "Terapon_jarbua" || FishNameEn[i] ==  "Zanclus_cornutus" || FishNameEn[i] ==  "Acanthurus_japonicus" || FishNameEn[i] ==  "Gomphosus_varius")
			{
				AddFishPos(FishPos,i,0.05f);
			}	
		}
		GoogleAnalytics.Client.SendEventHit("生物辨識_功能頁", "展示生物的尾鰭是內凹形");
		isQ3Finish = true;
	}
	//

	//Q4
	public void Q4ClickAnswerA()
	{
		for (int i = 0; i < FishNameEn.Count; i++) 
		{
			if(FishNameEn[i] == "Chanodichthys_erythropterus" || FishNameEn[i] ==  "Mylopharyngodon_piceus" || FishNameEn[i] ==  "Megalobrama_amblycephala" || FishNameEn[i] ==  "Cyprinus_carpio" || FishNameEn[i] ==  "Caranx_melampygus")
			{
				AddFishPos(FishPos,i,0.05f);
			}	
		}
		GoogleAnalytics.Client.SendEventHit("生物辨識_功能頁", "展示生物的形狀為側扁型");
		isQ4Finish = true;
	}

	public void Q4ClickAnswerB()
	{
		for (int i = 0; i < FishNameEn.Count; i++) 
		{
			if(FishNameEn[i] == "Parapercis_pulchella" || FishNameEn[i] ==  "Holothuria_atra" || FishNameEn[i] ==  "Gymnothorax_chilospilus" || FishNameEn[i] ==  "Gymnothorax_flavimarginatus" || FishNameEn[i] ==  "Gymnothorax_thyrsoideus")
			{
				AddFishPos(FishPos,i,0.05f);
			}	
		}
		GoogleAnalytics.Client.SendEventHit("生物辨識_功能頁", "展示生物的形狀為長條型");
		isQ4Finish = true;
	}

	public void Q4ClickAnswerC()
	{
		for (int i = 0; i < FishNameEn.Count; i++) 
		{
			if(FishNameEn[i] == "Tripneustes_gratilla")
			{
				AddFishPos(FishPos,i,0.05f);
			}	
		}
		GoogleAnalytics.Client.SendEventHit("生物辨識_功能頁", "展示生物的形狀為圓型");
		isQ4Finish = true;
	}

	public void Q4ClickAnswerD()
	{
		for (int i = 0; i < FishNameEn.Count; i++) 
		{
			if(FishNameEn[i] == "Carcharhinus_melanopterus" || FishNameEn[i] ==  "Epinephelus_lanceolatus" || FishNameEn[i] ==  "Parupeneus_barberinus" || FishNameEn[i] ==  "Parupeneus_heptacanthus" || FishNameEn[i] ==  "Parupeneus_indicus")
			{
				AddFishPos(FishPos,i,0.05f);
			}	
		}
		GoogleAnalytics.Client.SendEventHit("生物辨識_功能頁", "展示生物的形狀為紡錘型");
		isQ4Finish = true;
	}

	public void Q4ClickAnswerE()
	{
		for (int i = 0; i < FishNameEn.Count; i++) 
		{
			if(FishNameEn[i] == "Canthigaster_valentini" || FishNameEn[i] ==  "Canthigaster_axiologus" )
			{
				AddFishPos(FishPos,i,0.05f);
			}	
		}
		GoogleAnalytics.Client.SendEventHit("生物辨識_功能頁", "展示生物的形狀為球形");
		isQ4Finish = true;
	}

	public void Q4ClickAnswerF()
	{
		for (int i = 0; i < FishNameEn.Count; i++) 
		{
			if(FishNameEn[i] == "Delphinapterus_leucas" )
			{
				AddFishPos(FishPos,i,0.2f);
			}	
		}
		GoogleAnalytics.Client.SendEventHit("生物辨識_功能頁", "展示生物的形狀為鯨形");
		isQ4Finish = true;
	}

	public void Q4ClickAnswerG()
	{
		for (int i = 0; i < FishNameEn.Count; i++) 
		{
			if(FishNameEn[i] == "Arothron_hispidus" || FishNameEn[i] ==  "Arothron_mappa" || FishNameEn[i] ==  "Arothron_manilensis" || FishNameEn[i] ==  "Arothron_reticularis" || FishNameEn[i] ==  "Arothron_stellatus")
			{
				AddFishPos(FishPos,i,0.05f);
			}	
		}
		GoogleAnalytics.Client.SendEventHit("生物辨識_功能頁", "展示生物的形狀為箱形");
		isQ4Finish = true;
	}

	public void Q4ClickAnswerH()
	{
		for (int i = 0; i < FishNameEn.Count; i++) 
		{
			if(FishNameEn[i] == "Portunus_sanguinolentus")
			{
				AddFishPos(FishPos,i,0.2f);
			}	
		}
		GoogleAnalytics.Client.SendEventHit("生物辨識_功能頁", "展示生物的形狀為蟹形");
		isQ4Finish = true;
	}

	//
	//Q5

	public void Q5ClickAnswerA()
	{
		for (int i = 0; i < FishNameEn.Count; i++) 
		{
			if(FishNameEn[i] == "Plectorhinchus_picus" || FishNameEn[i] ==  "Diodon_holocanthus" || FishNameEn[i] ==  "Anguilla_marmorata")
			{
				AddFishPos(FishPos,i,0.05f);
			}	
		}
		GoogleAnalytics.Client.SendEventHit("生物辨識_功能頁", "展示生物的紋路為斑塊");
		isQ5Finish = true;
	}

	public void Q5ClickAnswerB()
	{
		for (int i = 0; i < FishNameEn.Count; i++) 
		{
			if(FishNameEn[i] == "Abudefduf_vaigiensis" || FishNameEn[i] ==  "Calotomus_carolinus" || FishNameEn[i] ==  "Arothron_reticularis")
			{
				AddFishPos(FishPos,i,0.05f);
			}	
		}
		GoogleAnalytics.Client.SendEventHit("生物辨識_功能頁", "展示生物的紋路為條紋");
		isQ5Finish = true;
	}

	public void Q5ClickAnswerC()
	{
		for (int i = 0; i < FishNameEn.Count; i++) 
		{
			if(FishNameEn[i] == "Diodon_holocanthus" || FishNameEn[i] ==  "Pomacanthus_semicirculatus" || FishNameEn[i] ==  "Acanthurus_maculiceps")
			{
				AddFishPos(FishPos,i,0.05f);
			}	
		}
		GoogleAnalytics.Client.SendEventHit("生物辨識_功能頁", "展示生物的紋路為點");
		isQ5Finish = true;
	}

	//
	public void GoToHomePage()
	{
		backCam.Stop();
//		var dicMainMenuSceneInfo = new DicSceneInfo ("Main", null, "img/mainMenuBg");
//		dicMainMenuSceneInfo.SetBgColor(new Color32 (69, 162, 180, 255));
//		SceneNavigator.Instance.PushScene (dicMainMenuSceneInfo);

		SceneNavigator.Instance.PopScene();
		PageNumber = 0;

	}


	public void GoToTakePicPage()
	{
		PageNumber = 0;
		backCam.Play();
		background.gameObject.SetActive (true);
		buttom.SetActive (true);
		click.SetActive (true);
		PanelTakePic.gameObject.SetActive(true);
		PanelQ1.gameObject.SetActive (false);
	}


	public void GoToPanelQ1()
	{
		PageNumber = 1;
		backCam.Stop();
		PanelQ1.gameObject.SetActive (true);
		PanelQ2.gameObject.SetActive (false);
		RawImgPicQ1.texture = (Texture)FishTempData.FishPic;
	}

	public void GoToPanelQ2()
	{
		PageNumber = 2;
//		if (isQ1Finish) 
//		{
			PanelQ1.gameObject.SetActive (false);
			PanelQ2.gameObject.SetActive (true);
			PanelQ3.gameObject.SetActive (false);
			RawImgPicQ2.texture = (Texture)FishTempData.FishPic;
			
//		}	
	}

	public void GoToPanelQ3()
	{
		PageNumber = 3;
//		if (isQ2Finish) 
//		{
			PanelQ2.gameObject.SetActive (false);
			PanelQ3.gameObject.SetActive (true);
			PanelQ4.gameObject.SetActive (false);
			RawImgPicQ3.texture = (Texture)FishTempData.FishPic;
//		}	
	}

	public void GoToPanelQ4()
	{
		PageNumber = 4;
//		if (isQ3Finish) 
//		{
			PanelQ3.gameObject.SetActive (false);
			PanelQ4.gameObject.SetActive (true);
			PanelQ5.gameObject.SetActive (false);
			RawImgPicQ4.texture = (Texture)FishTempData.FishPic;
//		}
	}

	public void GoToPanelQ5()
	{
		PageNumber = 5;
//		if (isQ4Finish) 
//		{
			PanelQ4.gameObject.SetActive (false);
			PanelQ5.gameObject.SetActive (true);
			PanelResult.gameObject.SetActive (false);
			RawImgPicQ5.texture = (Texture)FishTempData.FishPic;
//		}
	}

	public void GoToPanelResult()
	{
		PageNumber = 6;
		backCam.Stop();
		Ranking ();
//		if (isQ5Finish) 
//		{
			PanelQ5.gameObject.SetActive (false);
			PanelResult.gameObject.SetActive (true);
			LoadingPage.SetActive (true);
			StartCoroutine ("WaitShowPanelResult");
//		}

	}

	IEnumerator WaitShowPanelResult()
	{
		yield return new WaitForSeconds(8); 
		LoadingPage.SetActive (false);
	}

	IEnumerator DownLoadFishImage(RawImage FishPic ,string url)
	{
		WWW www = new WWW(url);
		yield return www;

		if (www.error == null)
		{
			Debug.Log("DownloadFishImage OK!");
			Texture2D tex2d = www.texture;
			FishPic.texture = tex2d;
		}
		else
		{
			Debug.Log("Error:" + www.error);
		}
	}

	public void GoCreaturInfoFish01() {
		//SaveCurrentScrollPosition ();
		PageNumber = 7;
		//Debug.Log ("[CreatureListController] OnCreatureListItemClick / strCreatureId: " + strClickCreatureId);
		//設定下一個場景資訊
		Dictionary<string, object> dicSceneSetting = new Dictionary<string, object> ();
//		dicSceneSetting.Add ("strBuildingName", (string) _sceneSetting ["strBuildingName"]);
//		dicSceneSetting.Add ("intBuildingId", (int) _sceneSetting ["intBuildingId"]);
//		dicSceneSetting.Add ("intAreaId", _dicAreaInfo.intAreaId);
		dicSceneSetting.Add ("strBuildingName", "珊瑚王國館");
		dicSceneSetting.Add ("intBuildingId", 2);
		dicSceneSetting.Add ("intAreaId", 39);
		dicSceneSetting.Add ("strCreatureId", FishNameEn01);//魚的學名(英文id)
		Debug.Log("FishNameEn01= "+FishNameEn01);
		FishTempData.OnClickFishName = TextFishName01.text;
//		dicSceneSetting.Add ("strCreatureId", strClickCreatureId);//魚的學名(英文id)
		var dicNextSceneInfo = new DicSceneInfo ("CreatureInfo", dicSceneSetting);
		string strBuildingName = AreaInfoManager.Instance.GetStrBuildingNameByAreaId (39);
//		string strBuildingName = AreaInfoManager.Instance.GetStrBuildingNameByAreaId (_dicAreaInfo.intAreaId);
//		DicBuildingInfo _dicBuildingInfo = AreaInfoManager.Instance.GetBuildingInfoByName (strBuildingName);
		DicBuildingInfo _dicBuildingInfo = AreaInfoManager.Instance.GetBuildingInfoByName ("珊瑚王國館");
		dicNextSceneInfo.strBgImageUrl = "img/bg_color";	//下一頁背景圖
//		dicNextSceneInfo.strBgImageUrl = "img/areaBg_" + _dicBuildingInfo.strBuildingCode;	//下一頁背景圖
		dicNextSceneInfo.BgColor = new Color32 (99, 99, 99, 255);

		GoogleAnalytics.Client.SendEventHit("生物辨識_功能頁", "從辨識結果轉跳到"+FishNameEn01+"的介紹頁面");

		SceneNavigator.Instance.PushScene (dicNextSceneInfo);
	}

	public void GoCreaturInfoFish02() {
		//SaveCurrentScrollPosition ();
		PageNumber = 7;
		//Debug.Log ("[CreatureListController] OnCreatureListItemClick / strCreatureId: " + strClickCreatureId);
		//設定下一個場景資訊
		Dictionary<string, object> dicSceneSetting = new Dictionary<string, object> ();
		//		dicSceneSetting.Add ("strBuildingName", (string) _sceneSetting ["strBuildingName"]);
		//		dicSceneSetting.Add ("intBuildingId", (int) _sceneSetting ["intBuildingId"]);
		//		dicSceneSetting.Add ("intAreaId", _dicAreaInfo.intAreaId);
		dicSceneSetting.Add ("strBuildingName", "珊瑚王國館");
		dicSceneSetting.Add ("intBuildingId", 2);
		dicSceneSetting.Add ("intAreaId", 40);
		dicSceneSetting.Add ("strCreatureId", FishNameEn02);//魚的學名(英文id)
		FishTempData.OnClickFishName = TextFishName02.text;
		//		dicSceneSetting.Add ("strCreatureId", strClickCreatureId);//魚的學名(英文id)
		var dicNextSceneInfo = new DicSceneInfo ("CreatureInfo", dicSceneSetting);
		string strBuildingName = AreaInfoManager.Instance.GetStrBuildingNameByAreaId (40);
		//		string strBuildingName = AreaInfoManager.Instance.GetStrBuildingNameByAreaId (_dicAreaInfo.intAreaId);
		//		DicBuildingInfo _dicBuildingInfo = AreaInfoManager.Instance.GetBuildingInfoByName (strBuildingName);
		DicBuildingInfo _dicBuildingInfo = AreaInfoManager.Instance.GetBuildingInfoByName ("珊瑚王國館");
		dicNextSceneInfo.strBgImageUrl = "img/bg_color";	//下一頁背景圖
		//		dicNextSceneInfo.strBgImageUrl = "img/areaBg_" + _dicBuildingInfo.strBuildingCode;	//下一頁背景圖
		dicNextSceneInfo.BgColor = new Color32 (99, 99, 99, 255);

		GoogleAnalytics.Client.SendEventHit("生物辨識_功能頁", "從辨識結果轉跳到"+FishNameEn02+"的介紹頁面");

		SceneNavigator.Instance.PushScene (dicNextSceneInfo);
	}

	public void GoCreaturInfoFish03() {
		//SaveCurrentScrollPosition ();
		PageNumber = 7;
		//Debug.Log ("[CreatureListController] OnCreatureListItemClick / strCreatureId: " + strClickCreatureId);
		//設定下一個場景資訊
		Dictionary<string, object> dicSceneSetting = new Dictionary<string, object> ();
		//		dicSceneSetting.Add ("strBuildingName", (string) _sceneSetting ["strBuildingName"]);
		//		dicSceneSetting.Add ("intBuildingId", (int) _sceneSetting ["intBuildingId"]);
		//		dicSceneSetting.Add ("intAreaId", _dicAreaInfo.intAreaId);
		dicSceneSetting.Add ("strBuildingName", "珊瑚王國館");
		dicSceneSetting.Add ("intBuildingId", 2);
		dicSceneSetting.Add ("intAreaId", 47);
		dicSceneSetting.Add ("strCreatureId",FishNameEn03);//魚的學名(英文id)
		FishTempData.OnClickFishName = TextFishName03.text;
		//		dicSceneSetting.Add ("strCreatureId", strClickCreatureId);//魚的學名(英文id)
		var dicNextSceneInfo = new DicSceneInfo ("CreatureInfo", dicSceneSetting);
		string strBuildingName = AreaInfoManager.Instance.GetStrBuildingNameByAreaId (47);
		//		string strBuildingName = AreaInfoManager.Instance.GetStrBuildingNameByAreaId (_dicAreaInfo.intAreaId);
		//		DicBuildingInfo _dicBuildingInfo = AreaInfoManager.Instance.GetBuildingInfoByName (strBuildingName);
		DicBuildingInfo _dicBuildingInfo = AreaInfoManager.Instance.GetBuildingInfoByName ("珊瑚王國館");
		dicNextSceneInfo.strBgImageUrl = "img/bg_color";	//下一頁背景圖
		//		dicNextSceneInfo.strBgImageUrl = "img/areaBg_" + _dicBuildingInfo.strBuildingCode;	//下一頁背景圖
		dicNextSceneInfo.BgColor = new Color32 (99, 99, 99, 255);

		GoogleAnalytics.Client.SendEventHit("生物辨識_功能頁", "從辨識結果轉跳到"+FishNameEn03+"的介紹頁面");

		SceneNavigator.Instance.PushScene (dicNextSceneInfo);
	}

	public void GoCreaturInfoFish04() {
		//SaveCurrentScrollPosition ();
		PageNumber = 7;
		//Debug.Log ("[CreatureListController] OnCreatureListItemClick / strCreatureId: " + strClickCreatureId);
		//設定下一個場景資訊
		Dictionary<string, object> dicSceneSetting = new Dictionary<string, object> ();
		//		dicSceneSetting.Add ("strBuildingName", (string) _sceneSetting ["strBuildingName"]);
		//		dicSceneSetting.Add ("intBuildingId", (int) _sceneSetting ["intBuildingId"]);
		//		dicSceneSetting.Add ("intAreaId", _dicAreaInfo.intAreaId);
		dicSceneSetting.Add ("strBuildingName", "珊瑚王國館");
		dicSceneSetting.Add ("intBuildingId", 2);
		dicSceneSetting.Add ("intAreaId", 47);
		dicSceneSetting.Add ("strCreatureId",FishNameEn04);//魚的學名(英文id)
		FishTempData.OnClickFishName = TextFishName04.text;
		//		dicSceneSetting.Add ("strCreatureId", strClickCreatureId);//魚的學名(英文id)
		var dicNextSceneInfo = new DicSceneInfo ("CreatureInfo", dicSceneSetting);
		string strBuildingName = AreaInfoManager.Instance.GetStrBuildingNameByAreaId (47);
		//		string strBuildingName = AreaInfoManager.Instance.GetStrBuildingNameByAreaId (_dicAreaInfo.intAreaId);
		//		DicBuildingInfo _dicBuildingInfo = AreaInfoManager.Instance.GetBuildingInfoByName (strBuildingName);
		DicBuildingInfo _dicBuildingInfo = AreaInfoManager.Instance.GetBuildingInfoByName ("珊瑚王國館");
		dicNextSceneInfo.strBgImageUrl = "img/bg_color";	//下一頁背景圖
		//		dicNextSceneInfo.strBgImageUrl = "img/areaBg_" + _dicBuildingInfo.strBuildingCode;	//下一頁背景圖
		dicNextSceneInfo.BgColor = new Color32 (99, 99, 99, 255);

		GoogleAnalytics.Client.SendEventHit("生物辨識_功能頁", "從辨識結果轉跳到"+FishNameEn04+"的介紹頁面");

		SceneNavigator.Instance.PushScene (dicNextSceneInfo);
	}

	public void GoCreaturInfoFish05() {
		//SaveCurrentScrollPosition ();
		PageNumber = 7;
		//Debug.Log ("[CreatureListController] OnCreatureListItemClick / strCreatureId: " + strClickCreatureId);
		//設定下一個場景資訊
		Dictionary<string, object> dicSceneSetting = new Dictionary<string, object> ();
		//		dicSceneSetting.Add ("strBuildingName", (string) _sceneSetting ["strBuildingName"]);
		//		dicSceneSetting.Add ("intBuildingId", (int) _sceneSetting ["intBuildingId"]);
		//		dicSceneSetting.Add ("intAreaId", _dicAreaInfo.intAreaId);
		dicSceneSetting.Add ("strBuildingName", "珊瑚王國館");
		dicSceneSetting.Add ("intBuildingId", 2);
		dicSceneSetting.Add ("intAreaId", 47);
		dicSceneSetting.Add ("strCreatureId",FishNameEn05);//魚的學名(英文id)
		FishTempData.OnClickFishName = TextFishName05.text;
		//		dicSceneSetting.Add ("strCreatureId", strClickCreatureId);//魚的學名(英文id)
		var dicNextSceneInfo = new DicSceneInfo ("CreatureInfo", dicSceneSetting);
		string strBuildingName = AreaInfoManager.Instance.GetStrBuildingNameByAreaId (47);
		//		string strBuildingName = AreaInfoManager.Instance.GetStrBuildingNameByAreaId (_dicAreaInfo.intAreaId);
		//		DicBuildingInfo _dicBuildingInfo = AreaInfoManager.Instance.GetBuildingInfoByName (strBuildingName);
		DicBuildingInfo _dicBuildingInfo = AreaInfoManager.Instance.GetBuildingInfoByName ("珊瑚王國館");
		dicNextSceneInfo.strBgImageUrl = "img/bg_color";	//下一頁背景圖
		//		dicNextSceneInfo.strBgImageUrl = "img/areaBg_" + _dicBuildingInfo.strBuildingCode;	//下一頁背景圖
		dicNextSceneInfo.BgColor = new Color32 (99, 99, 99, 255);

		GoogleAnalytics.Client.SendEventHit("生物辨識_功能頁", "從辨識結果轉跳到"+FishNameEn05+"的介紹頁面");

		SceneNavigator.Instance.PushScene (dicNextSceneInfo);
	}

	private void SaveCurrentScrollPosition() {
		float floatContentY = _scrollRect.content.anchoredPosition.y;

		Dictionary<string, object> _dicCurrentSceneSetting = SceneNavigator.Instance.GetCurrentSceneSetting ();
		if (_dicCurrentSceneSetting.ContainsKey ("floatContentY")) {
			_dicCurrentSceneSetting ["floatContentY"] = floatContentY;
		} else {
			_dicCurrentSceneSetting.Add ("floatContentY", floatContentY);
		}
	}

	//////////////////////////////////////////////////////////////// 
    //源自嘉呈的程式碼，上傳截圖(轉為bytecode)
    IEnumerator UploadPNG()
	{
		
		GoogleAnalytics.Client.SendEventHit("生物辨識_功能頁", "點擊拍照按鈕");
		// We should only read the screen after all rendering is complete
		yield return new WaitForEndOfFrame ();

		// Create a texture the size of the screen, RGB24 format
		int width = Screen.width;
		int height = Screen.height;
		var tex = new Texture2D (Screen.width, Screen.width, TextureFormat.RGB24, false);

		// Read screen contents into the texture
//		tex.ReadPixels (new Rect (0, 0, tmpTex.width, tmpTex.width), 0, 0);
		tex.ReadPixels (new Rect (0, 0.4f * Screen.height, Screen.width, Screen.width), 0, 0);
		tex.Apply ();
		tmpTex = tex;

		// Encode texture into PNG
		byte[] bytes = tex.EncodeToPNG ();
//		Destroy(tex);
		// Create a Web Form
		WWWForm form = new WWWForm ();
		form.AddField ("frameCount", Time.frameCount.ToString ());
		form.AddBinaryData ("file", bytes, "screenShot.png", "image/png");
		// Upload to a cgi script
//		TextTest.text = screenShotURL ;
		Debug.Log("screenShotURL= "+screenShotURL);
		WWW w = new WWW (screenShotURL, form);
		// WWW w = new WWW("file://" + localFileName);
		//  background.SetActive(false);
		// UI_display_predicting(true);

		yield return w;
		if (w.error == null) {
//			TextTest.text = w.text ;
			Debug.Log ("upload done :" + w.text);
			Debug.Log ("bbbbbbbbbbbbbbbbbbb");
		} 
		else 
		{
//			TextTest.text = w.error;
			Debug.Log("伺服器無法連線");
			Debug.Log (w.error);
		}	


        BioResult = w.text;
//		TextTest.text = BioResult;
        
        //用於資料交叉比對與排序
        BeaconCheck();

       
        if (!string.IsNullOrEmpty(w.error))
        {
        //    UI_display_predicting(false);
        //    prediction.SetActive(true);
            print(w.error);
        }

        else
        {
        /*    UI_display_predicting(false);
            background.SetActive(true);
            prediction.SetActive(true);
            Processjson(w.text.ToLower());*/
//            Debug.Log(w);
        }
    }
		
	IEnumerator CreateFishData()
	{
		WWW www = new WWW("http://nmmba.tapmovie.com/jsonData/jsonCreatureFeatures.json");
		yield return www;
		if (www.error == null) 
		{
			string jsonText = www.text;
			jsonFishData = JsonMapper.ToObject(jsonText);
			for (int i = 0; i < jsonFishData.Count; i++) 
			{
				FishNameEn.Add (jsonFishData [i] ["strCreatureId"].ToString ());
				FishNameCn.Add (jsonFishData [i] ["strCreatureName"].ToString ());
				FishPicPath.Add (jsonFishData [i] ["strThumbnailUrl"].ToString ());
				FishPos.Add (0);
				FishAreaId.Add (int.Parse (jsonFishData [i] ["intAreaId"].ToString ()));
//				FishSize.Add(jsonFishData [i] ["Feature_size"].ToString ());
//				FishColor.Add (jsonFishData [i] ["Feature_color"].ToString ());
//				FishTail.Add (jsonFishData [i] ["Feature_tail"].ToString ());
//				FishShape.Add (jsonFishData [i] ["Feature_shape"].ToString ());
//				FishMarking.Add (jsonFishData [i] ["Feature_marking"].ToString ());
			}
			//檢測用
//			for (int j = 0; j < jsonFishData.Count; j++) 
//			{
//				Debug.Log ("FishNameEn= " + FishNameEn [j].ToString () + "\n" + "FishNameCn= " + FishNameCn [j] .ToString ()
//					+ "\n" + "FishPicPath= " + FishPicPath [j] .ToString ()
//					+ "\n" + "FishPos= " + FishPos [j].ToString ()
//					+ "\n" + "FishAreaId= " + FishAreaId [j].ToString ());
//			}
		}
		AddFishRecognition();
	}

	private void AddFishRecognition()
	{
		jsonFishRecognition  = JsonMapper.ToObject(BioResult);
		for (int i = 0; i < jsonFishRecognition.Count; i++)
		{
			for (int j = 0; j < jsonFishData.Count; j++)
			{
				if (jsonFishRecognition [i] ["name"].ToString () == jsonFishData [j] ["strCreatureId"].ToString ()) 
				{
					FishPos.RemoveAt(j);
					FishPos.Insert(j,float.Parse(jsonFishRecognition[i]["pos"].ToString()));
				}	
			}
		}
//		for (int k = 0; k < jsonFishData.Count; k++) 
//		{
//			Debug.Log ("FishNameEn= " + FishNameEn [k].ToString () + "\n" + "FishNameCn= " + FishNameCn [k] .ToString ()
//				+ "\n" + "FishPicPath= " + FishPicPath [k] .ToString ()
//				+ "\n" + "FishPos= " + FishPos [k].ToString ()
//				+ "\n" + "FishAreaId= " + FishAreaId [k].ToString ());
//		}
		AddBeaconPos ();
	}

	private void AddBeaconPos()
	{
		NowBeaconAreaId = Cameo.LocationManager.Instance.intCurrentAreaId;
		//測試用假資料
//		NowBeaconAreaId = 39;
		if (NowBeaconAreaId != 0) 
		{
			for (int i = 0; i < jsonFishData.Count; i++) 
			{
				if (jsonFishData [i] ["intAreaId"].ToString () == NowBeaconAreaId.ToString()) 
				{
					AddFishPos(FishPos,i,0.1f);
//					float tempPos = FishPos [i];
//					FishPos.RemoveAt (i);
//					FishPos.Insert (i,0.1f+tempPos);
				}
			}
		}	
		//檢測用
//					for (int j = 0; j < jsonFishData.Count; j++) 
//					{
//						Debug.Log ("FishNameEn= " + FishNameEn [j].ToString () + "\n" + "FishNameCn= " + FishNameCn [j] .ToString ()
//							+ "\n" + "FishPicPath= " + FishPicPath [j] .ToString ()
//							+ "\n" + "FishPos= " + FishPos [j].ToString ()
//							+ "\n" + "FishAreaId= " + FishAreaId [j].ToString ());
//					}

	}

	private void AddFishPos(List<float> _fishPos ,int _index ,float _addVar)
	{
		float tempPos = _fishPos [_index];
		_fishPos.RemoveAt (_index);
		_fishPos.Insert (_index,_addVar+tempPos);
	}

    //用於資料交叉比對與排序
    public void BeaconCheck()
    {
		jsonDataSever = JsonMapper.ToObject(BioResult);
		StartCoroutine ("CreateFishData");

//		string jsonText = crossmatch.recordsCollection

//		#if UNITY_EDITOR
//		string jsonText = File.ReadAllText(Application.dataPath + "/Resources/jsonLstDicCreature.json");
//		jsonDataLocal = JsonMapper.ToObject(jsonText);
//		#else
////		Object tempJsonObject = Resources.Load("jsonLstDicCreature");
////		string jsonPhoneText = JsonMapper.ToJson (tempJsonObject);
////		jsonDataLocal = JsonMapper.ToObject(jsonPhoneText);
//		string jsonText = File.ReadAllText (Application.persistentDataPath + "/JsonCache/OceanNotesData.json");
//		#endif
////		jsonDataLocal = Resources.Load("jsonLstDicCreature");
//
//		for (int i = 0; i < jsonDataSever.Count; i++) 
//		{
////			TextTest.text = "ssssssssssssssssss";
//			for (int j = 0; j < jsonDataLocal.Count; j++) 
//			{
//				if (jsonDataSever [i] ["name"].ToString () == jsonDataLocal [j] ["strCreatureId"].ToString ()) {
//					if (!FishNameEn.Contains (jsonDataSever [i] ["name"].ToString ())) {
////						TextTest.text = "FishName= "+jsonDataLocal[j]["strCreatureId"].ToString()+"\n"+"FishPos= "+jsonDataSever[0]["pos"].ToString();
//						FishNameEn.Add (jsonDataLocal [j] ["strCreatureId"].ToString ());
//						FishNameCn.Add (jsonDataLocal [j] ["strCreatureName"].ToString ());
//						FishPicPath.Add (jsonDataLocal [j] ["strThumbnailUrl"].ToString ());
//						FishPos.Add (float.Parse (jsonDataSever [i] ["pos"].ToString ()));
//						FishAreaId.Add (int.Parse (jsonDataLocal [j] ["intAreaId"].ToString ()));
//					}	
//					Debug.Log ("FishNameEn= " + jsonDataLocal [j] ["strCreatureId"].ToString () + "\n" + "FishNameCn= " + jsonDataLocal [j] ["strCreatureName"].ToString ()
//					+ "\n" + "FishPicPath= " + jsonDataLocal [j] ["strThumbnailUrl"].ToString ()
//					+ "\n" + "FishPos= " + jsonDataSever [i] ["pos"].ToString ());
//				} 
//				else 
//				{
//					Debug.Log ("aaaaaaaaaaa");
//				}	
//			}	
//		}	
			


		//印出所有辨識出來的魚
//		Debug.Log (jsonDataSever[i]["name"].ToString());
//		TextTest.text = jsonData[0]["pos"].ToString();
//		print (jsonData[1]["pos"].ToString());
//        crossmatch.ForPhoneCam();
//        print("該區的生物種類：");
//		print("*****crossmatch.CurrentAreaId： "+crossmatch.CurrentAreaId);
//        print(crossmatch.AreaInfoList[crossmatch.CurrentAreaId].Count); 
		//////////////////////////////////////////

//		TextFishName01.text = jsonData [MaxPosFishIndex] ["name"].ToString ();
//		TextFishName03.text = jsonData [MinPosFishIndex] ["name"].ToString ()
//
//		FishPos01 = float.Parse(jsonData [MaxPosFishIndex] [0].ToString()) * 10;
////		FishPos02 = float.Parse(jsonData [1] [0].ToString())  * 10;
//		FishPos03 = float.Parse(jsonData [MinPosFishIndex] [0].ToString())  * 10;
//
//		TextFishPos01.text = FishPos01.ToString ()+"%";
////		TextFishPos02.text = FishPos02.ToString ()+"%";
//		TextFishPos03.text = FishPos03.ToString ()+"%";


		/// ////////////////////////////////////////////////

//        for (int j=0; j<5; j++)
//        {
//            for (int i = 0; i < crossmatch.AreaInfoList[crossmatch.CurrentAreaId].Count; i++)
//            {
////				TextTest.text = jsonData[j]["name"].ToString() ;
//                //若在Beacon內有找到符合資料，將其符合的pos+1，排序就會拉到最前
//                if (jsonData[j]["name"].ToString() == crossmatch.AreaInfoList[crossmatch.CurrentAreaId][i]["strCreatureId"].ToString())
//                {
//                    print("找到位於該區的魚類：");
////					print ("aaaaaaaa"+jsonData[j]["name"].ToString());
////					TextTest.text = jsonData[j]["name"].ToString() ;
////                    print(jsonData[j]["name"]);
////					print(jsonData[j]["pos"]);
////					print(crossmatch.AreaInfoList[crossmatch.CurrentAreaId][i]["strCreatureName"]);
////					print(crossmatch.AreaInfoList[crossmatch.CurrentAreaId][i]["strThumbnailUrl"]);
//                    jsonData[j]["pos"] = float.Parse(jsonData[j]["pos"].ToString()) + 1;
////					FishUrl01 = crossmatch.AreaInfoList [crossmatch.CurrentAreaId] [j] ["strThumbnailUrl"];
////					FishUrl02 = crossmatch.AreaInfoList [crossmatch.CurrentAreaId] [j] ["strThumbnailUrl"];
////					FishUrl03 = crossmatch.AreaInfoList [crossmatch.CurrentAreaId] [j] ["strThumbnailUrl"];
////					Debug.Log("***strThumbnailUrl = "+crossmatch.AreaInfoList[crossmatch.CurrentAreaId][j]["strThumbnailUrl"]);
////					if (j == 0) 
////					{
////						StartCoroutine (DownLoadFishImage( crossmatch.AreaInfoList[crossmatch.CurrentAreaId][0]["strThumbnailUrl"].ToString() ));
////					}
////
////					if (j == 1) 
////					{
////						
////					}	
////
////					if (j == 2) 
////					{
////						
////					}
//                }           
//            }
//        }
//        //把資料進行排序並印出
//        Sortjson();
    }

	public void Ranking()
	{
		//rank
		float tempPos;
		string tempNameCn;
		string tempNameEn;
		string tempPicPath;
		int Flag = 1; //旗標
		int w;
		for (w = 1; w <= FishPos.Count - 1 && Flag == 1; w++)
		{    // 外層迴圈控制比較回數
			Flag = 0;
			for (int j = 1; j <= FishPos.Count - w; j++) 
			{  // 內層迴圈控制每回比較次數            
				if (FishPos[j] > FishPos[j - 1]) 
				{  // 比較鄰近兩個物件，右邊比左邊小時就互換。	       
					tempPos = FishPos[j];
					tempNameCn = FishNameCn [j];
					tempNameEn = FishNameEn [j];
					tempPicPath = FishPicPath [j];

					FishPos[j] = FishPos[j - 1];
					FishPos[j - 1] = tempPos;

					FishNameCn [j] = FishNameCn [j - 1];
					FishNameCn [j - 1] = tempNameCn;

					FishNameEn [j] = FishNameEn [j - 1];
					FishNameEn [j - 1] = tempNameEn;

					FishPicPath [j] = FishPicPath [j - 1];
					FishPicPath [j - 1] = tempPicPath;
					Flag = 1;
				} 	
			}             
		}   

		RemoveTheSameFish ();





//				for (int k = 0; k < 5; k++) 
//				{
//					Debug.Log ("LastFishNameEn= " + LastFishNameEn [k].ToString ()
//						+ "\n" + "LastFishNameCn= " + LastFishNameCn [k].ToString ()
//						+ "\n" + "LastFishPicPath= " + LastFishPicPath [k].ToString ()
//						+ "\n" + "LastFishPos= " + LastFishPos [k].ToString ()
//						+ "\n" + "LastFishAreaId= " + LastFishAreaId [k].ToString ());
//				}


	}

	private void NoOutsize90Pa()
	{
		for (int i = 0; i < 5; i++) 	
		{
			if (LastFishPos [i] >= 0.9f) 
			{
				LastFishPos.RemoveAt (i);
				LastFishPos.Insert (i, 0.9f);
			}	
		}	
		ShowResultInformation ();
	}

	private void RemoveTheSameFish()
	{
		for (int i = 0; i < FishNameEn.Count; i++) 
		{
			if (!LastFishNameEn.Contains (FishNameEn [i])) 
			{
				LastFishNameEn.Add (FishNameEn [i]);
				LastFishNameCn.Add (FishNameCn [i]);
				LastFishPicPath.Add (FishPicPath [i]);
				LastFishPos.Add (FishPos [i]);
				LastFishAreaId.Add (FishAreaId [i]);
			} 
			else 
			{
				continue;
			}
		}	

//				for (int k = 0; k < FishNameEn.Count; k++) 
//				{
//					Debug.Log ("LastFishNameEn= " + LastFishNameEn [k].ToString ()
//						+ "\n" + "LastFishNameCn= " + LastFishNameCn [k].ToString ()
//						+ "\n" + "LastFishPicPath= " + LastFishPicPath [k].ToString ()
//						+ "\n" + "LastFishPos= " + LastFishPos [k].ToString ()
//						+ "\n" + "LastFishAreaId= " + LastFishAreaId [k].ToString ());
//				}
		NoOutsize90Pa();
	}

	private void ShowResultInformation()
	{
		TextFishName01.text = LastFishNameCn [0].ToString ();
		TextFishName02.text = LastFishNameCn [1].ToString ();
		TextFishName03.text = LastFishNameCn [2].ToString ();
		TextFishName04.text = LastFishNameCn [3].ToString ();
		TextFishName05.text = LastFishNameCn [4].ToString ();
		FishTempData.FishName01 = LastFishNameCn [0].ToString ();
		FishTempData.FishName02 = LastFishNameCn [1].ToString ();
		FishTempData.FishName03 = LastFishNameCn [2].ToString ();
		FishTempData.FishName04 = LastFishNameCn [3].ToString ();
		FishTempData.FishName05 = LastFishNameCn [4].ToString ();

		FishNameEn01 = LastFishNameEn [0].ToString ();
		FishNameEn02 = LastFishNameEn [1].ToString ();
		FishNameEn03 = LastFishNameEn [2].ToString ();
		FishNameEn04 = LastFishNameEn [3].ToString ();
		FishNameEn05 = LastFishNameEn [4].ToString ();
		FishTempData.FishNameEn01 = LastFishNameCn [0].ToString ();
		FishTempData.FishNameEn02 = LastFishNameCn [1].ToString ();
		FishTempData.FishNameEn03 = LastFishNameCn [2].ToString ();
		FishTempData.FishNameEn04 = LastFishNameCn [3].ToString ();
		FishTempData.FishNameEn05 = LastFishNameCn [4].ToString ();


		TextFishPos01.text = float.Parse(LastFishPos[0].ToString ())*100+"%";
		TextFishPos02.text = float.Parse(LastFishPos[1].ToString ())*100+"%";
		TextFishPos03.text = float.Parse(LastFishPos[2].ToString ())*100+"%";
		TextFishPos04.text = float.Parse(LastFishPos[3].ToString ())*100+"%";
		TextFishPos05.text = float.Parse(LastFishPos[4].ToString ())*100+"%";
		FishTempData.FishPos01 = float.Parse(LastFishPos[0].ToString ())*100+"%";
		FishTempData.FishPos02 = float.Parse(LastFishPos[1].ToString ())*100+"%";
		FishTempData.FishPos03 = float.Parse(LastFishPos[2].ToString ())*100+"%";
		FishTempData.FishPos04 = float.Parse(LastFishPos[3].ToString ())*100+"%";
		FishTempData.FishPos05 = float.Parse(LastFishPos[4].ToString ())*100+"%";


		StartCoroutine(DownLoadFishImage (RawImgFish01, LastFishPicPath [0].ToString ()));
		StartCoroutine(DownLoadFishImage (RawImgFish02, LastFishPicPath [1].ToString ()));
		StartCoroutine(DownLoadFishImage (RawImgFish03, LastFishPicPath [2].ToString ()));
		StartCoroutine(DownLoadFishImage (RawImgFish04, LastFishPicPath [3].ToString ()));
		StartCoroutine(DownLoadFishImage (RawImgFish05, LastFishPicPath [4].ToString ()));
		FishTempData.LastPicPathFish01 = LastFishPicPath [0].ToString ();
		FishTempData.LastPicPathFish02 = LastFishPicPath [1].ToString ();
		FishTempData.LastPicPathFish03 = LastFishPicPath [2].ToString ();
		FishTempData.LastPicPathFish04 = LastFishPicPath [3].ToString ();
		FishTempData.LastPicPathFish05 = LastFishPicPath [4].ToString ();
	}

    public void Sortjson()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < (5 - i - 1); j++)
            {
//                 Debug.Log(jsonData[j][0]);
                FirstOne = jsonData[j][0].ToString();
                SecondOne = jsonData[j + 1][0].ToString();
                if (float.Parse(FirstOne) < float.Parse(SecondOne))
                {
                    jsonDataTemp = jsonData[j][0];
                    jsonData[j][0] = jsonData[j + 1][0];
                    jsonData[j + 1][0] = jsonDataTemp;

                    jsonDataTemp = jsonData[j][1];
                    jsonData[j][1] = jsonData[j + 1][1];
                    jsonData[j + 1][1] = jsonDataTemp;

                }
            }
        }

        print("辨識結果已排序");
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 2; j++)
            {
//				Debug.Log("jsonData["+i+"]["+j+"] ="+jsonData[i][j]);

				print(jsonData[j]["name"]);
				print(jsonData[j]["pos"]);
				print(crossmatch.AreaInfoList[crossmatch.CurrentAreaId][i]["strCreatureName"]);
				print(crossmatch.AreaInfoList[crossmatch.CurrentAreaId][i]["strThumbnailUrl"]);
				print ("==============================");
//				TextTest.text = "Name= " + jsonData [0] ["name"] + "\n" + "Pos= " + jsonData [0] ["pos"];
				TextFishName01.text = crossmatch.AreaInfoList[crossmatch.CurrentAreaId][0]["strCreatureName"].ToString();
				StartCoroutine (DownLoadFishImage(RawImgFish01,crossmatch.AreaInfoList[crossmatch.CurrentAreaId][0]["strThumbnailUrl"].ToString()));
				TextFishName02.text = crossmatch.AreaInfoList[crossmatch.CurrentAreaId][1]["strCreatureName"].ToString();
				StartCoroutine (DownLoadFishImage(RawImgFish02,crossmatch.AreaInfoList[crossmatch.CurrentAreaId][1]["strThumbnailUrl"].ToString()));
				TextFishName03.text = crossmatch.AreaInfoList[crossmatch.CurrentAreaId][2]["strCreatureName"].ToString();
				StartCoroutine (DownLoadFishImage(RawImgFish03,crossmatch.AreaInfoList[crossmatch.CurrentAreaId][2]["strThumbnailUrl"].ToString()));
				FishPos01 = float.Parse(jsonData [0] [0].ToString()) * 10;
				FishPos02 = float.Parse(jsonData [1] [0].ToString())  * 10;
				FishPos03 = float.Parse(jsonData [2] [0].ToString())  * 10;
            }
        }
		TextFishPos01.text = FishPos01.ToString ()+"%";
		TextFishPos02.text = FishPos02.ToString ()+"%";
		TextFishPos03.text = FishPos03.ToString ()+"%";


    }
}


public class valueData:MonoBehaviour
{ public string nameEn; public string picName; public string nameCn;}

