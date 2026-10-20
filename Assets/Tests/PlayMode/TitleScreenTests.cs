using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace NovaLux.Tests
{
    public class TitleScreenTests : InputTestFixture
    {
        Mouse _mouse;
        int _quitCalls;

        public override void Setup()
        {
            base.Setup();
            _mouse = InputSystem.AddDevice<Mouse>();
            _quitCalls = 0;
            AppQuit.Handler = () => _quitCalls++;
        }

        public override void TearDown()
        {
            AppQuit.Handler = null;
            base.TearDown();
        }

        IEnumerator LoadTitle()
        {
            SceneManager.LoadScene(SceneNames.Title);
            yield return null;
            yield return null;
        }

        static Button FindButton(string id)
        {
            return Object.FindObjectsByType<Button>().Single(b => b.name == id);
        }

        IEnumerator Click(Button button)
        {
            var rt = (RectTransform)button.transform;
            var screen = RectTransformUtility.WorldToScreenPoint(null, rt.TransformPoint(rt.rect.center));
            Set(_mouse.position, screen);
            yield return null;
            Click(_mouse.leftButton);
            yield return null;
            yield return null;
        }

        [UnityTest]
        public IEnumerator Title_ShowsFiveButtons_ContinueDisabled()
        {
            yield return LoadTitle();

            var buttons = Object.FindObjectsByType<Button>();
            Assert.AreEqual(5, buttons.Length);
            foreach (var id in new[] { "NewGame", "Gallery", "Settings", "Quit" })
                Assert.IsTrue(FindButton(id).interactable, id);
            Assert.IsFalse(FindButton("Continue").interactable);
        }

        [UnityTest]
        public IEnumerator Click_NewGame_LoadsHallwayScene()
        {
            yield return LoadTitle();

            yield return Click(FindButton("NewGame"));
            yield return null;

            Assert.AreEqual(SceneNames.Hallway, SceneManager.GetActiveScene().name);
        }

        [UnityTest]
        public IEnumerator Click_Quit_QuitsApplication()
        {
            yield return LoadTitle();

            yield return Click(FindButton("Quit"));

            Assert.AreEqual(1, _quitCalls);
        }
    }
}
