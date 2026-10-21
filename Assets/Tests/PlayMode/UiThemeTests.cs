using System.Collections;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace NovaLux.Tests
{
    public class UiThemeTests : UiTestFixture
    {
        static readonly Color ButtonColor = new Color(1f, 0f, 0f);
        static readonly Color AccentColor = new Color(0f, 0f, 1f);
        static readonly Color TextColor = new Color(0f, 1f, 0f);
        static readonly Color HandleColor = new Color(1f, 1f, 0f);
        static readonly Color PanelColor = new Color(0f, 1f, 1f);
        static readonly Color ChipColor = new Color(1f, 0f, 1f);
        static readonly Color TrackColor = new Color(0.5f, 0f, 0.5f);

        UiTheme _theme;
        string _dir;

        public override void Setup()
        {
            base.Setup();
            _dir = Path.Combine(Path.GetTempPath(), "NovaLuxTests_" + System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_dir);
            Settings.FilePath = Path.Combine(_dir, "settings.json");

            _theme = ScriptableObject.CreateInstance<UiTheme>();
            _theme.buttonColor = ButtonColor;
            _theme.accentColor = AccentColor;
            _theme.textColor = TextColor;
            _theme.handleColor = HandleColor;
            _theme.panelColor = PanelColor;
            _theme.secondaryChipColor = ChipColor;
            _theme.sliderTrackColor = TrackColor;
            _theme.fontSize = 33;
            _theme.smallFontSize = 29;
            _theme.fontKo = Resources.Load<UiTheme>("UiTheme").fontKo;
            _theme.titleButtonSize = new Vector2(401f, 77f);
            _theme.titleButtonGap = 13f;
            _theme.rowGap = 71f;
            _theme.firstRowY = 301f;
            _theme.controlX = 203f;
            _theme.sliderSize = new Vector2(411f, 37f);
            _theme.closeButtonSize = new Vector2(251f, 73f);
            UiTheme.Current = _theme;
        }

        public override void TearDown()
        {
            UiTheme.Current = null;
            Object.Destroy(_theme);
            Settings.FilePath = null;
            if (Directory.Exists(_dir)) Directory.Delete(_dir, true);
            base.TearDown();
        }

        [UnityTest]
        public IEnumerator TitleButtons_ReadTheme()
        {
            yield return LoadTitle();

            var button = FindButton("NewGame");
            Assert.AreEqual(ButtonColor, ((Image)button.targetGraphic).color);
            Assert.AreEqual(TextColor, LabelOf(button).color);
            Assert.AreEqual(33, LabelOf(button).fontSize);
        }

        [UnityTest]
        public IEnumerator Labels_UseThemeFont()
        {
            yield return LoadTitle();
            Assert.IsNotNull(_theme.fontKo);
            Assert.AreSame(_theme.fontKo, LabelOf(FindButton("NewGame")).font);

            yield return Click(FindButton("Settings"));
            var panel = Object.FindObjectsByType<SettingsPanel>().Single().transform;
            Assert.AreSame(_theme.fontKo, panel.Find("LabelBgm").GetComponent<TMP_Text>().font);
        }

        [UnityTest]
        public IEnumerator TitleButtons_ReadThemeSpacing()
        {
            yield return LoadTitle();

            var first = (RectTransform)FindButton("NewGame").transform;
            var second = (RectTransform)FindButton("Continue").transform;
            Assert.AreEqual(new Vector2(401f, 77f), first.sizeDelta);
            Assert.AreEqual(77f + 13f, first.anchoredPosition.y - second.anchoredPosition.y, 1e-3f);
        }

        [UnityTest]
        public IEnumerator SettingsPanel_ReadsThemeSpacing()
        {
            yield return LoadTitle();
            yield return Click(FindButton("Settings"));

            var bgm = (RectTransform)FindSlider("Bgm").transform;
            var sfx = (RectTransform)FindSlider("Sfx").transform;
            Assert.AreEqual(301f, bgm.anchoredPosition.y, 1e-3f);
            Assert.AreEqual(203f, bgm.anchoredPosition.x, 1e-3f);
            Assert.AreEqual(71f, bgm.anchoredPosition.y - sfx.anchoredPosition.y, 1e-3f);
            Assert.AreEqual(new Vector2(411f, 37f), bgm.sizeDelta);
            Assert.AreEqual(new Vector2(251f, 73f), ((RectTransform)FindButton("Close").transform).sizeDelta);
        }

        [UnityTest]
        public IEnumerator SettingsPanel_ReadsTheme()
        {
            yield return LoadTitle();
            yield return Click(FindButton("Settings"));

            var panel = Object.FindObjectsByType<SettingsPanel>().Single().transform;
            Assert.AreEqual(PanelColor, panel.Find("Card").GetComponent<Image>().color);

            var label = panel.Find("LabelBgm").GetComponent<TMP_Text>();
            Assert.AreEqual(33, label.fontSize);
            Assert.AreEqual(TextColor, label.color);

            var slider = panel.Find("Bgm");
            Assert.AreEqual(TrackColor, slider.Find("Background").GetComponent<Image>().color);
            Assert.AreEqual(AccentColor, slider.Find("Fill Area/Fill").GetComponent<Image>().color);
            Assert.AreEqual(HandleColor, slider.Find("Handle Slide Area/Handle").GetComponent<Image>().color);

            Assert.AreEqual(ChipColor, ((Image)FindButton("Close").targetGraphic).color);
            Assert.AreEqual(29, LabelOf(FindButton("DisplayMode")).fontSize);

            // 기본 해상도 인덱스 1(1080)이 선택 색, 나머지는 칩 색.
            Assert.AreEqual(AccentColor, ((Image)FindButton("Res1080").targetGraphic).color);
            Assert.AreEqual(ChipColor, ((Image)FindButton("Res720").targetGraphic).color);
        }
    }
}
