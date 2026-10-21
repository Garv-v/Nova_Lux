using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NovaLux
{
    // 화면들이 함께 쓰는 UI 부품. 색과 글자 크기는 UiTheme.Current에서 읽는다.
    public static class UiKit
    {
        public static Image NewPanel(Transform parent, string name)
        {
            var image = NewImage(name, parent, UiTheme.Current.panelColor);
            Stretch(image.rectTransform);
            return image;
        }

        public static Button NewButton(Transform parent, string id, string label, Vector2 position, Vector2 size,
            int fontSize)
        {
            var image = NewImage(id, parent, UiTheme.Current.buttonColor);
            Place(image.rectTransform, position, size);
            var button = image.gameObject.AddComponent<Button>();
            Stretch(NewLabel(image.transform, "Label", label, fontSize, TextAlignmentOptions.Center).rectTransform);
            return button;
        }

        public static TextMeshProUGUI NewLabel(Transform parent, string name, string content, int size,
            TextAlignmentOptions alignment)
        {
            var text = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI))
                .GetComponent<TextMeshProUGUI>();
            text.transform.SetParent(parent, false);
            text.text = content;
            if (UiTheme.Current.Font != null) text.font = UiTheme.Current.Font;
            text.fontSize = size;
            text.alignment = alignment;
            text.textWrappingMode = TextWrappingModes.NoWrap;
            text.color = UiTheme.Current.textColor;
            text.raycastTarget = false;
            return text;
        }

        public static Slider NewSlider(Transform parent, string id, float min, float max, bool whole, float value)
        {
            var theme = UiTheme.Current;
            var root = new GameObject(id, typeof(RectTransform), typeof(Slider));
            root.transform.SetParent(parent, false);

            var bg = NewImage("Background", root.transform, theme.buttonColor);
            Stretch(bg.rectTransform);

            var fillArea = new GameObject("Fill Area", typeof(RectTransform)).GetComponent<RectTransform>();
            fillArea.SetParent(root.transform, false);
            Stretch(fillArea);
            fillArea.offsetMin = new Vector2(0f, theme.sliderFillInset);
            fillArea.offsetMax = new Vector2(0f, -theme.sliderFillInset);
            var fill = NewImage("Fill", fillArea, theme.accentColor);
            fill.rectTransform.anchorMin = Vector2.zero;
            fill.rectTransform.anchorMax = Vector2.one;
            fill.rectTransform.sizeDelta = Vector2.zero;

            var handleArea = new GameObject("Handle Slide Area", typeof(RectTransform)).GetComponent<RectTransform>();
            handleArea.SetParent(root.transform, false);
            Stretch(handleArea);
            handleArea.offsetMin = new Vector2(theme.sliderHandleInset, 0f);
            handleArea.offsetMax = new Vector2(-theme.sliderHandleInset, 0f);
            var handle = NewImage("Handle", handleArea, theme.handleColor);
            handle.rectTransform.sizeDelta = new Vector2(theme.sliderHandleWidth, 0f);

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

        public static void Stretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = rt.offsetMax = Vector2.zero;
        }

        public static void Place(RectTransform rt, Vector2 position, Vector2 size)
        {
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = position;
            rt.sizeDelta = size;
        }

        static Image NewImage(string name, Transform parent, Color color)
        {
            var image = new GameObject(name, typeof(RectTransform), typeof(Image)).GetComponent<Image>();
            image.transform.SetParent(parent, false);
            image.color = color;
            return image;
        }
    }
}
