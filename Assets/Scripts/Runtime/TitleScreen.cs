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
        const float ButtonWidth = 480f;
        const float ButtonHeight = 90f;
        const float ButtonGap = 24f;

        void Awake()
        {
            var canvas = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas.transform.SetParent(transform, false);
            canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvas.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            if (FindFirstObjectByType<EventSystem>() == null)
                new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule))
                    .transform.SetParent(transform, false);

            AddButton(canvas.transform, 0, "NewGame", "새로 시작", true, () => SceneManager.LoadScene(SceneNames.Hallway));
            AddButton(canvas.transform, 1, "Continue", "이어하기", false, null);
            AddButton(canvas.transform, 2, "Gallery", "갤러리", true, null);
            AddButton(canvas.transform, 3, "Settings", "환경설정", true, null);
            AddButton(canvas.transform, 4, "Quit", "종료", true, AppQuit.Quit);
        }

        static void AddButton(Transform parent, int index, string id, string label, bool interactable,
            UnityEngine.Events.UnityAction onClick)
        {
            var go = new GameObject(id, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            var rt = (RectTransform)go.transform;
            rt.sizeDelta = new Vector2(ButtonWidth, ButtonHeight);
            rt.anchoredPosition = new Vector2(0f, 2f * (ButtonHeight + ButtonGap) - index * (ButtonHeight + ButtonGap));

            go.GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.3f);
            var button = go.GetComponent<Button>();
            button.interactable = interactable;
            if (onClick != null) button.onClick.AddListener(onClick);

            var text = new GameObject("Label", typeof(RectTransform), typeof(Text)).GetComponent<Text>();
            text.transform.SetParent(go.transform, false);
            var trt = (RectTransform)text.transform;
            trt.anchorMin = Vector2.zero;
            trt.anchorMax = Vector2.one;
            trt.offsetMin = trt.offsetMax = Vector2.zero;
            text.text = label;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 40;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.raycastTarget = false;
        }
    }
}
