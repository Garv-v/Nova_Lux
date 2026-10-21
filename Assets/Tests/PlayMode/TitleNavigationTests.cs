using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace NovaLux.Tests
{
    public class TitleNavigationTests : UiTestFixture
    {
        [UnityTest]
        public IEnumerator Keyboard_Down_WalksAllFiveButtons_AndBackUp()
        {
            yield return LoadTitle();
            Assert.IsNull(Selected);

            yield return Tap(Keyboard.downArrowKey);
            Assert.AreSame(FindButton("NewGame").gameObject, Selected);

            yield return Tap(Keyboard.downArrowKey);
            Assert.AreSame(FindButton("Continue").gameObject, Selected);
            yield return Tap(Keyboard.downArrowKey);
            Assert.AreSame(FindButton("Gallery").gameObject, Selected);
            yield return Tap(Keyboard.downArrowKey);
            Assert.AreSame(FindButton("Settings").gameObject, Selected);
            yield return Tap(Keyboard.downArrowKey);
            Assert.AreSame(FindButton("Quit").gameObject, Selected);

            for (var i = 0; i < 4; i++) yield return Tap(Keyboard.upArrowKey);
            Assert.AreSame(FindButton("NewGame").gameObject, Selected);
        }

        [UnityTest]
        public IEnumerator Keyboard_Enter_OnQuit_Quits()
        {
            yield return LoadTitle();
            for (var i = 0; i < 5; i++) yield return Tap(Keyboard.downArrowKey);
            Assert.AreSame(FindButton("Quit").gameObject, Selected);

            yield return Tap(Keyboard.enterKey);
            yield return null;

            Assert.AreEqual(1, QuitCalls);
        }

        [UnityTest]
        public IEnumerator Keyboard_Enter_OnSettings_OpensPanel_ClearsSelection_ThenDownSelectsFirstSetting()
        {
            yield return LoadTitle();
            for (var i = 0; i < 4; i++) yield return Tap(Keyboard.downArrowKey);
            Assert.AreSame(FindButton("Settings").gameObject, Selected);

            yield return Tap(Keyboard.enterKey);
            yield return null;

            Assert.AreEqual(1, Object.FindObjectsByType<SettingsPanel>().Length);
            Assert.IsNull(Selected);
            yield return Tap(Keyboard.downArrowKey);
            Assert.AreSame(FindSlider("Bgm").gameObject, Selected);
        }

        [UnityTest]
        public IEnumerator Gamepad_South_OnQuit_Quits()
        {
            yield return LoadTitle();
            for (var i = 0; i < 5; i++) yield return Tap(Gamepad.dpad.down);
            Assert.AreSame(FindButton("Quit").gameObject, Selected);

            yield return Tap(Gamepad.buttonSouth);
            yield return null;

            Assert.AreEqual(1, QuitCalls);
        }

        [UnityTest]
        public IEnumerator Gamepad_Dpad_Navigates()
        {
            yield return LoadTitle();

            yield return Tap(Gamepad.dpad.down);
            Assert.AreSame(FindButton("NewGame").gameObject, Selected);
            yield return Tap(Gamepad.dpad.down);
            Assert.AreSame(FindButton("Continue").gameObject, Selected);
        }
    }
}
