using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NovaLux
{
    // 알약 모양 버튼. 상태(보통·선택·눌림·비활성)에 따라 면, 라벨, 아이콘 칩 색을 테마에서 읽어 바꾼다.
    // 호버, 키보드·패드 포커스는 모두 "선택"이라 같은 모양이다. transition은 쓰지 않는다(곱셈 틴트로는 글자를 흰색으로 못 바꾼다).
    // ring이 있으면 토글 칩이다: 채움은 IsOn(현재 설정값), 포커스는 테두리로 따로 보여 준다.
    public class PillButton : Button
    {
        UiTheme _theme;
        TMP_Text _label;
        Image _chip;
        Image _icon;
        Color _rest;
        Outline _ring;
        bool _isOn;

        // chip, icon은 칩이 없는 버튼이면 null이다.
        public void Bind(UiTheme theme, TMP_Text label, Image chip, Image icon, Color rest, Outline ring)
        {
            _theme = theme;
            _label = label;
            _chip = chip;
            _icon = icon;
            _rest = rest;
            _ring = ring;
            transition = Transition.None;
            DoStateTransition(currentSelectionState, true);
        }

        public bool IsOn
        {
            get => _isOn;
            set
            {
                _isOn = value;
                if (_label != null) DoStateTransition(currentSelectionState, true);
            }
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
            Color face = _rest, text = theme.textColor, chip = theme.chipColor, icon = theme.iconColor;
            var focused = state == SelectionState.Highlighted || state == SelectionState.Selected;
            var ring = false;
            if (state == SelectionState.Disabled)
            {
                text = icon = theme.disabledColor;
                chip = theme.disabledChipColor;
            }
            else if (_ring != null)
            {
                ring = focused || state == SelectionState.Pressed;
                if (state == SelectionState.Pressed)
                {
                    face = theme.pressedColor;
                    text = theme.selectedTextColor;
                }
                else if (_isOn)
                {
                    face = theme.accentColor;
                    text = theme.selectedTextColor;
                }
                else if (focused)
                {
                    text = theme.pressedColor;
                }
            }
            else if (focused)
            {
                face = theme.accentColor;
                text = theme.selectedTextColor;
                chip = theme.selectedChipColor;
                icon = theme.accentColor;
            }
            else if (state == SelectionState.Pressed)
            {
                face = theme.pressedColor;
                text = theme.selectedTextColor;
                chip = theme.selectedChipColor;
                icon = theme.pressedColor;
            }
            ((Graphic)targetGraphic).color = face;
            _label.color = text;
            if (_chip != null) _chip.color = chip;
            if (_icon != null) _icon.color = icon;
            if (_ring != null)
            {
                _ring.effectColor = theme.pressedColor;
                _ring.enabled = ring;
            }
        }
    }
}
