using UnityEngine;
using System.Collections;
using Cameo;

public class CheckNetWork : MonoBehaviour
{
    public const int NotReachable = 0;                   // 沒有網路
    public const int ReachableViaLocalAreaNetwork = 1;   // 網路Wifi,網路線。
    public const int ReachableViaCarrierDataNetwork = 2; // 網路3G,4G。

    // Use this for initialization
    /*void Start()
    {
        // IPhone, Android
        int nStatus = ConnectionStatus();
        Debug.Log("ConnectionStatus : " + nStatus);
        if (nStatus > 0)
            Debug.Log("有連線狀態");
        else
            Debug.Log("無連線狀態");
    }*/

    public static int ConnectionStatus()
    {

        int nStatus;

        if (Application.internetReachability == NetworkReachability.NotReachable)
            nStatus = NotReachable;
        else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            nStatus = ReachableViaLocalAreaNetwork;
        else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
            nStatus = ReachableViaCarrierDataNetwork;
        else
            nStatus = -1;

        return nStatus;
    }
}

