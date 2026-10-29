using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace NovaLux
{
    // 화면 위에 뜨는 둥근 카드 패널의 공통부. 취소 입력으로 닫히고, 선택된 것이 없을 때 방향 입력이 오면 FirstSelection을 고른다.
    public abstract class ModalPanel : MonoBehaviour
    {
        protected InputSystemUIInputModule Module { get; private set; }
        Action _onClosed;
        bool _closing;
        int _cancelBlockedFrame;

        // onClosed는 패널이 닫힌 직후 불린다(화면이 포커스를 되돌려 놓는 자리).
        protected static T Open<T>(Transform canvas, Action onClosed) where T : ModalPanel
        {
            var root = UiKit.NewPanel(canvas, typeof(T).Name, UiTheme.Current.scrimColor);
            // 막은 기준 해상도 영역(레터박스 안쪽)만 덮는다. 캔버스 전체에 펴면 16:9가 아닌 화면의 검은 띠까지 번진다.
            UiKit.Place(root.rectTransform, Vector2.zero, canvas.GetComponentInParent<CanvasScaler>().referenceResolution);
            var panel = root.gameObject.AddComponent<T>();
            panel._onClosed = onClosed;
            return panel;
        }

        protected abstract GameObject FirstSelection { get; }

        protected virtual void Awake()
        {
            Module = FindFirstObjectByType<InputSystemUIInputModule>();
            // 패널을 연 입력(Esc)이 같은 프레임에 이 패널을 닫지 않게 한다.
            SuppressCancelThisFrame();
        }

        protected void SuppressCancelThisFrame() => _cancelBlockedFrame = Time.frameCount;

        void Update()
        {
            if (_closing || Module == null || Module.cancel == null || Time.frameCount == _cancelBlockedFrame) return;
            if (Module.cancel.action.WasPressedThisFrame()) Close();
        }

        void LateUpdate()
        {
            if (!_closing && UiNav.MoveWithoutSelection(Module))
                EventSystem.current.SetSelectedGameObject(FirstSelection);
        }

        protected void Close()
        {
            if (_closing) return;
            _closing = true;
            Destroy(gameObject);
            _onClosed?.Invoke();
        }
    }
}
