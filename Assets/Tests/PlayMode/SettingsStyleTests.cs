using System.Collections;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace NovaLux.Tests
{
    public class SettingsStyleTests : UiTestFixture
    {
        static readonly Color Rest = new Color(1f, 0f, 0f);
        static readonly Color Accent = new Color(0f, 0f, 1f);
        static readonly Color Pressed = new Color(0f, 0f, 0.5f);
        static readonly Color Text = new Color(0f, 1f, 0f);
        static readonly Color SelectedText = new Color(1f, 1f, 1f);
        static readonly Color PanelColor = new Color(0f, 1f, 1f);
        static readonly Color ScrimColor = new Color(0.1f, 0.1f, 0.1f, 0.5f);
        static readonly Color TrackColor = new Color(0.3f, 0.3f, 0f);
        static readonly Color HandleColor = new Color(1f, 1f, 0f);

        static readonly string[] SliderIds = { "Bgm", "Sfx", "Voice", "TextSpeed", "AutoSpeed" };
        static readonly string[] ChipButtons = { "Res720", "Res1080", "Res1440", "Res2160", "DisplayMode", "Close" };

        UiTheme _theme;
        Sprite _pill, _circle;
        Sprite[] _icons;
        Texture2D _texture;
        string _dir;

        public override void Setup()
        {
            base.Setup();
            _dir = Path.Combine(Path.GetTempPath(), "NovaLuxTests_" + System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_dir);
            Settings.FilePath = Path.Combine(_dir, "settings.json");

            _texture = new Texture2D(8, 8);
            _pill = NewSprite(new Vector4(3f, 3f, 3f, 3f));
            _circle = NewSprite(Vector4.zero);
            _icons = SliderIds.Select(_ => NewSprite(Vector4.zero)).ToArray();

            _theme = ScriptableObject.CreateInstance<UiTheme>();
            _theme.secondaryChipColor = Rest;
            _theme.accentColor = Accent;
            _theme.pressedColor = Pressed;
            _theme.textColor = Text;
            _theme.selectedTextColor = SelectedText;
            _theme.panelColor = PanelColor;
            _theme.scrimColor = ScrimColor;
            _theme.sliderTrackColor = TrackColor;
            _theme.handleColor = HandleColor;
            _theme.fontKo = Resources.Load<UiTheme>("UiTheme").fontKo;
            _theme.pillSprite = _pill;
            _theme.circleSprite = _circle;
            _theme.iconBgm = _icons[0];
            _theme.iconSfx = _icons[1];
            _theme.iconVoice = _icons[2];
            _theme.iconTextSpeed = _icons[3];
            _theme.iconAuto = _icons[4];
            _theme.sliderTrackHeight = 21f;
            _theme.sliderHandleSize = 43f;
            UiTheme.Current = _theme;
        }

        public override void TearDown()
        {
            UiTheme.Current = null;
            Object.Destroy(_theme);
            Object.Destroy(_texture);
            Settings.FilePath = null;
            if (Directory.Exists(_dir)) Directory.Delete(_dir, true);
            base.TearDown();
        }

        Sprite NewSprite(Vector4 border) =>
            Sprite.Create(_texture, new Rect(0, 0, 8, 8), Vector2.one * 0.5f, 100f, 0, SpriteMeshType.FullRect, border);

        IEnumerator OpenSettings()
        {
            yield return LoadTitle();
            yield return Click(FindButton("Settings"));
        }

        static Transform Panel => Object.FindObjectsByType<SettingsPanel>().Single().transform;

        static void AssertRoundPill(Image image, Sprite pill, string id)
        {
            Assert.AreSame(pill, image.sprite, id);
            Assert.AreEqual(Image.Type.Sliced, image.type, id);
            var height = image.rectTransform.rect.height;
            var scaledHeight = image.sprite.rect.height / (image.pixelsPerUnit * image.pixelsPerUnitMultiplier);
            Assert.AreEqual(height, scaledHeight, 1e-2f, id);
        }

        [Test]
        public void DefaultTheme_HasSettingsIcons()
        {
            var theme = Resources.Load<UiTheme>("UiTheme");

            Assert.IsNotNull(theme.iconBgm);
            Assert.IsNotNull(theme.iconSfx);
            Assert.IsNotNull(theme.iconVoice);
            Assert.IsNotNull(theme.iconTextSpeed);
            Assert.IsNotNull(theme.iconAuto);
        }

        static float Luminance(Color c)
        {
            static float Channel(float v) => v <= 0.03928f ? v / 12.92f : Mathf.Pow((v + 0.055f) / 1.055f, 2.4f);
            return 0.2126f * Channel(c.r) + 0.7152f * Channel(c.g) + 0.0722f * Channel(c.b);
        }

        static float Contrast(Color a, Color b)
        {
            float x = Luminance(a), y = Luminance(b);
            return (Mathf.Max(x, y) + 0.05f) / (Mathf.Min(x, y) + 0.05f);
        }

        // WCAG AA(일반 글자 4.5). 선택 버튼의 흰 글자와, 쉬는 칩·패널 위의 본문 글자를 함께 본다.
        [Test]
        public void DefaultTheme_TextContrast_MeetsWcagAa()
        {
            var theme = Resources.Load<UiTheme>("UiTheme");

            Assert.GreaterOrEqual(Contrast(theme.selectedTextColor, theme.accentColor), 4.5f, "accent");
            Assert.GreaterOrEqual(Contrast(theme.selectedTextColor, theme.pressedColor), 4.5f, "pressed");
            Assert.GreaterOrEqual(Contrast(theme.textColor, theme.panelColor), 4.5f, "panel");
            Assert.GreaterOrEqual(Contrast(theme.textColor, theme.secondaryChipColor), 4.5f, "chip");
            Assert.GreaterOrEqual(Contrast(theme.pressedColor, theme.secondaryChipColor), 4.5f, "focused chip text");
        }

        [UnityTest]
        public IEnumerator ChipButtons_UsePillSprite_Sliced_RoundCorners()
        {
            yield return OpenSettings();

            foreach (var id in ChipButtons) AssertRoundPill(Face(FindButton(id)), _pill, id);
        }

        [UnityTest]
        public IEnumerator CheckedResolution_FilledAccent_WhiteText_OthersSecondary()
        {
            yield return OpenSettings();

            var checkedChip = FindButton("Res1080");
            Assert.AreEqual(Accent, Face(checkedChip).color);
            Assert.AreEqual(SelectedText, LabelOf(checkedChip).color);
            var other = FindButton("Res720");
            Assert.AreEqual(Rest, Face(other).color);
            Assert.AreEqual(Text, LabelOf(other).color);
        }

        [UnityTest]
        public IEnumerator FocusedUncheckedChip_ShowsRing_NotFill()
        {
            yield return OpenSettings();
            var other = FindButton("Res720");

            yield return Hover(other);

            Assert.AreSame(other.gameObject, Selected);
            Assert.AreEqual(Rest, Face(other).color);
            Assert.AreEqual(Pressed, LabelOf(other).color);
            Assert.IsTrue(other.GetComponent<Outline>().enabled);
            Assert.IsFalse(FindButton("Res1080").GetComponent<Outline>().enabled);
        }

        [UnityTest]
        public IEnumerator FocusedCheckedChip_KeepsFill_AndShowsRing()
        {
            yield return OpenSettings();
            var chip = FindButton("Res1080");

            yield return Hover(chip);

            Assert.AreEqual(Accent, Face(chip).color);
            Assert.AreEqual(SelectedText, LabelOf(chip).color);
            Assert.IsTrue(chip.GetComponent<Outline>().enabled);
        }

        [UnityTest]
        public IEnumerator Close_FillsWithAccent_OnFocus_WithoutRing()
        {
            yield return OpenSettings();
            var close = FindButton("Close");
            Assert.AreEqual(Rest, Face(close).color);

            yield return Hover(close);

            Assert.AreEqual(Accent, Face(close).color);
            Assert.AreEqual(SelectedText, LabelOf(close).color);
            Assert.IsNull(close.GetComponent<Outline>());
        }

        [UnityTest]
        public IEnumerator Slider_TrackAndFillArePills_HandleIsThickerCircle()
        {
            yield return OpenSettings();
            var slider = FindSlider("Bgm");

            AssertRoundPill(slider.transform.Find("Background").GetComponent<Image>(), _pill, "track");
            AssertRoundPill(slider.fillRect.GetComponent<Image>(), _pill, "fill");
            var track = slider.transform.Find("Background").GetComponent<Image>();
            Assert.AreEqual(TrackColor, track.color);
            Assert.AreEqual(21f, track.rectTransform.rect.height, 1e-3f);
            var handle = slider.handleRect.GetComponent<Image>();
            Assert.AreSame(_circle, handle.sprite);
            Assert.AreEqual(HandleColor, handle.color);
            Assert.AreEqual(new Vector2(43f, 43f), handle.rectTransform.rect.size);
        }

        [UnityTest]
        public IEnumerator Slider_FillAtZero_StillAsWideAsTrackHeight()
        {
            yield return OpenSettings();
            var slider = FindSlider("Bgm");

            slider.value = 0f;
            yield return null;
            yield return null;

            Assert.GreaterOrEqual(slider.fillRect.rect.width, 21f - 1e-3f);
        }

        [UnityTest]
        public IEnumerator Panel_HasDimmedBackdrop_AndRoundedCard()
        {
            yield return OpenSettings();

            var scrim = Panel.GetComponent<Image>();
            Assert.AreEqual(ScrimColor, scrim.color);
            // 막은 기준 해상도(1920x1080) 영역만 덮어 16:9가 아닌 화면의 레터박스를 건드리지 않는다.
            Assert.AreEqual(new Vector2(1920f, 1080f), scrim.rectTransform.rect.size);
            var card = Panel.Find("Card").GetComponent<Image>();
            Assert.AreEqual(PanelColor, card.color);
            Assert.AreSame(_pill, card.sprite);
            Assert.AreEqual(Image.Type.Sliced, card.type);
        }

        [UnityTest]
        public IEnumerator EachSettingRow_HasIconChipFromTheme()
        {
            yield return OpenSettings();

            for (var i = 0; i < SliderIds.Length; i++)
            {
                var chip = Panel.Find("Chip" + SliderIds[i]).GetComponent<Image>();
                var icon = Panel.Find("Chip" + SliderIds[i] + "/Icon").GetComponent<Image>();
                Assert.AreSame(_circle, chip.sprite, SliderIds[i]);
                Assert.AreSame(_icons[i], icon.sprite, SliderIds[i]);
                Assert.IsFalse(chip.raycastTarget);
                Assert.IsFalse(icon.raycastTarget);
            }
        }
    }
}
