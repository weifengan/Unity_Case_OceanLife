using UnityEngine;
using System.Collections;

namespace Cameo
{
    public class RectResizeHandler : MonoBehaviour
    {
        public RectTransform RectTrans;
        public Vector3 From;
        public Vector3 To;
        public float TotalTime;

        private float _curTime;

        public void Init(RectTransform rectTrans, Vector2 from, Vector2 to, float totalTime)
        {
            RectTrans = rectTrans;
            From = new Vector3(from.x, from.y);
            To = new Vector3(to.x, to.y);
            TotalTime = totalTime;
            _curTime = 0;

			if(TotalTime == 0)
			{
				RectTrans.localScale = To;
				Component.Destroy(this);
			}
        }

        void Update()
        {
            if (_curTime >= TotalTime)
            {
                RectTrans.localScale = To;
                Component.Destroy(this);
            }
            else
            {
                RectTrans.localScale = Vector3.Lerp(From, To, _curTime / TotalTime);
                _curTime += Time.deltaTime;
            }
        }
    }
}

