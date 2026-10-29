using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NovaLux
{
    // ESC 메뉴. 계속하기·환경설정·타이틀로만 동작한다. 저장·불러오기는 세이브 구조가 생기기 전까지 비활성이다.
    // 이동 경로는 직접 이어 둔다(자동 이동에 맡기면 뒤에 가려진 방의 나가기 버튼으로 새 나간다).
    public class PausePanel : ModalPanel
    {
        PillButton _resume;
        PillButton _settings;

        public static PausePanel Create(Transform canvas, Action onClosed) => Open<PausePanel>(canvas, onClosed);

        protected override GameObject FirstSelection => _resume.gameObject;

        protected override void Awake()
        {
            base.Awake();
            var theme = UiTheme.Current;
            UiKit.NewCard(transform, "Card", new Vector2(0f, theme.cardOffsetY), theme.cardSize);

            _resume = AddButton(0, "Resume", "pause.resume");
            _resume.onClick.AddListener(Close);
            AddButton(1, "Save", "pause.save").interactable = false;
            AddButton(2, "Load", "pause.load").interactable = false;
            _settings = AddButton(3, "Settings", "pause.settings");
            _settings.onClick.AddListener(OpenSettings);
            var title = AddButton(4, "Title", "pause.title");
            title.onClick.AddListener(() => SceneManager.LoadScene(SceneNames.Title));

            LinkNavigation(new List<PillButton> { _resume, _settings, title });
        }

        PillButton AddButton(int index, string id, string key)
        {
            var theme = UiTheme.Current;
            var step = theme.pauseButtonSize.y + theme.pauseButtonGap;
            var y = theme.cardOffsetY + 2f * step - index * step;
            var button = UiKit.NewChipButton(transform, id, "", new Vector2(0f, y), theme.pauseButtonSize, theme.fontSize, false);
            button.GetComponentInChildren<LocText>().Key = key;
            return button;
        }

        // 환경설정이 열려 있는 동안은 이 패널을 숨겨 취소 입력이 둘을 한꺼번에 닫지 않게 한다.
        void OpenSettings()
        {
            EventSystem.current.SetSelectedGameObject(null);
            gameObject.SetActive(false);
            SettingsPanel.Create(transform.parent, () =>
            {
                gameObject.SetActive(true);
                SuppressCancelThisFrame();
                EventSystem.current.SetSelectedGameObject(_settings.gameObject);
            });
        }

        static void LinkNavigation(List<PillButton> buttons)
        {
            for (var i = 0; i < buttons.Count; i++)
            {
                buttons[i].navigation = new Navigation
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnUp = i > 0 ? buttons[i - 1] : null,
                    selectOnDown = i < buttons.Count - 1 ? buttons[i + 1] : null,
                };
            }
        }
    }
}
