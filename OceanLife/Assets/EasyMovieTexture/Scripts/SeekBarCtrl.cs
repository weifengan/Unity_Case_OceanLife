using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;



#if !UNITY_WEBGL

public class SeekBarCtrl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IPointerClickHandler
{

    public MediaPlayerCtrl m_srcVideo;
    public Slider m_srcSlider;
    public float m_fDragTime = 0.2f;
    public float fadeTime;


    bool m_bActiveDrag = true;
    bool m_bUpdate = true;
    bool isfadeUpdate = true;

    float m_fDeltaTime = 0.0f;
    float m_fLastValue = 0.0f;
    float m_fLastSetValue = 0.0f;

    // Use this for initialization
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {

        if (m_bActiveDrag == false)
        {
            m_fDeltaTime += Time.deltaTime;
            if (m_fDeltaTime > m_fDragTime)
            {
                m_bActiveDrag = true;
                m_fDeltaTime = 0.0f;
                //if(m_fLastSetValue != m_fLastValue)
                //	m_srcVideo.SetSeekBarValue (m_fLastValue);

            }
        }

        if (fadeTime > 2)
        {
            CanvasGroup fade = GetComponent<CanvasGroup>();
            Hashtable ht = new Hashtable();
            ht.Add("from", 1);
            ht.Add("to", 0);
            ht.Add("time", 1f);
            ht.Add("onupdate", (System.Action<object>)(floatAlpha => fade.alpha = (float)floatAlpha));
            ht.Add("oncomplete", "FadeEnd");
            iTween.ValueTo(gameObject, ht);
            fadeTime = 0;
        }

        if (m_bUpdate == false)
            return;

        fadeTime += Time.deltaTime;
        if (m_srcVideo != null)
        {
            if (m_srcSlider != null)
            {
                m_srcSlider.value = m_srcVideo.GetSeekBarValue();
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnDown!");
        iTween.Stop(gameObject);
        CanvasGroup fade = GetComponent<CanvasGroup>();
        fade.alpha = 1;
        fadeTime = 0;
        m_bUpdate = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("OnUp!");
        m_bUpdate = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        m_srcVideo.SetSeekBarValue(m_srcSlider.value);
    }


    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag:" + eventData);

        if (m_bActiveDrag == false)
        {
            m_fLastValue = m_srcSlider.value;
            return;
        }

        //m_srcVideo.SetSeekBarValue (m_srcSlider.value);
        m_fLastSetValue = m_srcSlider.value;
        m_bActiveDrag = false;
    }

    private void FadeEnd()
    {
        gameObject.SetActive(false);
    }
}
#endif