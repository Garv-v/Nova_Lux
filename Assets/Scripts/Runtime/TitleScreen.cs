using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NovaLux
{
    // 임시 박스 UI. 아트와 문자열 표는 이후 티켓에서 바꾼다.
    public class TitleScreen : MonoBehaviour
    {
        InputSystemUIInputModule _module;
        SettingsPanel _panel;

        void Awake()
        {
            var canvas = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas.transform.SetParent(transform, false);
            canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvas.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            // 16:9가 아닌 화면에서도 1920x1080 영역이 레터박스 안쪽에 들어가게 한다.
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            if (FindFirstObjectByType<EventSystem>() == null)
                new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule))
                    .transform.SetParent(transform, false);
            _module = FindFirstObjectByType<InputSystemUIInputModule>();

            AddButton(canvas.transform, 0, "NewGame", "새로 시작", UiTheme.Current.iconNewGame, () => SceneManager.LoadScene(SceneNames.Hallway));
            AddButton(canvas.transform, 1, "Continue", "이어하기", UiTheme.Current.iconContinue, null);
            AddButton(canvas.transform, 2, "Gallery", "갤러리", UiTheme.Current.iconGallery, null);
            PillButton settings = null;
            settings = AddButton(canvas.transform, 3, "Settings", "환경설정", UiTheme.Current.iconSettings, () =>
            {
                // 패널이 가린 초기화면 버튼이 키보드·패드로 눌리지 않게 한다. 닫히면 포커스는 환경설정 버튼으로 돌아온다.
                EventSystem.current.SetSelectedGameObject(null);
                _panel = SettingsPanel.Create(canvas.transform,
                    () => EventSystem.current.SetSelectedGameObject(settings.gameObject));
            });
            AddButton(canvas.transform, 4, "Quit", "종료", UiTheme.Current.iconQuit, AppQuit.Quit);
        }

        // 선택된 것이 없을 때 방향 입력이 오면 첫 버튼부터 선택한다. 시작부터 선택해 두면 첫 버튼이 채워져 보인다.
        void LateUpdate()
        {
            if (_panel != null || !UiNav.MoveWithoutSelection(_module)) return;
            var first = GetComponentsInChildren<Button>().FirstOrDefault(b => b.IsInteractable());
            if (first != null) EventSystem.current.SetSelectedGameObject(first.gameObject);
        }

        static PillButton AddButton(Transform parent, int index, string id, string label, Sprite icon,
            UnityEngine.Events.UnityAction onClick)
        {
            var theme = UiTheme.Current;
            var step = theme.titleButtonSize.y + theme.titleButtonGap;
            var button = UiKit.NewPillButton(parent, id, label, icon,
                new Vector2(theme.titleMenuX, 2f * step - index * step), theme.titleButtonSize, theme.fontSize);
            if (onClick != null) button.onClick.AddListener(onClick);
            return button;
        }
    }
}
