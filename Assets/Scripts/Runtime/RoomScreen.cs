using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace NovaLux
{
    // 방 임시 화면. 복도 씬 위에 덮이는 오버레이라 닫으면 복도가 그대로 남는다.
    // 상호작용 자리는 표시만 한다. 동작과 행동력 소모는 이번 범위가 아니다.
    // 실제 방 화면이 생기면 Open 안쪽만 바꾼다.
    public class RoomScreen : MonoBehaviour
    {
        Door _door;
        Action _onExit;
        InputAction _exit;

        public Door Door => _door;

        public static RoomScreen Open(Door door, Action onExit)
        {
            var room = UiKit.NewScreenCanvas("RoomScreen", null, 100).gameObject.AddComponent<RoomScreen>();
            room._door = door;
            room._onExit = onExit;
            room.Build();
            return room;
        }

        void Build()
        {
            var theme = UiTheme.Current;

            UiKit.EnsureEventSystem(transform);

            var scrim = UiKit.NewPanel(transform, "Scrim", theme.scrimColor);
            scrim.raycastTarget = true;
            UiKit.NewCard(transform, "Card", new Vector2(0f, theme.cardOffsetY), theme.cardSize);

            var title = UiKit.NewLabel(transform, "Title", "", theme.fontSize, TextAlignmentOptions.Center);
            title.GetComponent<LocText>().Key = "room." + _door.Id;
            UiKit.Place(title.rectTransform, new Vector2(0f, theme.galleryTitleY), theme.galleryTitleSize);

            var slot = UiKit.NewCard(transform, "InteractSlot", new Vector2(0f, theme.cardOffsetY), new Vector2(520f, 200f));
            slot.color = theme.lockedSlotColor;
            slot.raycastTarget = false;
            var label = UiKit.NewLabel(slot.transform, "Label", "", theme.fontSize, TextAlignmentOptions.Center);
            label.GetComponent<LocText>().Key = "room.interact";
            UiKit.Stretch(label.rectTransform);

            var exit = UiKit.NewChipButton(transform, "Exit", "", new Vector2(0f, theme.galleryCloseY),
                theme.closeButtonSize, theme.fontSize, false);
            exit.GetComponentInChildren<LocText>().Key = "room.exit";
            exit.onClick.AddListener(RequestExit);
            // 자동 이동이 다른 대상으로 새지 않게 한다.
            exit.navigation = new Navigation { mode = Navigation.Mode.None };
            EventSystem.current.SetSelectedGameObject(exit.gameObject);

            _exit = new InputAction("Exit");
            _exit.AddBinding("<Keyboard>/escape");
            _exit.AddBinding("<Gamepad>/buttonEast");
            _exit.Enable();
        }

        void Update()
        {
            if (_exit.WasPressedThisFrame()) RequestExit();
        }

        void OnDestroy()
        {
            _exit?.Disable();
            _exit?.Dispose();
        }

        void RequestExit() => _onExit?.Invoke();
    }
}
