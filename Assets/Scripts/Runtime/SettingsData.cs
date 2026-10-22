using System;
using UnityEngine;

namespace NovaLux
{
    // settings.json에 그대로 직렬화된다. 필드 초기값이 기본값이며, 누락 키는 이 값으로 남는다.
    [Serializable]
    public class SettingsData
    {
        public float bgm = 0.8f;
        public float sfx = 0.8f;
        public float voice = 0.8f;
        public float textCps = 40f;
        public float autoDelaySec = 1.5f;
        public bool fullscreen = true;
        // ScreenSettings.Resolutions의 인덱스. 1 = 1080p.
        public int resolution = 1;
        public Language language = Language.Ko;

        public void Clamp()
        {
            resolution = Mathf.Clamp(resolution, 0, ScreenSettings.Resolutions.Length - 1);
            if (!Enum.IsDefined(typeof(Language), language)) language = Language.Ko;
            bgm = Mathf.Clamp01(bgm);
            sfx = Mathf.Clamp01(sfx);
            voice = Mathf.Clamp01(voice);
            textCps = Mathf.Clamp(textCps, 10f, 100f);
            autoDelaySec = Mathf.Clamp(autoDelaySec, 0.5f, 5f);
        }
    }
}
