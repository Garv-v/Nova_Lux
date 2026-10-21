using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NovaLux
{
    // 알약 모양 메뉴 버튼. 상태(보통·선택·눌림·비활성)에 따라 면, 라벨, 아이콘 칩 색을 테마에서 읽어 바꾼다.
    // 호버, 키보드·패드 포커스는 모두 "선택"이라 같은 모양이다. transition은 쓰지 않는다(곱셈 틴트로는 글자를 흰색으로 못 바꾼다).
    public class PillButton : Button
    {
        UiTheme _theme;
        TMP_Text _label;
        Image _chip;
        Image _icon;

        public void Bind(UiTheme theme, TMP_Text label, Image chip, Image icon)
        {
            _theme = theme;
            _label = label;
            _chip = chip;
            _icon = icon;
            transition = Transition.None;
            DoStateTransition(currentSelectionState, true);
            FitCorners();
        }

        // 호버한 버튼이 곧 선택이라 마우스와 키보드가 같은 항목을 가리킨다.
        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            if (IsInteractable() && EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(gameObject);
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            if (_label == null || !gameObject.activeInHierarchy) return;
            var theme = _theme != null ? _theme : UiTheme.Current;
            Color face = theme.buttonColor, text = theme.textColor, chip = theme.chipColor, icon = theme.iconColor;
            switch (state)
            {
                case SelectionState.Highlighted:
                case SelectionState.Selected:
                    face = theme.accentColor;
                    text = theme.selectedTextColor;
                    chip = theme.selectedChipColor;
                    icon = theme.accentColor;
                    break;
                case SelectionState.Pressed:
                    face = theme.pressedColor;
                    text = theme.selectedTextColor;
                    chip = theme.selectedChipColor;
                    icon = theme.pressedColor;
                    break;
                case SelectionState.Disabled:
                    text = icon = theme.disabledColor;
                    chip = theme.disabledChipColor;
                    break;
            }
            ((Graphic)targetGraphic).color = face;
            _label.color = text;
            _chip.color = chip;
            _icon.color = icon;
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            FitCorners();
        }

        // 9-slice 모서리는 가로·세로를 따로 줄이므로, 스프라이트 높이가 버튼 높이에 딱 맞게 배율을 잡아 반지름이 항상 높이의 절반이 되게 한다.
        void FitCorners()
        {
            var face = targetGraphic as Image;
            if (face == null || face.sprite == null) return;
            var height = ((RectTransform)transform).rect.height;
            if (height <= 0f || face.pixelsPerUnit <= 0f) return;
            face.pixelsPerUnitMultiplier = face.sprite.rect.height / (face.pixelsPerUnit * height);
        }
    }
}
