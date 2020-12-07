using UnityEngine;
using System.Collections;

public class WebLink : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {

    }

    public void OpenUrl(string url)
    {
        Application.OpenURL(url);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
