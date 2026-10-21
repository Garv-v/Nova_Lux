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

        // 초기화면용 알약 버튼: 9-slice 면, 왼쪽의 아이콘 칩, 칩 오른쪽의 라벨.
        public static PillButton NewPillButton(Transform parent, string id, string label, Sprite icon, Vector2 position,
            Vector2 size, int fontSize)
        {
            var theme = UiTheme.Current;
            var face = NewImage(id, parent, theme.buttonColor);
            face.sprite = theme.pillSprite;
            face.type = theme.pillSprite != null ? Image.Type.Sliced : Image.Type.Simple;
            Place(face.rectTransform, position, size);
            var button = face.gameObject.AddComponent<PillButton>();

            var chip = NewImage("Chip", face.transform, theme.chipColor);
            chip.sprite = theme.circleSprite;
            chip.raycastTarget = false;
            var chipRect = chip.rectTransform;
            chipRect.anchorMin = chipRect.anchorMax = chipRect.pivot = new Vector2(0f, 0.5f);
            chipRect.anchoredPosition = new Vector2(theme.titleChipInset, 0f);
            chipRect.sizeDelta = new Vector2(theme.titleChipSize, theme.titleChipSize);

            var iconImage = NewImage("Icon", chip.transform, theme.iconColor);
            iconImage.sprite = icon;
            iconImage.enabled = icon != null;
            iconImage.raycastTarget = false;
            Place(iconImage.rectTransform, Vector2.zero, new Vector2(theme.titleIconSize, theme.titleIconSize));

            var text = NewLabel(face.transform, "Label", label, fontSize, TextAlignmentOptions.Left);
            Stretch(text.rectTransform);
            text.rectTransform.offsetMin = new Vector2(theme.titleChipInset + theme.titleChipSize + theme.titleLabelGap, 0f);

            button.targetGraphic = face;
            button.Bind(theme, text, chip, iconImage);
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
