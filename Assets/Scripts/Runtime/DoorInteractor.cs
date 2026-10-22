using UnityEngine;
using UnityEngine.InputSystem;

namespace NovaLux
{
    // 금태양이 문 앞에 서면 입장 표시를 띄우고, 입력하면 페이드로 방에 들어간다.
    // 나가면 들어간 문의 x에서 다시 걷는다. 입장·퇴장은 행동력을 쓰지 않는다.
    [RequireComponent(typeof(HallwayWalker))]
    public class DoorInteractor : MonoBehaviour
    {
        HallwayWalker _walker;
        Door[] _doors;
        InputAction _enter;
        RoomScreen _room;

        void Awake()
        {
            _walker = GetComponent<HallwayWalker>();
            _doors = FindObjectsByType<Door>();
        }

        void OnEnable()
        {
            _enter = new InputAction("Enter", InputActionType.Button);
            _enter.AddBinding("<Keyboard>/e");
            _enter.AddBinding("<Keyboard>/enter");
            _enter.AddBinding("<Keyboard>/upArrow");
            _enter.AddBinding("<Keyboard>/w");
            _enter.AddBinding("<Gamepad>/buttonSouth");
            _enter.Enable();
        }

        void OnDisable()
        {
            _enter.Disable();
            _enter.Dispose();
        }

        void Update()
        {
            var x = transform.position.x;
            Door near = null;
            foreach (var door in _doors)
                if (door.Contains(x)) { near = door; break; }

            var canEnter = _room == null && near != null;
            foreach (var door in _doors) door.SetPromptVisible(canEnter && door == near);

            if (canEnter && !ScreenFade.Busy && _enter.WasPressedThisFrame()) Enter(near);
        }

        void Enter(Door door)
        {
            // 페이드 중에 문 앞을 벗어나지 않게 입력 시점에 걷기를 끈다.
            _walker.enabled = false;
            ScreenFade.Run(() => _room = RoomScreen.Open(door, Exit));
        }

        void Exit()
        {
            if (ScreenFade.Busy) return;
            ScreenFade.Run(() =>
            {
                var door = _room.Door;
                Destroy(_room.gameObject);
                _room = null;
                var p = transform.position;
                p.x = door.X;
                transform.position = p;
                _walker.enabled = true;
            });
        }
    }
}
