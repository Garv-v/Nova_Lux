using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace NovaLux
{
    // 화면을 검게 덮었다 걷는다. 검은 때에 콜백을 불러 그 사이에 화면을 바꾼다.
    // 씬마다 처음 쓸 때 만들고 씬과 함께 사라진다. sortingOrder는 레터박스(1000)보다 낮게 둔다.
    public class ScreenFade : MonoBehaviour
    {
        public const float Duration = 0.25f;

        static ScreenFade _instance;
        Image _veil;
        bool _busy;

        public static bool Busy => _instance != null && _instance._busy;
        public static float Alpha => _instance != null ? _instance._veil.color.a : 0f;

        public static void Run(Action atBlack)
        {
            if (Busy) return;
            if (_instance == null) _instance = Create();
            _instance.StartCoroutine(_instance.Play(atBlack));
        }

        static ScreenFade Create()
        {
            var go = new GameObject("ScreenFade", typeof(Canvas), typeof(GraphicRaycaster), typeof(ScreenFade));
            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 900;
            var fade = go.GetComponent<ScreenFade>();
            fade._veil = UiKit.NewPanel(go.transform, "Veil", new Color(0f, 0f, 0f, 0f));
            fade._veil.raycastTarget = false;
            return fade;
        }

        IEnumerator Play(Action atBlack)
        {
            _busy = true;
            _veil.raycastTarget = true;
            yield return Ramp(0f, 1f);
            try { atBlack?.Invoke(); }
            catch (Exception e) { Debug.LogException(e); }
            yield return Ramp(1f, 0f);
            _veil.raycastTarget = false;
            _busy = false;
        }

        IEnumerator Ramp(float from, float to)
        {
            for (var t = 0f; t < Duration; t += Time.deltaTime)
            {
                SetAlpha(Mathf.Lerp(from, to, t / Duration));
                yield return null;
            }
            SetAlpha(to);
        }

        void SetAlpha(float a) => _veil.color = new Color(0f, 0f, 0f, a);
    }
}
