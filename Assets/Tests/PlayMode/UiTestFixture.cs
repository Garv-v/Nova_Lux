using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NovaLux.Tests
{
    // 초기화면 UI 테스트 공통 베이스: 가상 마우스, 종료 가로채기, 씬 로드, 버튼 클릭.
    public abstract class UiTestFixture : InputTestFixture
    {
        Mouse _mouse;

        protected int QuitCalls { get; private set; }

        public override void Setup()
        {
            base.Setup();
            _mouse = InputSystem.AddDevice<Mouse>();
            QuitCalls = 0;
            AppQuit.Handler = () => QuitCalls++;
        }

        public override void TearDown()
        {
            AppQuit.Handler = null;
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

        protected IEnumerator Click(Button button)
        {
            var rt = (RectTransform)button.transform;
            var screen = RectTransformUtility.WorldToScreenPoint(null, rt.TransformPoint(rt.rect.center));
            Set(_mouse.position, screen);
            yield return null;
            Click(_mouse.leftButton);
            yield return null;
            yield return null;
        }
    }
}
