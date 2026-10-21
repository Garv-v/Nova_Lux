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

            AddButton(canvas.transform, 0, "NewGame", "새로 시작", true, () => SceneManager.LoadScene(SceneNames.Hallway));
            AddButton(canvas.transform, 1, "Continue", "이어하기", false, null);
            AddButton(canvas.transform, 2, "Gallery", "갤러리", true, null);
            AddButton(canvas.transform, 3, "Settings", "환경설정", true, () => SettingsPanel.Create(canvas.transform));
            AddButton(canvas.transform, 4, "Quit", "종료", true, AppQuit.Quit);
        }

        static void AddButton(Transform parent, int index, string id, string label, bool interactable,
            UnityEngine.Events.UnityAction onClick)
        {
            var theme = UiTheme.Current;
            var step = theme.titleButtonSize.y + theme.titleButtonGap;
            var button = UiKit.NewButton(parent, id, label, new Vector2(0f, 2f * step - index * step),
                theme.titleButtonSize, theme.fontSize);
            button.interactable = interactable;
            if (onClick != null) button.onClick.AddListener(onClick);
        }
    }
}
