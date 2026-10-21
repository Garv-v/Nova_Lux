using System.Collections;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace NovaLux.Tests
{
    public class FontTests : UiTestFixture
    {
        string _dir;

        public override void Setup()
        {
            base.Setup();
            _dir = Path.Combine(Path.GetTempPath(), "NovaLuxTests_" + System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_dir);
            Settings.FilePath = Path.Combine(_dir, "settings.json");
            UiTheme.Current = null;
        }

        public override void TearDown()
        {
            Settings.FilePath = null;
            if (Directory.Exists(_dir)) Directory.Delete(_dir, true);
            base.TearDown();
        }

        // 서체 자신이나 fallback 중 하나라도 글리프를 가지면 보인다.
        static bool Covers(TMP_FontAsset asset, char c)
        {
            foreach (var a in new[] { asset }.Concat(asset.fallbackFontAssetTable))
            {
                FontEngine.LoadFontFace(a.sourceFontFile, 90);
                if (FontEngine.TryGetGlyphIndex(c, out _)) return true;
            }
            return false;
        }

        static string Missing(TMP_FontAsset asset, string text)
        {
            return new string(text.Where(c => !char.IsWhiteSpace(c) && !Covers(asset, c)).ToArray());
        }

        [UnityTest]
        public IEnumerator Screens_HaveNoLegacyText()
        {
            yield return LoadTitle();
            Assert.AreEqual(0, Object.FindObjectsByType<Text>(FindObjectsInactive.Include).Length);

            yield return Click(FindButton("Settings"));
            Assert.AreEqual(0, Object.FindObjectsByType<Text>(FindObjectsInactive.Include).Length);
            Assert.Greater(Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include).Length, 5);
        }

        [UnityTest]
        public IEnumerator DefaultTheme_PointsAtBothLanguageFonts()
        {
            yield return null;
            Assert.IsNotNull(UiTheme.Current.fontKo);
            Assert.IsNotNull(UiTheme.Current.fontJa);
        }

        [UnityTest]
        public IEnumerator Fonts_CoverKoreanScreensAndJapaneseSamples()
        {
            yield return LoadTitle();
            yield return Click(FindButton("Settings"));

            var shown = string.Concat(Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include).Select(t => t.text));
            Assert.AreEqual("", Missing(UiTheme.Current.fontKo, shown));

            // 일본어 모드의 언어 선택에는 "한국어"도 같이 나온다.
            Assert.AreEqual("", Missing(UiTheme.Current.fontJa, "ひらがなカタカナ日本語設定学国気新開始 한국어"));
            Assert.AreEqual("", Missing(UiTheme.Current.fontKo, "ひらがなカタカナ日本語設定学国気新開始"));
        }
    }
}
