using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TypeScrollManager : MonoBehaviour
{
    private int itemNumber;
    private int itemCount;
    private float interval;
    private float current;
    private float lerpValue;
    private ScrollRect scroll;
    private bool isScroll;
    private bool isTouchUp;
    private bool isUpdata;
    public GameObject types;






    // 初始化
    void Start()
    {
        itemNumber = 1;
        itemCount = types.transform.childCount;
        interval = 1f / (itemCount - 1);
        scroll = GetComponent<ScrollRect>();
        scroll.horizontalNormalizedPosition = interval * itemNumber;
    }

    // 移動選單
    void Update()
    {
        if (isTouchUp)
        {
            itemNumber = (int)Mathf.Round(scroll.horizontalNormalizedPosition / interval);
            float startValue = scroll.horizontalNormalizedPosition;
            float endValue = interval * itemNumber;
            if (Mathf.Abs(scroll.velocity.x) < 130f)
            {
                if (lerpValue > 1)
                {
                    isTouchUp = false;
                    lerpValue = 0;
                    Debug.Log("UpdataList...");
                    return;
                }
                lerpValue += Time.deltaTime;
                scroll.horizontalNormalizedPosition = Mathf.Lerp(startValue, endValue, lerpValue / 1.5f);
            }
        }
    }

    public void TouchDown()
    {
        isTouchUp = false;
        lerpValue = 0;
    }

    public void TouchUp()
    {
        isTouchUp = true;
    }

}
