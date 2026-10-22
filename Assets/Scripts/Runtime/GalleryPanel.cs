using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace NovaLux
{
    // 둥근 카드 위의 갤러리. 해금 연결 전이라 컷신 자리가 모두 잠긴 이미지로 보인다.
    // 자리는 눌러도 아무 일이 없고 선택되지 않는다. 키보드·패드로 고를 수 있는 것은 닫기뿐이다.
    public class GalleryPanel : MonoBehaviour
    {
        // 임시 배치(여주 4명 × 5구간 20자리). 기획서 13장은 갤러리 구성이 미확정이라 컷신 수와 종류가 정해지면 바꾼다.
        public const int Columns = 5;
        public const int Rows = 4;

        InputSystemUIInputModule _module;
        Action _onClosed;
        PillButton _close;
        bool _closing;

        // onClosed는 패널이 닫힌 직후 불린다(화면이 포커스를 되돌려 놓는 자리).
        public static GalleryPanel Create(Transform canvas, Action onClosed)
        {
            var root = UiKit.NewPanel(canvas, "GalleryPanel", UiTheme.Current.scrimColor);
            UiKit.Place(root.rectTransform, Vector2.zero, canvas.GetComponentInParent<CanvasScaler>().referenceResolution);
            var panel = root.gameObject.AddComponent<GalleryPanel>();
            panel._onClosed = onClosed;
            return panel;
        }

        void Awake()
        {
            _module = FindFirstObjectByType<InputSystemUIInputModule>();
            var theme = UiTheme.Current;
            UiKit.NewCard(transform, "Card", new Vector2(0f, theme.cardOffsetY), theme.cardSize);

            var title = UiKit.NewLabel(transform, "Title", Loc.Get("gallery.title"), theme.fontSize, TextAlignmentOptions.Center);
            title.GetComponent<LocText>().Key = "gallery.title";
            UiKit.Place(title.rectTransform, new Vector2(0f, theme.galleryTitleY), theme.galleryTitleSize);

            var step = theme.gallerySlotSize + Vector2.one * theme.gallerySlotGap;
            for (var row = 0; row < Rows; row++)
            for (var col = 0; col < Columns; col++)
            {
                var x = (col - (Columns - 1) / 2f) * step.x;
                var y = theme.galleryGridY + ((Rows - 1) / 2f - row) * step.y;
                UiKit.NewLockedSlot(transform, $"Slot{row * Columns + col}", new Vector2(x, y));
            }

            _close = UiKit.NewChipButton(transform, "Close", Loc.Get("gallery.close"), new Vector2(0f, theme.galleryCloseY),
                theme.closeButtonSize, theme.fontSize, false);
            _close.GetComponentInChildren<LocText>().Key = "gallery.close";
            _close.onClick.AddListener(Close);
            // 자동 이동에 맡기면 뒤에 가려진 초기화면 버튼으로 새 나간다.
            _close.navigation = new Navigation { mode = Navigation.Mode.None };
        }

        void Update()
        {
            if (_closing || _module == null || _module.cancel == null) return;
            if (_module.cancel.action.WasPressedThisFrame()) Close();
        }

        // 선택된 것이 없을 때 방향 입력이 오면 닫기를 고른다.
        void LateUpdate()
        {
            if (!_closing && UiNav.MoveWithoutSelection(_module))
                EventSystem.current.SetSelectedGameObject(_close.gameObject);
        }

        void Close()
        {
            if (_closing) return;
            _closing = true;
            Destroy(gameObject);
            _onClosed?.Invoke();
        }
    }
}
