using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NovaLux
{
    // 화면들이 함께 쓰는 UI 부품. 색과 글자 크기는 UiTheme.Current에서 읽는다.
    public static class UiKit
    {
        public static Image NewPanel(Transform parent, string name, Color color)
        {
            var image = NewImage(name, parent, color);
            Stretch(image.rectTransform);
            return image;
        }

        // 가운데 놓이는 둥근 카드. 모서리 반지름은 크기와 상관없이 theme.cardCornerRadius다.
        public static Image NewCard(Transform parent, string name, Vector2 position, Vector2 size)
        {
            var theme = UiTheme.Current;
            var card = NewPillFace(name, parent, theme.panelColor, position, size);
            card.GetComponent<PillCorners>().Radius = theme.cardCornerRadius;
            return card;
        }

        // 초기화면용 알약 버튼: 9-slice 면, 왼쪽의 아이콘 칩, 칩 오른쪽의 라벨.
        public static PillButton NewPillButton(Transform parent, string id, string label, Sprite icon, Vector2 position,
            Vector2 size, int fontSize)
        {
            var theme = UiTheme.Current;
            var face = NewPillFace(id, parent, theme.buttonColor, position, size);
            var button = face.gameObject.AddComponent<PillButton>();

            var chip = NewIconChip(face.transform, "Chip", icon, new Vector2(theme.titleChipInset, 0f),
                theme.titleChipSize, theme.titleIconSize, true);
            var text = NewLabel(face.transform, "Label", label, fontSize, TextAlignmentOptions.Left);
            Stretch(text.rectTransform);
            text.rectTransform.offsetMin = new Vector2(theme.titleChipInset + theme.titleChipSize + theme.titleLabelGap, 0f);

            button.targetGraphic = face;
            button.Bind(theme, text, chip, chip.transform.Find("Icon").GetComponent<Image>(), theme.buttonColor, null);
            return button;
        }

        // 칩 없이 글자만 가운데 있는 알약 버튼. ringFocus면 토글 칩이라 포커스를 테두리로 보여 준다.
        public static PillButton NewChipButton(Transform parent, string id, string label, Vector2 position, Vector2 size,
            int fontSize, bool ringFocus)
        {
            var theme = UiTheme.Current;
            var face = NewPillFace(id, parent, theme.secondaryChipColor, position, size);
            var button = face.gameObject.AddComponent<PillButton>();
            var text = NewLabel(face.transform, "Label", label, fontSize, TextAlignmentOptions.Center);
            Stretch(text.rectTransform);

            Outline ring = null;
            if (ringFocus)
            {
                ring = face.gameObject.AddComponent<Outline>();
                ring.effectDistance = new Vector2(theme.focusRingWidth, -theme.focusRingWidth);
            }
            button.targetGraphic = face;
            button.Bind(theme, text, null, null, theme.secondaryChipColor, ring);
            return button;
        }

        // 원 칩 안에 아이콘이 들어간 부품. leftAnchored면 부모 왼쪽 가장자리 기준, 아니면 가운데 기준 위치다.
        public static Image NewIconChip(Transform parent, string name, Sprite icon, Vector2 position, float chipSize,
            float iconSize, bool leftAnchored)
        {
            var theme = UiTheme.Current;
            var chip = NewImage(name, parent, theme.chipColor);
            chip.sprite = theme.circleSprite;
            chip.raycastTarget = false;
            var rect = chip.rectTransform;
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(leftAnchored ? 0f : 0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(chipSize, chipSize);

            var iconImage = NewImage("Icon", chip.transform, theme.iconColor);
            iconImage.sprite = icon;
            iconImage.enabled = icon != null;
            iconImage.raycastTarget = false;
            Place(iconImage.rectTransform, Vector2.zero, new Vector2(iconSize, iconSize));
            return chip;
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

        public static PillSlider NewSlider(Transform parent, string id, float min, float max, bool whole, float value,
            float keyStep)
        {
            var theme = UiTheme.Current;
            var root = new GameObject(id, typeof(RectTransform), typeof(PillSlider));
            root.transform.SetParent(parent, false);
            var half = theme.sliderHandleSize / 2f;
            var track = theme.sliderTrackHeight;

            // 트랙은 세로 가운데에 가로로 꽉 차고, 채움 영역과 손잡이 영역은 양끝을 손잡이 반지름만큼 들인다.
            var bg = NewPillImage("Background", root.transform, theme.sliderTrackColor);
            bg.rectTransform.anchorMin = new Vector2(0f, 0.5f);
            bg.rectTransform.anchorMax = new Vector2(1f, 0.5f);
            bg.rectTransform.sizeDelta = new Vector2(0f, track);

            var fillArea = new GameObject("Fill Area", typeof(RectTransform)).GetComponent<RectTransform>();
            fillArea.SetParent(root.transform, false);
            fillArea.anchorMin = new Vector2(0f, 0.5f);
            fillArea.anchorMax = new Vector2(1f, 0.5f);
            fillArea.offsetMin = new Vector2(track / 2f, -track / 2f);
            fillArea.offsetMax = new Vector2(-track / 2f, track / 2f);
            // 채움은 폭이 값 0에서도 트랙 높이만큼이라 양끝이 둥글고, 왼쪽 끝은 트랙과 맞고 오른쪽 끝은 손잡이 반지름 안에 가려진다.
            var fill = NewPillImage("Fill", fillArea, theme.accentColor);
            fill.rectTransform.anchorMin = Vector2.zero;
            fill.rectTransform.anchorMax = Vector2.one;
            fill.rectTransform.sizeDelta = new Vector2(track, 0f);

            var handleArea = new GameObject("Handle Slide Area", typeof(RectTransform)).GetComponent<RectTransform>();
            handleArea.SetParent(root.transform, false);
            handleArea.anchorMin = new Vector2(0f, 0.5f);
            handleArea.anchorMax = new Vector2(1f, 0.5f);
            handleArea.offsetMin = new Vector2(half, -half);
            handleArea.offsetMax = new Vector2(-half, half);
            // Slider가 값을 바꿀 때마다 손잡이의 세로 앵커를 0~1로 되돌리므로, 영역 높이를 손잡이 크기로 두고 세로는 영역에 맞춘다.
            var handle = NewImage("Handle", handleArea, theme.handleColor);
            handle.sprite = theme.circleSprite;
            handle.rectTransform.sizeDelta = new Vector2(theme.sliderHandleSize, 0f);
            var ring = handle.gameObject.AddComponent<Outline>();
            ring.effectDistance = new Vector2(theme.focusRingWidth, -theme.focusRingWidth);

            var slider = root.GetComponent<PillSlider>();
            slider.fillRect = fill.rectTransform;
            slider.handleRect = handle.rectTransform;
            slider.targetGraphic = handle;
            slider.direction = Slider.Direction.LeftToRight;
            slider.minValue = min;
            slider.maxValue = max;
            slider.wholeNumbers = whole;
            slider.keyStep = keyStep;
            slider.SetValueWithoutNotify(value);
            slider.Bind(theme, ring);
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

        // 알약 스프라이트 면. 스프라이트가 없으면 단색 사각형이다.
        static Image NewPillFace(string name, Transform parent, Color color, Vector2 position, Vector2 size)
        {
            var face = NewPillImage(name, parent, color);
            Place(face.rectTransform, position, size);
            return face;
        }

        static Image NewPillImage(string name, Transform parent, Color color)
        {
            var theme = UiTheme.Current;
            var image = NewImage(name, parent, color);
            image.sprite = theme.pillSprite;
            image.type = theme.pillSprite != null ? Image.Type.Sliced : Image.Type.Simple;
            image.gameObject.AddComponent<PillCorners>();
            return image;
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
