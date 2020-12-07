using UnityEngine;
using System.Collections;


public class FileUpload : MonoBehaviour
{
    private string m_LocalFileName = @"D:\user\Documents\GitHub\nmmba\NMMBA\Screenshot.png";
    private string m_URL = "203.66.83.170:11000/predict";


    IEnumerator UploadFileCo(string localFileName, string uploadURL)
    {
        WWW localFile = new WWW("file://" + localFileName);
        yield return localFile;
        if (localFile.error == null)
            Debug.Log("Loaded file successfully");
        else
        {
            Debug.Log("Open file error: " + localFile.error);
            yield break; // stop the coroutine here
        }
        WWWForm postForm = new WWWForm();
        // version 1
        //postForm.AddBinaryData("theFile",localFile.bytes);
        // version 2
        postForm.AddBinaryData("File", localFile.bytes, localFileName, "image/png");
        WWW upload = new WWW(uploadURL, postForm);
        yield return upload;
        if (upload.error == null)
            Debug.Log("upload done :" + upload.text);
        else
            Debug.Log("Error during upload: " + upload.error);
    }


    void UploadFile(string localFileName, string uploadURL)
    {
        StartCoroutine(UploadFileCo(localFileName, uploadURL));
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
        m_LocalFileName = GUILayout.TextField(m_LocalFileName);
        m_URL = GUILayout.TextField(m_URL);
        if (GUILayout.Button("Upload"))
        {
            UploadFile(m_LocalFileName, m_URL);
        }
        GUILayout.EndArea();

    }
}
