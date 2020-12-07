using UnityEngine;
using System.Collections;
using Cameo;

public class TestMessageBox : MonoBehaviour 
{
	public void OpenMessageBox()
	{
		MessageBoxManager.Instance.ShowMessageBox ("Simple", "The Message");
	}
}
