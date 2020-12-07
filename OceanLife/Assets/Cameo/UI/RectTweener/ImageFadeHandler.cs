using UnityEngine;
using System.Collections;

namespace Cameo
{
    public class ImageFadeHandler : MonoBehaviour
    {
        public UnityEngine.UI.Image Img;
        public Color From;
        public Color To;
        public float TotalTime;

        private float _curTime;

        public void Init(UnityEngine.UI.Image img, Color from, Color to, float totalTime)
        {
            Img = img;
            From = from;
            To = to;
            TotalTime = totalTime;
            _curTime = 0;

            if (totalTime == 0)
            {
                Img.color = To;
                Component.Destroy(this);
            }
        }

        void Update()
        {
            if (_curTime >= TotalTime)
            {
                Img.color = To;
                Component.Destroy(this);
            }
            else
            {
                Img.color = Color.Lerp(From, To, _curTime / TotalTime);
                _curTime += Time.deltaTime;
            }
        }
    }
}