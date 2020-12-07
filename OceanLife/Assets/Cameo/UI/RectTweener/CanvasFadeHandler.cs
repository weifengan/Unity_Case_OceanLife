using UnityEngine;
using System.Collections;

namespace Cameo
{
    public class CanvasFadeHandler : MonoBehaviour
    {
        public CanvasGroup Group;
        public float From;
        public float To;
        public float TotalTime;

        private float _curTime;

        public void Init(CanvasGroup canvasGroup, float fadeFrom, float fadeTo, float totalTime)
        {
            Group = canvasGroup;
            From = fadeFrom;
            To = fadeTo;
            TotalTime = totalTime;
            _curTime = 0;
        }

        void Update()
        {
            if (_curTime >= TotalTime)
            {
                Group.alpha = To;
                Component.Destroy(this);
            }
            else
            {
                Group.alpha = From + (To - From) * _curTime / TotalTime;
                _curTime += Time.deltaTime;
            }
        }
    }
}