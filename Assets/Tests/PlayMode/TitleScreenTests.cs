using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace NovaLux.Tests
{
    public class TitleScreenTests : UiTestFixture
    {
        public override void Setup()
        {
            base.Setup();
            UseTempSettingsFile();
        }

        [UnityTest]
        public IEnumerator Title_ShowsFiveInteractableButtons()
        {
            yield return LoadTitle();

            var buttons = Object.FindObjectsByType<Button>();
            Assert.AreEqual(5, buttons.Length);
            foreach (var id in new[] { "NewGame", "Continue", "Gallery", "Settings", "Quit" })
                Assert.IsTrue(FindButton(id).interactable, id);
        }

        // 이어하기는 선택·호버 색은 바뀌지만, 저장 데이터가 생기기 전까지 눌러도 아무 일이 없다.
        [UnityTest]
        public IEnumerator Click_Continue_DoesNothing()
        {
            yield return LoadTitle();

            yield return Click(FindButton("Continue"));
            yield return null;

            Assert.AreEqual(SceneNames.Title, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            Assert.AreEqual(0, Object.FindObjectsByType<SettingsPanel>().Length);
            Assert.AreEqual(0, QuitCalls);
            Assert.AreSame(FindButton("Continue").gameObject, Selected);
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

            Assert.AreEqual(1, QuitCalls);
        }
    }
}
