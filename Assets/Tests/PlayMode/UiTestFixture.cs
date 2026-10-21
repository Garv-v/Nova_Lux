using System.Collections;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NovaLux.Tests
{
    // 초기화면 UI 테스트 공통 베이스: 가상 마우스, 종료 가로채기, 씬 로드, 버튼 클릭.
    public abstract class UiTestFixture : InputTestFixture
    {
        Mouse _mouse;
        protected Keyboard Keyboard { get; private set; }
        protected Gamepad Gamepad { get; private set; }

        string _settingsDir;

        protected int QuitCalls { get; private set; }
        protected System.Collections.Generic.List<(int w, int h, FullScreenMode mode)> ScreenCalls { get; } = new();

        public override void Setup()
        {
            base.Setup();
            _mouse = InputSystem.AddDevice<Mouse>();
            Keyboard = InputSystem.AddDevice<Keyboard>();
            Gamepad = InputSystem.AddDevice<Gamepad>();
            QuitCalls = 0;
            AppQuit.Handler = () => QuitCalls++;
            ScreenCalls.Clear();
            ScreenSettings.Handler = (w, h, m) => ScreenCalls.Add((w, h, m));
        }

        // 설정 파일을 임시 폴더로 돌린다. 폴더는 TearDown에서 지운다.
        protected void UseTempSettingsFile()
        {
            _settingsDir = Path.Combine(Path.GetTempPath(), "NovaLuxTests_" + System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_settingsDir);
            Settings.FilePath = Path.Combine(_settingsDir, "settings.json");
        }

        public override void TearDown()
        {
            if (_settingsDir != null)
            {
                Settings.FilePath = null;
                if (Directory.Exists(_settingsDir)) Directory.Delete(_settingsDir, true);
                _settingsDir = null;
            }
            AppQuit.Handler = null;
            ScreenSettings.Handler = null;
            base.TearDown();
        }

        protected static IEnumerator LoadTitle()
        {
            SceneManager.LoadScene(SceneNames.Title);
            yield return null;
            yield return null;
        }

        protected static Button FindButton(string id)
        {
            return Object.FindObjectsByType<Button>().Single(b => b.name == id);
        }

        protected static Slider FindSlider(string id)
        {
            return Object.FindObjectsByType<Slider>().Single(s => s.name == id);
        }

        protected static GameObject Selected => EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;

        protected static TMP_Text LabelOf(Button button) => button.transform.Find("Label").GetComponent<TMP_Text>();

        protected static Image Face(Button button) => (Image)button.targetGraphic;

        protected IEnumerator Click(Button button)
        {
            yield return Hover(button);
            Click(_mouse.leftButton);
            yield return null;
            yield return null;
        }

        protected IEnumerator Hover(Button button)
        {
            var rt = (RectTransform)button.transform;
            var screen = RectTransformUtility.WorldToScreenPoint(null, rt.TransformPoint(rt.rect.center));
            Set(_mouse.position, screen);
            yield return null;
            yield return null;
        }

        protected IEnumerator HoldMouse()
        {
            Press(_mouse.leftButton);
            yield return null;
        }

        protected IEnumerator ReleaseMouse()
        {
            Release(_mouse.leftButton);
            yield return null;
        }

        protected IEnumerator Tap(ButtonControl control)
        {
            Press(control);
            yield return null;
            Release(control);
            yield return null;
        }
    }
}
