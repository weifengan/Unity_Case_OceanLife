using UnityEngine;
using System.Collections;

namespace Cameo
{
    public class RectRotateHandler : MonoBehaviour
    {
        RectTransform RectTrans;
        public Vector3 From;
        public Vector3 To;
        public float TotalTime;

        private float _curTime;

        public void Init(RectTransform rectTrans, Vector3 from, Vector3 to, float totalTime)
        {
            RectTrans = rectTrans;
            From = from;
            To = to;
            TotalTime = totalTime;
            _curTime = 0;

			if(TotalTime == 0)
			{
				RectTrans.localEulerAngles = To;
				Component.Destroy(this);
			}
        }

        void Update()
        {
            if (_curTime >= TotalTime)
            {
                RectTrans.localEulerAngles = To;
                Component.Destroy(this);
            }
            else
            {
                RectTrans.localEulerAngles = Vector3.Lerp(From, To, _curTime / TotalTime);
                _curTime += Time.deltaTime;
            }
        }
    }
}