using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TypeInfoController : MonoBehaviour
{
    public Canvas _canvas;
    public ScrollRect _textInfoScrollRect;
    public GameObject _textInfoPanel;
    public GameObject _textInfoPanelIndicator;
    public Button _textInfoPannelSwitch;
    private bool isTextInfoShow = false;
    private Vector2 _textInfoPanelHidePosition;
    private Vector2 _textInfoPanelShowPosition;

    // Use this for initialization
    void Start()
    {
        // Set Text Info Panel Init Position, End Position and ScrollRect Height
        float floatPanelShowAnchoredPositionY = Screen.height - 480 * _canvas.scaleFactor - 110 * _canvas.scaleFactor;
        float floatPanelHeight = floatPanelShowAnchoredPositionY - 20 * _canvas.scaleFactor;
        _textInfoPanelHidePosition = _textInfoPanel.GetComponent<RectTransform>().anchoredPosition;
        _textInfoPanelShowPosition = new Vector2(_textInfoPanel.GetComponent<RectTransform>().anchoredPosition.x, floatPanelShowAnchoredPositionY / _canvas.scaleFactor);
        _textInfoScrollRect.GetComponent<RectTransform>().sizeDelta = new Vector2(_textInfoScrollRect.GetComponent<RectTransform>().rect.width, floatPanelHeight / _canvas.scaleFactor);
    }

    public void OnTextInfoSwitchClick()
    {
        Hashtable ht = new Hashtable();
        ht.Add("from", _textInfoPanel.GetComponent<RectTransform>().anchoredPosition);
        ht.Add("time", 0.8f);
        ht.Add("easeType", iTween.EaseType.easeOutQuad);
        ht.Add("onupdatetarget", this.gameObject);
        ht.Add("onupdate", "SetTextInfoPanelAnchoredPosition");

        Hashtable indicatorRotateHt = new Hashtable();
        indicatorRotateHt.Add("time", 0.3);

        if (isTextInfoShow)
        {
            ht.Add("to", _textInfoPanelHidePosition);
            indicatorRotateHt.Add("z", 90);
            isTextInfoShow = false;
        }
        else
        {
            ht.Add("to", _textInfoPanelShowPosition);
            indicatorRotateHt.Add("z", 270);
            isTextInfoShow = true;
        }

        iTween.RotateTo(_textInfoPanelIndicator, indicatorRotateHt);
        iTween.ValueTo(_textInfoPanel, ht);
    }

    private void SetTextInfoPanelAnchoredPosition(Vector2 position)
    {
        _textInfoPanel.GetComponent<RectTransform>().anchoredPosition = position;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
