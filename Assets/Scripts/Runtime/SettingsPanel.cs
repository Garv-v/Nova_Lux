using System;
using UnityEngine;
using UnityEngine.UI;

namespace NovaLux
{
    // 임시 박스 UI. 값을 바꾸면 즉시 Settings에 저장한다.
    public class SettingsPanel : MonoBehaviour
    {
        const float RowGap = 95f;
        const float FirstRowY = 330f;

        public static SettingsPanel Create(Transform canvas)
        {
            var go = new GameObject("SettingsPanel", typeof(RectTransform), typeof(Image));
            go.transform.SetParent(canvas, false);
            Stretch((RectTransform)go.transform);
            go.GetComponent<Image>().color = new Color(0.05f, 0.05f, 0.08f, 1f);
            return go.AddComponent<SettingsPanel>();
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
            var modeY = FirstRowY - 5 * RowGap;
            AddLabel("DisplayMode", "화면 모드", modeY);
            var mode = AddButton("DisplayMode", ModeLabel(s.fullscreen), new Vector2(250f, modeY),
                new Vector2(500f, 60f), Normal);
            var modeText = mode.GetComponentInChildren<Text>();
            mode.onClick.AddListener(() =>
            {
                Settings.Set(d => d.fullscreen = !d.fullscreen);
                modeText.text = ModeLabel(Settings.Current.fullscreen);
                ScreenSettings.Apply(Settings.Current);
            });

            var resY = FirstRowY - 6 * RowGap;
            AddLabel("Resolution", "해상도", resY);
            var buttons = new Button[ScreenSettings.Resolutions.Length];
            for (var i = 0; i < buttons.Length; i++)
            {
                var index = i;
                var x = 57.5f + i * 125f;
                buttons[i] = AddButton("Res" + ScreenSettings.Resolutions[i].y, ScreenSettings.ResolutionNames[i],
                    new Vector2(x, resY), new Vector2(115f, 60f), Normal);
                buttons[i].onClick.AddListener(() =>
                {
                    Settings.Set(d => d.resolution = index);
                    Highlight(buttons, index);
                    ScreenSettings.Apply(Settings.Current);
                });
            }
            Highlight(buttons, s.resolution);
        }

        static string ModeLabel(bool fullscreen) => fullscreen ? "전체화면" : "창모드";

        static readonly Color Normal = new Color(0.25f, 0.25f, 0.3f);
        static readonly Color Selected = new Color(0.45f, 0.6f, 0.9f);

        static void Highlight(Button[] buttons, int selected)
        {
            for (var i = 0; i < buttons.Length; i++)
                ((Image)buttons[i].targetGraphic).color = i == selected ? Selected : Normal;
        }

        void AddLabel(string id, string label, float y)
        {
            var text = AddText(transform, "Label" + id, label, 40, TextAnchor.MiddleRight);
            Place((RectTransform)text.transform, new Vector2(-300f, y), new Vector2(420f, 60f));
        }

        Button AddButton(string id, string label, Vector2 position, Vector2 size, Color color)
        {
            var image = NewImage(id, transform, color);
            Place(image.rectTransform, position, size);
            var button = image.gameObject.AddComponent<Button>();
            var text = AddText(image.transform, "Label", label, 36, TextAnchor.MiddleCenter);
            Stretch((RectTransform)text.transform);
            return button;
        }

        void AddRow(int row, string id, string label, float min, float max, bool whole, float value,
            Action<SettingsData, float> apply)
        {
            var y = FirstRowY - row * RowGap;
            AddLabel(id, label, y);

            var slider = AddSlider(id, min, max, whole, value);
            Place((RectTransform)slider.transform, new Vector2(250f, y), new Vector2(500f, 40f));
            slider.onValueChanged.AddListener(v => Settings.Set(d => apply(d, v)));
        }

        Slider AddSlider(string id, float min, float max, bool whole, float value)
        {
            var root = new GameObject(id, typeof(RectTransform), typeof(Slider));
            root.transform.SetParent(transform, false);

            var bg = NewImage("Background", root.transform, new Color(0.25f, 0.25f, 0.3f));
            Stretch(bg.rectTransform);

            var fillArea = new GameObject("Fill Area", typeof(RectTransform)).GetComponent<RectTransform>();
            fillArea.SetParent(root.transform, false);
            Stretch(fillArea);
            fillArea.offsetMin = new Vector2(0f, 12f);
            fillArea.offsetMax = new Vector2(0f, -12f);
            var fill = NewImage("Fill", fillArea, new Color(0.45f, 0.6f, 0.9f));
            fill.rectTransform.anchorMin = Vector2.zero;
            fill.rectTransform.anchorMax = Vector2.one;
            fill.rectTransform.sizeDelta = Vector2.zero;

            var handleArea = new GameObject("Handle Slide Area", typeof(RectTransform)).GetComponent<RectTransform>();
            handleArea.SetParent(root.transform, false);
            Stretch(handleArea);
            handleArea.offsetMin = new Vector2(15f, 0f);
            handleArea.offsetMax = new Vector2(-15f, 0f);
            var handle = NewImage("Handle", handleArea, Color.white);
            handle.rectTransform.sizeDelta = new Vector2(30f, 0f);

            var slider = root.GetComponent<Slider>();
            slider.fillRect = fill.rectTransform;
            slider.handleRect = handle.rectTransform;
            slider.targetGraphic = handle;
            slider.direction = Slider.Direction.LeftToRight;
            slider.minValue = min;
            slider.maxValue = max;
            slider.wholeNumbers = whole;
            slider.SetValueWithoutNotify(value);
            return slider;
        }

        void AddCloseButton()
        {
            var image = NewImage("Close", transform, new Color(0.25f, 0.25f, 0.3f));
            Place(image.rectTransform, new Vector2(0f, FirstRowY - 7 * RowGap - 20f), new Vector2(300f, 80f));
            var button = image.gameObject.AddComponent<Button>();
            button.onClick.AddListener(() => Destroy(gameObject));

            var text = AddText(image.transform, "Label", "닫기", 40, TextAnchor.MiddleCenter);
            Stretch((RectTransform)text.transform);
        }

        static Image NewImage(string name, Transform parent, Color color)
        {
            var image = new GameObject(name, typeof(RectTransform), typeof(Image)).GetComponent<Image>();
            image.transform.SetParent(parent, false);
            image.color = color;
            return image;
        }

        static Text AddText(Transform parent, string name, string content, int size, TextAnchor anchor)
        {
            var text = new GameObject(name, typeof(RectTransform), typeof(Text)).GetComponent<Text>();
            text.transform.SetParent(parent, false);
            text.text = content;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = size;
            text.alignment = anchor;
            text.color = Color.white;
            text.raycastTarget = false;
            return text;
        }

        static void Stretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
        }

        static void Place(RectTransform rt, Vector2 position, Vector2 size)
        {
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = position;
            rt.sizeDelta = size;
        }
    }
}
