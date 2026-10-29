using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NovaLux
{
    // 둥근 카드 위의 환경설정. 값을 바꾸면 즉시 Settings에 저장한다.
    // 키보드·패드는 패널 안에서만 움직이도록 이동 경로를 직접 이어 둔다(뒤에 가려진 초기화면 버튼으로 새지 않게).
    public class SettingsPanel : ModalPanel
    {
        readonly PillSlider[] _sliders = new PillSlider[5];
        PillButton _mode;
        PillButton[] _resolutions;
        PillButton[] _languages;
        PillButton _close;

        public static SettingsPanel Create(Transform canvas, Action onClosed) => Open<SettingsPanel>(canvas, onClosed);

        // 선택된 것이 없을 때 방향 입력이 오면 첫 슬라이더부터 고른다. 열자마자 고르면 마우스로 열어도 채워져 보인다.
        protected override GameObject FirstSelection => _sliders[0].gameObject;

        protected override void Awake()
        {
            base.Awake();
            var theme = UiTheme.Current;
            UiKit.NewCard(transform, "Card", new Vector2(0f, theme.cardOffsetY), theme.cardSize);

            var s = Settings.Current;
            _sliders[0] = AddRow(0, "Bgm", "settings.bgm", theme.iconBgm, 0f, 1f, false, s.bgm, 0.05f, (d, v) => d.bgm = v);
            _sliders[1] = AddRow(1, "Sfx", "settings.sfx", theme.iconSfx, 0f, 1f, false, s.sfx, 0.05f, (d, v) => d.sfx = v);
            _sliders[2] = AddRow(2, "Voice", "settings.voice", theme.iconVoice, 0f, 1f, false, s.voice, 0.05f, (d, v) => d.voice = v);
            _sliders[3] = AddRow(3, "TextSpeed", "settings.textSpeed", theme.iconTextSpeed, 10f, 100f, true, s.textCps, 5f,
                (d, v) => d.textCps = v);
            _sliders[4] = AddRow(4, "AutoSpeed", "settings.autoSpeed", theme.iconAuto, 0.5f, 5f, false, s.autoDelaySec, 0.25f,
                (d, v) => d.autoDelaySec = v);
            AddDisplayRows(s);
            AddCloseButton();
            LinkNavigation();
        }

        // 놓지 못한 채 닫히거나 씬이 바뀌어도 마지막 값이 남게 한다.
        void OnDestroy() => Settings.SaveIfDirty();

        void AddDisplayRows(SettingsData s)
        {
            var theme = UiTheme.Current;
            var modeY = RowY(5);
            AddLabel("DisplayMode", "settings.displayMode", modeY);
            _mode = AddChipButton("DisplayMode", Loc.Get(ModeKey(s.fullscreen)), new Vector2(theme.controlX, modeY),
                theme.modeButtonSize, false);
            var modeText = _mode.GetComponentInChildren<LocText>();
            modeText.Key = ModeKey(s.fullscreen);
            _mode.onClick.AddListener(() =>
            {
                Settings.Set(d => d.fullscreen = !d.fullscreen);
                modeText.Key = ModeKey(Settings.Current.fullscreen);
                ScreenSettings.Apply(Settings.Current);
            });

            var resY = RowY(6);
            AddLabel("Resolution", "settings.resolution", resY);
            _resolutions = new PillButton[ScreenSettings.Resolutions.Length];
            for (var i = 0; i < _resolutions.Length; i++)
            {
                var index = i;
                var x = theme.resFirstX + i * theme.resStep;
                _resolutions[i] = AddChipButton("Res" + ScreenSettings.Resolutions[i].y, ScreenSettings.ResolutionNames[i],
                    new Vector2(x, resY), theme.resButtonSize, true);
                _resolutions[i].onClick.AddListener(() =>
                {
                    Settings.Set(d => d.resolution = index);
                    ShowChecked(index);
                    ScreenSettings.Apply(Settings.Current);
                });
            }
            ShowChecked(s.resolution);

            var langY = RowY(7);
            AddLabel("Language", "settings.language", langY);
            _languages = new PillButton[Loc.LanguageNames.Length];
            for (var i = 0; i < _languages.Length; i++)
            {
                var language = (Language)i;
                var x = theme.langFirstX + i * theme.langStep;
                _languages[i] = AddChipButton("Lang" + language, Loc.LanguageNames[i], new Vector2(x, langY),
                    theme.langButtonSize, true);
                _languages[i].onClick.AddListener(() =>
                {
                    Loc.Set(language);
                    ShowLanguage();
                });
            }
            ShowLanguage();
        }

        void ShowLanguage()
        {
            for (var i = 0; i < _languages.Length; i++) _languages[i].IsOn = i == (int)Loc.Current;
            LinkNavigation();
        }

        static float RowY(int row) => UiTheme.Current.firstRowY - row * UiTheme.Current.rowGap;

        static string ModeKey(bool fullscreen) => fullscreen ? "settings.fullscreen" : "settings.windowed";

        void ShowChecked(int selected)
        {
            for (var i = 0; i < _resolutions.Length; i++) _resolutions[i].IsOn = i == selected;
            LinkNavigation();
        }

        // 위아래는 항목 순서대로, 해상도·언어 줄은 좌우로 잇는다. 슬라이더의 좌우는 비워 둬야 값 조정으로 쓰인다.
        // 해상도·언어 줄로 들어갈 때는 현재 선택된 칩으로 간다.
        void LinkNavigation()
        {
            if (_close == null || _languages == null || _languages[_languages.Length - 1] == null) return;
            var current = _resolutions[Settings.Current.resolution];
            for (var i = 0; i < _sliders.Length; i++)
                Link(_sliders[i], i > 0 ? _sliders[i - 1] : null, i < _sliders.Length - 1 ? _sliders[i + 1] : _mode);
            Link(_mode, _sliders[_sliders.Length - 1], current);
            var language = _languages[(int)Loc.Current];
            for (var i = 0; i < _resolutions.Length; i++)
                Link(_resolutions[i], _mode, language, i > 0 ? _resolutions[i - 1] : null,
                    i < _resolutions.Length - 1 ? _resolutions[i + 1] : null);
            for (var i = 0; i < _languages.Length; i++)
                Link(_languages[i], current, _close, i > 0 ? _languages[i - 1] : null,
                    i < _languages.Length - 1 ? _languages[i + 1] : null);
            Link(_close, language, null);
        }

        static void Link(Selectable item, Selectable up, Selectable down, Selectable left = null, Selectable right = null)
        {
            item.navigation = new Navigation
            {
                mode = Navigation.Mode.Explicit,
                selectOnUp = up,
                selectOnDown = down,
                selectOnLeft = left,
                selectOnRight = right,
            };
        }

        void AddLabel(string id, string key, float y)
        {
            var theme = UiTheme.Current;
            var text = UiKit.NewLabel(transform, "Label" + id, Loc.Get(key), theme.fontSize, TextAlignmentOptions.Left);
            text.GetComponent<LocText>().Key = key;
            UiKit.Place(text.rectTransform, new Vector2(theme.labelX, y), theme.labelSize);
        }

        PillButton AddChipButton(string id, string label, Vector2 position, Vector2 size, bool toggle) =>
            UiKit.NewChipButton(transform, id, label, position, size, UiTheme.Current.smallFontSize, toggle);

        PillSlider AddRow(int row, string id, string key, Sprite icon, float min, float max, bool whole, float value,
            float keyStep, Action<SettingsData, float> apply)
        {
            var theme = UiTheme.Current;
            var y = RowY(row);
            AddLabel(id, key, y);
            var chip = UiKit.NewIconChip(transform, "Chip" + id, icon, new Vector2(theme.rowChipX, y), theme.rowChipSize,
                theme.rowIconSize, false);

            var slider = UiKit.NewSlider(transform, id, min, max, whole, value, keyStep, chip);
            UiKit.Place((RectTransform)slider.transform, new Vector2(theme.controlX, y), theme.sliderSize);
            // 끄는 동안은 메모리에만 반영하고, 놓을 때 저장한다(매 프레임 디스크에 쓰면 끊긴다).
            slider.onValueChanged.AddListener(v => Settings.Edit(d => apply(d, v)));
            slider.Committed += Settings.SaveIfDirty;
            return slider;
        }

        void AddCloseButton()
        {
            var theme = UiTheme.Current;
            _close = UiKit.NewChipButton(transform, "Close", Loc.Get("settings.close"), new Vector2(0f, RowY(8) + theme.closeOffsetY),
                theme.closeButtonSize, theme.fontSize, false);
            _close.GetComponentInChildren<LocText>().Key = "settings.close";
            _close.onClick.AddListener(Close);
        }
    }
}
