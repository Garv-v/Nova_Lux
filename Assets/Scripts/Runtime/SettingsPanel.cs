using System;
using UnityEngine;
using UnityEngine.UI;

namespace NovaLux
{
    // 임시 박스 UI. 값을 바꾸면 즉시 Settings에 저장한다.
    public class SettingsPanel : MonoBehaviour
    {
        public static SettingsPanel Create(Transform canvas)
        {
            var panel = UiKit.NewPanel(canvas, "SettingsPanel");
            return panel.gameObject.AddComponent<SettingsPanel>();
        }

        void Awake()
        {
            var s = Settings.Current;
            AddRow(0, "Bgm", "BGM", 0f, 1f, false, s.bgm, (d, v) => d.bgm = v);
            AddRow(1, "Sfx", "효과음", 0f, 1f, false, s.sfx, (d, v) => d.sfx = v);
            AddRow(2, "Voice", "보이스", 0f, 1f, false, s.voice, (d, v) => d.voice = v);
            AddRow(3, "TextSpeed", "텍스트 속도", 10f, 100f, true, s.textCps, (d, v) => d.textCps = v);
            AddRow(4, "AutoSpeed", "자동 진행", 0.5f, 5f, false, s.autoDelaySec, (d, v) => d.autoDelaySec = v);
            AddDisplayRows(s);
            AddCloseButton();
        }

        void AddDisplayRows(SettingsData s)
        {
            var theme = UiTheme.Current;
            var modeY = RowY(5);
            AddLabel("DisplayMode", "화면 모드", modeY);
            var mode = AddButton("DisplayMode", ModeLabel(s.fullscreen), new Vector2(theme.controlX, modeY),
                theme.modeButtonSize);
            var modeText = mode.GetComponentInChildren<Text>();
            mode.onClick.AddListener(() =>
            {
                Settings.Set(d => d.fullscreen = !d.fullscreen);
                modeText.text = ModeLabel(Settings.Current.fullscreen);
                ScreenSettings.Apply(Settings.Current);
            });

            var resY = RowY(6);
            AddLabel("Resolution", "해상도", resY);
            var buttons = new Button[ScreenSettings.Resolutions.Length];
            for (var i = 0; i < buttons.Length; i++)
            {
                var index = i;
                var x = theme.resFirstX + i * theme.resStep;
                buttons[i] = AddButton("Res" + ScreenSettings.Resolutions[i].y, ScreenSettings.ResolutionNames[i],
                    new Vector2(x, resY), theme.resButtonSize);
                buttons[i].onClick.AddListener(() =>
                {
                    Settings.Set(d => d.resolution = index);
                    Highlight(buttons, index);
                    ScreenSettings.Apply(Settings.Current);
                });
            }
            Highlight(buttons, s.resolution);
        }

        static float RowY(int row) => UiTheme.Current.firstRowY - row * UiTheme.Current.rowGap;

        static string ModeLabel(bool fullscreen) => fullscreen ? "전체화면" : "창모드";

        static void Highlight(Button[] buttons, int selected)
        {
            var theme = UiTheme.Current;
            for (var i = 0; i < buttons.Length; i++)
                ((Image)buttons[i].targetGraphic).color = i == selected ? theme.accentColor : theme.buttonColor;
        }

        void AddLabel(string id, string label, float y)
        {
            var theme = UiTheme.Current;
            var text = UiKit.NewLabel(transform, "Label" + id, label, theme.fontSize, TextAnchor.MiddleRight);
            UiKit.Place(text.rectTransform, new Vector2(theme.labelX, y), theme.labelSize);
        }

        Button AddButton(string id, string label, Vector2 position, Vector2 size) =>
            UiKit.NewButton(transform, id, label, position, size, UiTheme.Current.smallFontSize);

        void AddRow(int row, string id, string label, float min, float max, bool whole, float value,
            Action<SettingsData, float> apply)
        {
            var y = RowY(row);
            AddLabel(id, label, y);

            var slider = UiKit.NewSlider(transform, id, min, max, whole, value);
            var theme = UiTheme.Current;
            UiKit.Place((RectTransform)slider.transform, new Vector2(theme.controlX, y), theme.sliderSize);
            slider.onValueChanged.AddListener(v => Settings.Set(d => apply(d, v)));
        }

        void AddCloseButton()
        {
            var theme = UiTheme.Current;
            var button = UiKit.NewButton(transform, "Close", "닫기", new Vector2(0f, RowY(7) + theme.closeOffsetY),
                theme.closeButtonSize, theme.fontSize);
            button.onClick.AddListener(() => Destroy(gameObject));
        }
    }
}
