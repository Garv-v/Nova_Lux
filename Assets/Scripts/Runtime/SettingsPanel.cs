using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace NovaLux
{
    // 둥근 카드 위의 환경설정. 값을 바꾸면 즉시 Settings에 저장한다.
    // 키보드·패드는 패널 안에서만 움직이도록 이동 경로를 직접 이어 둔다(뒤에 가려진 초기화면 버튼으로 새지 않게).
    public class SettingsPanel : MonoBehaviour
    {
        InputSystemUIInputModule _module;
        Action _onClosed;
        bool _closing;

        readonly PillSlider[] _sliders = new PillSlider[5];
        PillButton _mode;
        PillButton[] _resolutions;
        PillButton _close;

        // onClosed는 패널이 닫힌 직후 불린다(화면이 포커스를 되돌려 놓는 자리).
        public static SettingsPanel Create(Transform canvas, Action onClosed)
        {
            var root = UiKit.NewPanel(canvas, "SettingsPanel", UiTheme.Current.scrimColor);
            // 막은 기준 해상도 영역(레터박스 안쪽)만 덮는다. 캔버스 전체에 펴면 16:9가 아닌 화면의 검은 띠까지 번진다.
            UiKit.Place(root.rectTransform, Vector2.zero, canvas.GetComponentInParent<CanvasScaler>().referenceResolution);
            var panel = root.gameObject.AddComponent<SettingsPanel>();
            panel._onClosed = onClosed;
            return panel;
        }

        void Awake()
        {
            _module = FindFirstObjectByType<InputSystemUIInputModule>();
            var theme = UiTheme.Current;
            UiKit.NewCard(transform, "Card", new Vector2(0f, theme.cardOffsetY), theme.cardSize);

            var s = Settings.Current;
            _sliders[0] = AddRow(0, "Bgm", "BGM", theme.iconBgm, 0f, 1f, false, s.bgm, 0.05f, (d, v) => d.bgm = v);
            _sliders[1] = AddRow(1, "Sfx", "효과음", theme.iconSfx, 0f, 1f, false, s.sfx, 0.05f, (d, v) => d.sfx = v);
            _sliders[2] = AddRow(2, "Voice", "보이스", theme.iconVoice, 0f, 1f, false, s.voice, 0.05f, (d, v) => d.voice = v);
            _sliders[3] = AddRow(3, "TextSpeed", "텍스트 속도", theme.iconTextSpeed, 10f, 100f, true, s.textCps, 5f,
                (d, v) => d.textCps = v);
            _sliders[4] = AddRow(4, "AutoSpeed", "자동 진행", theme.iconAuto, 0.5f, 5f, false, s.autoDelaySec, 0.25f,
                (d, v) => d.autoDelaySec = v);
            AddDisplayRows(s);
            AddCloseButton();
            LinkNavigation();
        }

        void Update()
        {
            if (_closing || _module == null || _module.cancel == null) return;
            if (_module.cancel.action.WasPressedThisFrame()) Close();
        }

        // 선택된 것이 없을 때 방향 입력이 오면 첫 슬라이더부터 고른다. 열자마자 고르면 마우스로 열어도 채워져 보인다.
        void LateUpdate()
        {
            if (!_closing && UiNav.MoveWithoutSelection(_module))
                EventSystem.current.SetSelectedGameObject(_sliders[0].gameObject);
        }

        void Close()
        {
            if (_closing) return;
            _closing = true;
            Destroy(gameObject);
            _onClosed?.Invoke();
        }

        void AddDisplayRows(SettingsData s)
        {
            var theme = UiTheme.Current;
            var modeY = RowY(5);
            AddLabel("DisplayMode", "화면 모드", modeY);
            _mode = AddChipButton("DisplayMode", ModeLabel(s.fullscreen), new Vector2(theme.controlX, modeY),
                theme.modeButtonSize, false);
            var modeText = _mode.GetComponentInChildren<TMP_Text>();
            _mode.onClick.AddListener(() =>
            {
                Settings.Set(d => d.fullscreen = !d.fullscreen);
                modeText.text = ModeLabel(Settings.Current.fullscreen);
                ScreenSettings.Apply(Settings.Current);
            });

            var resY = RowY(6);
            AddLabel("Resolution", "해상도", resY);
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
        }

        static float RowY(int row) => UiTheme.Current.firstRowY - row * UiTheme.Current.rowGap;

        static string ModeLabel(bool fullscreen) => fullscreen ? "전체화면" : "창모드";

        void ShowChecked(int selected)
        {
            for (var i = 0; i < _resolutions.Length; i++) _resolutions[i].IsOn = i == selected;
            LinkNavigation();
        }

        // 위아래는 항목 순서대로, 해상도 줄은 좌우로 잇는다. 슬라이더의 좌우는 비워 둬야 값 조정으로 쓰인다.
        // 해상도 줄로 들어갈 때는 현재 선택된 해상도로 간다.
        void LinkNavigation()
        {
            if (_close == null) return;
            var current = _resolutions[Settings.Current.resolution];
            for (var i = 0; i < _sliders.Length; i++)
                Link(_sliders[i], i > 0 ? _sliders[i - 1] : null, i < _sliders.Length - 1 ? _sliders[i + 1] : _mode);
            Link(_mode, _sliders[_sliders.Length - 1], current);
            for (var i = 0; i < _resolutions.Length; i++)
                Link(_resolutions[i], _mode, _close, i > 0 ? _resolutions[i - 1] : null,
                    i < _resolutions.Length - 1 ? _resolutions[i + 1] : null);
            Link(_close, current, null);
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

        void AddLabel(string id, string label, float y)
        {
            var theme = UiTheme.Current;
            var text = UiKit.NewLabel(transform, "Label" + id, label, theme.fontSize, TextAlignmentOptions.Left);
            UiKit.Place(text.rectTransform, new Vector2(theme.labelX, y), theme.labelSize);
        }

        PillButton AddChipButton(string id, string label, Vector2 position, Vector2 size, bool toggle) =>
            UiKit.NewChipButton(transform, id, label, position, size, UiTheme.Current.smallFontSize, toggle);

        PillSlider AddRow(int row, string id, string label, Sprite icon, float min, float max, bool whole, float value,
            float keyStep, Action<SettingsData, float> apply)
        {
            var theme = UiTheme.Current;
            var y = RowY(row);
            AddLabel(id, label, y);
            var chip = UiKit.NewIconChip(transform, "Chip" + id, icon, new Vector2(theme.rowChipX, y), theme.rowChipSize,
                theme.rowIconSize, false);

            var slider = UiKit.NewSlider(transform, id, min, max, whole, value, keyStep, chip);
            UiKit.Place((RectTransform)slider.transform, new Vector2(theme.controlX, y), theme.sliderSize);
            slider.onValueChanged.AddListener(v => Settings.Set(d => apply(d, v)));
            return slider;
        }

        void AddCloseButton()
        {
            var theme = UiTheme.Current;
            _close = UiKit.NewChipButton(transform, "Close", "닫기", new Vector2(0f, RowY(7) + theme.closeOffsetY),
                theme.closeButtonSize, theme.fontSize, false);
            _close.onClick.AddListener(Close);
        }
    }
}
