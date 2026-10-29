using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NovaLux
{
    // 임시 박스 UI. 아트는 이후 티켓에서 바꾼다. 문자열은 Loc 표의 키로 받는다.
    public class TitleScreen : MonoBehaviour
    {
        InputSystemUIInputModule _module;
        MonoBehaviour _panel;

        void Awake()
        {
            var canvas = UiKit.NewScreenCanvas("Canvas", transform, 0).transform;
            UiKit.EnsureEventSystem(transform);
            _module = FindFirstObjectByType<InputSystemUIInputModule>();

            AddButton(canvas, 0, "NewGame", "title.newGame", UiTheme.Current.iconNewGame, () => SceneManager.LoadScene(SceneNames.Hallway));
            AddButton(canvas, 1, "Continue", "title.continue", UiTheme.Current.iconContinue, null);
            PillButton gallery = null;
            gallery = AddButton(canvas, 2, "Gallery", "title.gallery", UiTheme.Current.iconGallery, () =>
            {
                EventSystem.current.SetSelectedGameObject(null);
                _panel = GalleryPanel.Create(canvas,
                    () => EventSystem.current.SetSelectedGameObject(gallery.gameObject));
            });
            PillButton settings = null;
            settings = AddButton(canvas, 3, "Settings", "title.settings", UiTheme.Current.iconSettings, () =>
            {
                // 패널이 가린 초기화면 버튼이 키보드·패드로 눌리지 않게 한다. 닫히면 포커스는 환경설정 버튼으로 돌아온다.
                EventSystem.current.SetSelectedGameObject(null);
                _panel = SettingsPanel.Create(canvas,
                    () => EventSystem.current.SetSelectedGameObject(settings.gameObject));
            });
            AddButton(canvas, 4, "Quit", "title.quit", UiTheme.Current.iconQuit, AppQuit.Quit);
        }

        // 선택된 것이 없을 때 방향 입력이 오면 첫 버튼부터 선택한다. 시작부터 선택해 두면 첫 버튼이 채워져 보인다.
        void LateUpdate()
        {
            if (_panel != null || !UiNav.MoveWithoutSelection(_module)) return;
            var first = GetComponentsInChildren<Button>().FirstOrDefault(b => b.IsInteractable());
            if (first != null) EventSystem.current.SetSelectedGameObject(first.gameObject);
        }

        static PillButton AddButton(Transform parent, int index, string id, string key, Sprite icon,
            UnityEngine.Events.UnityAction onClick)
        {
            var theme = UiTheme.Current;
            var step = theme.titleButtonSize.y + theme.titleButtonGap;
            var button = UiKit.NewPillButton(parent, id, "", icon,
                new Vector2(theme.titleMenuX, 2f * step - index * step), theme.titleButtonSize, theme.fontSize);
            button.GetComponentInChildren<LocText>().Key = key;
            if (onClick != null) button.onClick.AddListener(onClick);
            return button;
        }
    }
}
