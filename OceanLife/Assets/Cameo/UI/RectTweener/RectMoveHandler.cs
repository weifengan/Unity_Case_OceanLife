using UnityEngine;
using System.Collections;

namespace Cameo
{
    public class RectMoveHandler : MonoBehaviour
    {
        public RectTransform RectTrans;
        public Vector2 From;
        public Vector2 To;
        public float TotalTime;

        private float _curTime;

        public void Init(RectTransform rectTrans, Vector2 from, Vector2 to, float totalTime)
        {
            RectTrans = rectTrans;
            From = from;
            To = to;
            TotalTime = totalTime;
            _curTime = 0;
        }

        void Update()
        {
            if (_curTime > TotalTime)
            {
                RectTrans.anchoredPosition = To;
                Component.Destroy(this);
            }
            else
            {
                RectTrans.anchoredPosition = Vector2.Lerp(From, To, _curTime / TotalTime);
                _curTime += Time.deltaTime;
            }
        }
    }
}