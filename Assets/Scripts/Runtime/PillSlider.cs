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

        UiTheme _theme;
        Outline _ring;
        Image _chip;
        Image _icon;

        public void Bind(UiTheme theme, Outline ring)
        {
            _theme = theme;
            _ring = ring;
            transition = Transition.None;
            DoStateTransition(currentSelectionState, true);
        }

        public void AttachChip(Image chip, Image icon)
        {
            _chip = chip;
            _icon = icon;
            DoStateTransition(currentSelectionState, true);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            if (IsInteractable() && EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(gameObject);
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
                _ring.effectColor = _theme.accentColor;
                _ring.enabled = focused;
            }
            if (_chip != null) _chip.color = focused ? _theme.accentColor : _theme.chipColor;
            if (_icon != null) _icon.color = focused ? _theme.selectedTextColor : _theme.iconColor;
        }
    }
}
