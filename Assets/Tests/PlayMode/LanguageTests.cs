using System.Collections;
using System.Linq;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

namespace NovaLux.Tests
{
    public class LanguageTests : UiTestFixture
    {
        public override void Setup()
        {
            base.Setup();
            UseTempSettingsFile();
            UiTheme.Current = null;
        }

        static string TextOf(string buttonId) => LabelOf(FindButton(buttonId)).text;

        [Test]
        public void DefaultLanguage_IsKorean()
        {
            Assert.AreEqual(Language.Ko, Settings.Current.language);
            Assert.AreEqual("새로 시작", Loc.Get("title.newGame"));
        }

        [Test]
        public void EveryKey_HasNonEmptyTextInEveryLanguage()
        {
            foreach (var language in new[] { Language.Ko, Language.Ja })
            {
                Settings.Set(d => d.language = language);
                foreach (var key in Loc.Keys) Assert.IsNotEmpty(Loc.Get(key), $"{language} {key}");
            }
        }

        [Test]
        public void Reload_InvalidLanguage_FallsBackToKorean()
        {
            System.IO.File.WriteAllText(Settings.FilePath, "{\"language\":9}");
            Settings.Reload();
            Assert.AreEqual(Language.Ko, Settings.Current.language);
        }

        [UnityTest]
        public IEnumerator ChoosingJapanese_ChangesTitleAndSettingsAtOnce_AndSaves()
        {
            yield return LoadTitle();
            Assert.AreEqual("새로 시작", TextOf("NewGame"));
            yield return Click(FindButton("Settings"));
            yield return Click(FindButton("LangJa"));

            Assert.AreEqual("はじめから", TextOf("NewGame"));
            Assert.AreEqual("設定", TextOf("Settings"));
            Assert.AreEqual("閉じる", TextOf("Close"));
            Assert.AreEqual("フルスクリーン", TextOf("DisplayMode"));
            Assert.AreEqual("言語", GameObject.Find("LabelLanguage").GetComponent<TMP_Text>().text);

            Settings.Reload();
            Assert.AreEqual(Language.Ja, Settings.Current.language);
        }

        [UnityTest]
        public IEnumerator Switching_UsesThatLanguagesFont_AndBackToKorean()
        {
            yield return LoadTitle();
            yield return Click(FindButton("Settings"));
            yield return Click(FindButton("LangJa"));
            Assert.AreSame(UiTheme.Current.fontJa, LabelOf(FindButton("NewGame")).font);

            yield return Click(FindButton("LangKo"));
            Assert.AreEqual("새로 시작", TextOf("NewGame"));
            Assert.AreSame(UiTheme.Current.fontKo, LabelOf(FindButton("NewGame")).font);
        }

        [UnityTest]
        public IEnumerator SavedJapanese_ShowsOnNextLaunch()
        {
            Settings.Set(d => d.language = Language.Ja);
            yield return LoadTitle();
            Assert.AreEqual("はじめから", TextOf("NewGame"));
            yield return Click(FindButton("Settings"));
            Assert.IsTrue(((PillButton)FindButton("LangJa")).IsOn);
        }

        [UnityTest]
        public IEnumerator ToggleStateButton_StaysInChosenLanguage_AfterToggle()
        {
            Settings.Set(d => d.language = Language.Ja);
            yield return LoadTitle();
            yield return Click(FindButton("Settings"));
            yield return Click(FindButton("DisplayMode"));
            Assert.AreEqual("ウィンドウ", TextOf("DisplayMode"));
        }
    }
}
