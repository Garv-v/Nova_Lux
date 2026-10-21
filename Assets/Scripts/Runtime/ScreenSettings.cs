using System;
using UnityEngine;

namespace NovaLux
{
    public static class ScreenSettings
    {
        public static readonly Vector2Int[] Resolutions =
        {
            new Vector2Int(1280, 720),
            new Vector2Int(1920, 1080),
            new Vector2Int(2560, 1440),
            new Vector2Int(3840, 2160),
        };

        // 테스트가 화면 변경 호출을 가로채는 자리. null이면 실제로 적용한다.
        public static Action<int, int, FullScreenMode> Handler;

        public static void Apply(SettingsData d)
        {
            var r = Resolutions[d.resolution];
            var mode = d.fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
            if (Handler != null) Handler(r.x, r.y, mode);
            else Screen.SetResolution(r.x, r.y, mode);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Boot()
        {
            Apply(Settings.Current);
            Letterbox.Create();
        }
    }
}
