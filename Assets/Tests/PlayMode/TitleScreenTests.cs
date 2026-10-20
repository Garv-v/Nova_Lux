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

            Assert.AreEqual(1, QuitCalls);
        }
    }
}
