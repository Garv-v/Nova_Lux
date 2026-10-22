using System;
using System.Collections.Generic;
using UnityEngine;

namespace NovaLux
{
    // 설정 파일에는 정수로 저장되므로 순서를 바꾸지 않는다.
    public enum Language { Ko = 0, Ja = 1 }

    // 자체 문자열 표. 키마다 {한국어, 일본어} 순서. 새 화면의 문자열은 여기에 키를 더한다.
    // 현재 언어는 Settings가 보관하고, Set으로 바꾸면 저장하고 Changed를 알린다.
    public static class Loc
    {
        static readonly Dictionary<string, string[]> Table = new()
        {
            ["title.newGame"] = new[] { "새로 시작", "はじめから" },
            ["title.continue"] = new[] { "이어하기", "つづきから" },
            ["title.gallery"] = new[] { "갤러리", "ギャラリー" },
            ["title.settings"] = new[] { "환경설정", "設定" },
            ["title.quit"] = new[] { "종료", "終了" },

            ["settings.bgm"] = new[] { "BGM", "BGM" },
            ["settings.sfx"] = new[] { "효과음", "効果音" },
            ["settings.voice"] = new[] { "보이스", "ボイス" },
            ["settings.textSpeed"] = new[] { "텍스트 속도", "テキスト速度" },
            ["settings.autoSpeed"] = new[] { "자동 진행", "オート速度" },
            ["settings.displayMode"] = new[] { "화면 모드", "画面モード" },
            ["settings.fullscreen"] = new[] { "전체화면", "フルスクリーン" },
            ["settings.windowed"] = new[] { "창모드", "ウィンドウ" },
            ["settings.resolution"] = new[] { "해상도", "解像度" },
            ["settings.language"] = new[] { "언어", "言語" },
            ["settings.close"] = new[] { "닫기", "閉じる" },
        };

        // 언어 선택 칩은 어느 언어 모드에서도 자기 언어 이름으로 보인다.
        public static readonly string[] LanguageNames = { "한국어", "日本語" };

        public static IEnumerable<string> Keys => Table.Keys;

        public static event Action Changed;

        public static Language Current => Settings.Current.language;

        public static string Get(string key)
        {
            if (!Table.TryGetValue(key, out var values))
            {
                Debug.LogWarning($"문자열 표에 없는 키: {key}");
                return key;
            }
            return values[(int)Current];
        }

        public static void Set(Language language)
        {
            if (language == Current) return;
            Settings.Set(d => d.language = language);
            Changed?.Invoke();
        }
    }
}
