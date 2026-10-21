using UnityEngine;
using UnityEngine.UI;

namespace NovaLux
{
    // 화면이 16:9가 아닐 때 월드는 카메라 rect로, UI는 맨 위 검은 띠로 가린다.
    // 다른 Overlay 캔버스는 sortingOrder를 이보다 낮게 둔다.
    public class Letterbox : MonoBehaviour
    {
        const float Aspect = 16f / 9f;
        static Letterbox _instance;
        static RectTransform _left, _right, _bottom, _top;

        public static Rect Fit(int w, int h)
        {
            var screen = (float)w / h;
            if (screen > Aspect)
            {
                var width = Aspect / screen;
                return new Rect((1f - width) / 2f, 0f, width, 1f);
            }
            var height = screen / Aspect;
            return new Rect(0f, (1f - height) / 2f, 1f, height);
        }

        public static void Create()
        {
            if (_instance != null) return;

            var go = new GameObject("Letterbox", typeof(Canvas), typeof(Letterbox));
            DontDestroyOnLoad(go);
            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000;
            _left = NewBar(go.transform, "BarLeft");
            _right = NewBar(go.transform, "BarRight");
            _bottom = NewBar(go.transform, "BarBottom");
            _top = NewBar(go.transform, "BarTop");
            _instance = go.GetComponent<Letterbox>();
        }

        public static void Layout(int w, int h)
        {
            if (_instance == null) return;

            var r = Fit(w, h);
            foreach (var camera in Camera.allCameras) camera.rect = r;

            Anchor(_left, 0f, 0f, r.xMin, 1f);
            Anchor(_right, r.xMax, 0f, 1f, 1f);
            Anchor(_bottom, 0f, 0f, 1f, r.yMin);
            Anchor(_top, 0f, r.yMax, 1f, 1f);
        }

        void LateUpdate() => Layout(Screen.width, Screen.height);

        static RectTransform NewBar(Transform parent, string name)
        {
            var image = new GameObject(name, typeof(RectTransform), typeof(Image)).GetComponent<Image>();
            image.transform.SetParent(parent, false);
            image.color = Color.black;
            var rt = image.rectTransform;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            return rt;
        }

        static void Anchor(RectTransform rt, float x0, float y0, float x1, float y1)
        {
            rt.anchorMin = new Vector2(x0, y0);
            rt.anchorMax = new Vector2(x1, y1);
        }
    }
}
