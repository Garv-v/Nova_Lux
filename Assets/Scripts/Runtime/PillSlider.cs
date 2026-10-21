using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NovaLux
{
    // 둥근 슬라이더. 포커스(호버·키보드·패드)를 손잡이 테두리와 그 줄의 아이콘 칩으로 보여 준다.
    // 키보드·패드 좌우 입력은 keyStep만큼 값을 바꾼다(기본 Slider는 범위의 10%라 TextSpeed가 너무 느리다).
    public class PillSlider : Slider
    {
        public float keyStep;

        // 사용자가 값을 정하는 순간: 마우스를 놓을 때, 키보드·패드로 한 단계 옮길 때. 끄는 중에는 불리지 않는다.
        public event Action Committed;

        UiTheme _theme;
        Outline _ring;
        Image _chip;
        Image _icon;

        // chip, icon은 그 줄의 아이콘 칩이다(없으면 null).
        public void Bind(UiTheme theme, Outline ring, Image chip, Image icon)
        {
            _theme = theme;
            _ring = ring;
            _chip = chip;
            _icon = icon;
            transition = Transition.None;
            DoStateTransition(currentSelectionState, true);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            if (IsInteractable() && EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(gameObject);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            Committed?.Invoke();
        }

        public override void OnMove(AxisEventData eventData)
        {
            // 옆으로 갈 항목이 없을 때만 값을 바꾼다. 그 규칙은 기본 Slider와 같다.
            var right = eventData.moveDir == MoveDirection.Right;
            var horizontal = right || eventData.moveDir == MoveDirection.Left;
            if (keyStep > 0f && IsActive() && IsInteractable() && horizontal
                && (right ? FindSelectableOnRight() : FindSelectableOnLeft()) == null)
            {
                value += right ? keyStep : -keyStep;
                Committed?.Invoke();
                eventData.Use();
                return;
            }
            base.OnMove(eventData);
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            if (_theme == null || !gameObject.activeInHierarchy) return;
            var focused = state == SelectionState.Highlighted || state == SelectionState.Selected
                || state == SelectionState.Pressed;
            if (_ring != null)
            {
                _ring.effectColor = _theme.pressedColor;
                _ring.enabled = focused;
            }
            if (_chip != null) _chip.color = focused ? _theme.accentColor : _theme.chipColor;
            if (_icon != null) _icon.color = focused ? _theme.selectedTextColor : _theme.iconColor;
        }
    }
}
