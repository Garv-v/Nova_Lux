using System;
using UnityEngine;
using UnityEngine.UI;

namespace NovaLux
{
    // 임시 박스 UI. 값을 바꾸면 즉시 Settings에 저장한다.
    public class SettingsPanel : MonoBehaviour
    {
        const float RowGap = 110f;
        const float FirstRowY = 250f;

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
            AddCloseButton();
        }

        void AddRow(int row, string id, string label, float min, float max, bool whole, float value,
            Action<SettingsData, float> apply)
        {
            var y = FirstRowY - row * RowGap;
            var text = AddText(transform, "Label" + id, label, 40, TextAnchor.MiddleRight);
            Place((RectTransform)text.transform, new Vector2(-300f, y), new Vector2(420f, 60f));

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
            Place(image.rectTransform, new Vector2(0f, FirstRowY - 5 * RowGap - 20f), new Vector2(300f, 80f));
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
