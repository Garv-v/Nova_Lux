using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace NovaLux.Tests
{
    public class TitleStyleTests : UiTestFixture
    {
        static readonly Color Normal = new Color(1f, 0f, 0f);
        static readonly Color Accent = new Color(0f, 0f, 1f);
        static readonly Color Pressed = new Color(0f, 0f, 0.5f);
        static readonly Color Text = new Color(0f, 1f, 0f);
        static readonly Color SelectedText = new Color(1f, 1f, 1f);
        static readonly Color Disabled = new Color(0.5f, 0.5f, 0.5f);
        static readonly Color Chip = new Color(0.1f, 0.2f, 0.3f);
        static readonly Color SelectedChip = new Color(0.4f, 0.5f, 0.6f);
        static readonly Color DisabledChip = new Color(0.7f, 0.8f, 0.9f);
        static readonly Color IconColor = new Color(0.9f, 0.1f, 0.2f);

        static readonly string[] Ids = { "NewGame", "Continue", "Gallery", "Settings", "Quit" };

        UiTheme _theme;
        Sprite _pill, _circle;
        Sprite[] _icons;
        Texture2D _texture;

        public override void Setup()
        {
            base.Setup();
            UseTempSettingsFile();
            _texture = new Texture2D(8, 8);
            _pill = NewSprite(new Vector4(3f, 3f, 3f, 3f));
            _circle = NewSprite(Vector4.zero);
            _icons = Ids.Select(_ => NewSprite(Vector4.zero)).ToArray();

            _theme = ScriptableObject.CreateInstance<UiTheme>();
            _theme.buttonColor = Normal;
            _theme.accentColor = Accent;
            _theme.pressedColor = Pressed;
            _theme.textColor = Text;
            _theme.selectedTextColor = SelectedText;
            _theme.disabledColor = Disabled;
            _theme.chipColor = Chip;
            _theme.selectedChipColor = SelectedChip;
            _theme.disabledChipColor = DisabledChip;
            _theme.iconColor = IconColor;
            _theme.fontKo = Resources.Load<UiTheme>("UiTheme").fontKo;
            _theme.titleButtonSize = new Vector2(401f, 77f);
            _theme.pillSprite = _pill;
            _theme.circleSprite = _circle;
            _theme.iconNewGame = _icons[0];
            _theme.iconContinue = _icons[1];
            _theme.iconGallery = _icons[2];
            _theme.iconSettings = _icons[3];
            _theme.iconQuit = _icons[4];
            UiTheme.Current = _theme;
        }

        public override void TearDown()
        {
            UiTheme.Current = null;
            Object.Destroy(_theme);
            Object.Destroy(_texture);
            base.TearDown();
        }

        Sprite NewSprite(Vector4 border) =>
            Sprite.Create(_texture, new Rect(0, 0, 8, 8), Vector2.one * 0.5f, 100f, 0, SpriteMeshType.FullRect, border);

        static Image Find(Button button, string path) => button.transform.Find(path).GetComponent<Image>();

        [UnityTest]
        public IEnumerator TitleButtons_UseThemePillSprite_Sliced_WithRoundCorners()
        {
            yield return LoadTitle();

            foreach (var id in Ids)
            {
                var button = FindButton(id);
                var face = Face(button);
                Assert.AreSame(_pill, face.sprite, id);
                Assert.AreEqual(Image.Type.Sliced, face.type, id);
                // 스프라이트 높이가 버튼 높이에 맞게 늘어, 모서리 반지름이 가로·세로 같은 비율로 줄어든다.
                var rect = ((RectTransform)button.transform).rect;
                var scaledHeight = face.sprite.rect.height / (face.pixelsPerUnit * face.pixelsPerUnitMultiplier);
                Assert.AreEqual(rect.height, scaledHeight, 1e-2f, id);
            }
        }

        [UnityTest]
        public IEnumerator TitleButtons_HaveIconChipFromTheme()
        {
            yield return LoadTitle();

            for (var i = 0; i < Ids.Length; i++)
            {
                var button = FindButton(Ids[i]);
                var chip = Find(button, "Chip");
                var icon = Find(button, "Chip/Icon");
                Assert.AreSame(_circle, chip.sprite, Ids[i]);
                Assert.AreSame(_icons[i], icon.sprite, Ids[i]);
                Assert.IsFalse(chip.raycastTarget);
                Assert.IsFalse(icon.raycastTarget);
                Assert.IsNotNull(LabelOf(button), Ids[i]);
            }
        }

        [UnityTest]
        public IEnumerator Rest_UsesNormalColors()
        {
            yield return LoadTitle();

            var button = FindButton("Gallery");
            Assert.AreEqual(Normal, Face(button).color);
            Assert.AreEqual(Text, LabelOf(button).color);
            Assert.AreEqual(Chip, Find(button, "Chip").color);
            Assert.AreEqual(IconColor, Find(button, "Chip/Icon").color);
        }

        [UnityTest]
        public IEnumerator Hover_SelectsAndFillsWithAccent_ThenReleasesWhenAnotherIsHovered()
        {
            yield return LoadTitle();
            var gallery = FindButton("Gallery");

            yield return Hover(gallery);
            Assert.AreSame(gallery.gameObject, Selected);
            Assert.AreEqual(Accent, Face(gallery).color);
            Assert.AreEqual(SelectedText, LabelOf(gallery).color);
            Assert.AreEqual(SelectedChip, Find(gallery, "Chip").color);
            Assert.AreEqual(Accent, Find(gallery, "Chip/Icon").color);

            yield return Hover(FindButton("Settings"));
            Assert.AreEqual(Normal, Face(gallery).color);
            Assert.AreEqual(Text, LabelOf(gallery).color);
            Assert.AreEqual(Accent, Face(FindButton("Settings")).color);
        }

        [UnityTest]
        public IEnumerator Press_UsesPressedColor()
        {
            yield return LoadTitle();
            var gallery = FindButton("Gallery");

            yield return Hover(gallery);
            yield return HoldMouse();
            Assert.AreEqual(Pressed, Face(gallery).color);
            Assert.AreEqual(SelectedText, LabelOf(gallery).color);

            yield return ReleaseMouse();
        }

        [UnityTest]
        public IEnumerator Continue_Disabled_UsesDisabledColors()
        {
            yield return LoadTitle();

            var button = FindButton("Continue");
            button.interactable = false;
            yield return null;

            Assert.AreEqual(Normal, Face(button).color);
            Assert.AreEqual(Disabled, LabelOf(button).color);
            Assert.AreEqual(DisabledChip, Find(button, "Chip").color);
            Assert.AreEqual(Disabled, Find(button, "Chip/Icon").color);
        }

        [UnityTest]
        public IEnumerator Keyboard_Selection_FillsWithAccent()
        {
            yield return LoadTitle();

            yield return Tap(Keyboard.downArrowKey);

            var button = FindButton("NewGame");
            Assert.AreEqual(Accent, Face(button).color);
            Assert.AreEqual(SelectedText, LabelOf(button).color);
        }

        [Test]
        public void DefaultTheme_HasUiSprites()
        {
            var theme = Resources.Load<UiTheme>("UiTheme");
            Assert.IsNotNull(theme.pillSprite);
            Assert.IsNotNull(theme.circleSprite);
            Assert.IsNotNull(theme.iconNewGame);
            Assert.IsNotNull(theme.iconContinue);
            Assert.IsNotNull(theme.iconGallery);
            Assert.IsNotNull(theme.iconSettings);
            Assert.IsNotNull(theme.iconQuit);
            var border = theme.pillSprite.border;
            Assert.Greater(border.x, 0f);
            Assert.Greater(border.y, 0f);
            Assert.Greater(border.z, 0f);
            Assert.Greater(border.w, 0f);
        }
    }
}
