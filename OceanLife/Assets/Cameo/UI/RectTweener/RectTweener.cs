using UnityEngine;
using System.Collections;

namespace Cameo
{
    public class RectTweener
    {
        public static void Move(RectTransform rectTrans, Vector2 moveFrom, Vector2 moveTo, float totalTime)
        {
            RectMoveHandler handler = GetOrAddComponent<RectMoveHandler>(rectTrans.gameObject);
            handler.Init(rectTrans, moveFrom, moveTo, totalTime);
        }

        public static void MoveTo(RectTransform rectTrans, Vector2 moveTo, float totalTime)
        {
            Vector2 moveFrom = rectTrans.anchoredPosition;
            Move(rectTrans, moveFrom, moveTo, totalTime);
        }

        public static void Scale(RectTransform rectTrans, Vector2 scaleFrom, Vector2 scaleTo, float totalTime)
        {
            RectResizeHandler handler = GetOrAddComponent<RectResizeHandler>(rectTrans.gameObject);
            handler.Init(rectTrans, scaleFrom, scaleTo, totalTime);
        }

        public static void ScaleTo(RectTransform rectTrans, Vector2 scaleTo, float totalTime)
        {
            Vector2 scaleFrom = rectTrans.localScale;
            Scale(rectTrans, scaleFrom, scaleTo, totalTime);
        }

        public static void ImageFade(UnityEngine.UI.Image img, Color fadeFrom, Color fadeTo, float totalTime)
        {
            ImageFadeHandler handler = GetOrAddComponent<ImageFadeHandler>(img.gameObject);
            handler.Init(img, fadeFrom, fadeTo, totalTime);
        }

        public static void ImageFadeTo(UnityEngine.UI.Image img, Color fadeTo, float totalTime)
        {
            Color fadeFrom = img.color;
            ImageFade(img, fadeFrom, fadeTo, totalTime);
        }

        public static void CanvasGroupFade(CanvasGroup canvasGroup, float fadeFrom, float fadeTo, float totalTime)
        {
            CanvasFadeHandler handler = GetOrAddComponent<CanvasFadeHandler>(canvasGroup.gameObject);
            handler.Init(canvasGroup, fadeFrom, fadeTo, totalTime);
        }

        public static void CanvasGroupFadeTo(CanvasGroup canvasGroup, float fadeTo, float totalTime)
        {
            float fadeFrom = canvasGroup.alpha;
            CanvasGroupFade(canvasGroup, fadeFrom, fadeTo, totalTime);
        }

        public static void Rotate(RectTransform rectTrans, Vector3 rotFrom, Vector3 rotTo, float totalTime)
        {
            RectRotateHandler handler = GetOrAddComponent<RectRotateHandler>(rectTrans.gameObject);
            handler.Init(rectTrans, rotFrom, rotTo, totalTime);
        }

        public static void RotateTo(RectTransform rectTrans, Vector3 rotTo, float totalTime)
        {
            Vector3 rotFrom = rectTrans.localEulerAngles;
            Rotate(rectTrans, rotFrom, rotTo, totalTime);
        }

        private static T GetOrAddComponent<T>(GameObject gameObj) where T : Component
        {
            T comp = gameObj.GetComponent<T>();

            if (comp == null)
            {
                comp = gameObj.AddComponent<T>();
            }

            return comp;
        }
    }
}

