using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace NovaLux
{
    // 복도 씬에서 Esc로 ESC 메뉴를 연다. 방 화면(100) 위, 페이드(900) 아래에 뜬다.
    // 열려 있는 동안 걷기·문 입장·방 나가기는 IsOpen을 보고 멈춘다.
    public class PauseMenu : MonoBehaviour
    {
        const int SortingOrder = 200;

        public static bool IsOpen { get; private set; }

        InputAction _open;
        GameObject _previousSelection;
        int _closedFrame = -1;

        void OnEnable()
        {
            _open = new InputAction("Pause", InputActionType.Button);
            _open.AddBinding("<Keyboard>/escape");
            _open.Enable();
        }

        void OnDisable()
        {
            _open.Disable();
            _open.Dispose();
            IsOpen = false;
        }

        void Update()
        {
            // 닫힌 프레임에 같은 Esc로 다시 열리지 않게 한다.
            if (IsOpen || ScreenFade.Busy || Time.frameCount == _closedFrame || !_open.WasPressedThisFrame()) return;
            Open();
        }

        void Open()
        {
            UiKit.EnsureEventSystem(transform);
            var canvas = UiKit.NewScreenCanvas("PauseMenu", transform, SortingOrder);
            // 뒤에 있는 방의 나가기 버튼이 선택된 채 Enter를 받지 않게 비우고, 닫히면 되돌린다.
            _previousSelection = EventSystem.current.currentSelectedGameObject;
            EventSystem.current.SetSelectedGameObject(null);
            IsOpen = true;
            PausePanel.Create(canvas.transform, () =>
            {
                IsOpen = false;
                _closedFrame = Time.frameCount;
                Destroy(canvas.gameObject);
                if (_previousSelection != null) EventSystem.current.SetSelectedGameObject(_previousSelection);
            });
        }
    }
}
