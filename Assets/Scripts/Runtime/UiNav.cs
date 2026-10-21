using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace NovaLux
{
    public static class UiNav
    {
        // 선택된 것이 없을 때 방향 입력이 왔는지. 화면이 첫 항목을 고르는 시점이다.
        public static bool MoveWithoutSelection(InputSystemUIInputModule module)
        {
            var system = EventSystem.current;
            if (system == null || module == null || module.move == null) return false;
            var selected = system.currentSelectedGameObject;
            if (selected != null && selected.activeInHierarchy) return false;
            return module.move.action.ReadValue<Vector2>() != Vector2.zero;
        }
    }
}
