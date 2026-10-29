using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.TestTools;

namespace NovaLux.Tests
{
    public class SettingsNavigationTests : UiTestFixture
    {
        static readonly string[] Order =
            { "Bgm", "Sfx", "Voice", "TextSpeed", "AutoSpeed", "DisplayMode", "Res1080", "LangKo", "Close" };

        public override void Setup()
        {
            base.Setup();
            UseTempSettingsFile();
        }

        IEnumerator OpenSettings()
        {
            yield return LoadTitle();
            yield return Click(FindButton("Settings"));
        }

        static bool PanelOpen => Object.FindObjectsByType<SettingsPanel>().Length > 0;

        static void AssertSelected(string id) => Assert.AreEqual(id, Selected != null ? Selected.name : null);

        // 첫 입력으로 Bgm을 고른 뒤, 지정한 항목까지 아래로 내려간다.
        IEnumerator WalkTo(string id, ButtonControl down)
        {
            yield return Tap(down);
            for (var i = 1; i <= System.Array.IndexOf(Order, id); i++) yield return Tap(down);
            AssertSelected(id);
        }

        [UnityTest]
        public IEnumerator Keyboard_Down_WalksEveryRow_FromFirstSliderToClose()
        {
            yield return OpenSettings();
            Assert.IsNull(Selected);

            foreach (var id in Order)
            {
                yield return Tap(Keyboard.downArrowKey);
                AssertSelected(id);
            }
        }

        [UnityTest]
        public IEnumerator Up_AtFirstSlider_StaysThere_NeverReachesTitleButtons()
        {
            yield return OpenSettings();
            yield return Tap(Keyboard.downArrowKey);

            for (var i = 0; i < 3; i++) yield return Tap(Keyboard.upArrowKey);

            AssertSelected("Bgm");
        }

        [UnityTest]
        public IEnumerator Right_And_Left_OnSlider_ChangeByKeyStep_AndSave()
        {
            yield return OpenSettings();
            yield return Tap(Keyboard.downArrowKey);

            yield return Tap(Keyboard.rightArrowKey);
            Assert.AreEqual(0.85f, Settings.Current.bgm, 1e-4f);
            Settings.Reload();
            Assert.AreEqual(0.85f, Settings.Current.bgm, 1e-4f);

            yield return Tap(Keyboard.leftArrowKey);
            yield return Tap(Keyboard.leftArrowKey);
            Assert.AreEqual(0.75f, Settings.Current.bgm, 1e-4f);
        }

        [UnityTest]
        public IEnumerator Right_OnTextSpeed_StepsByFive_AndStaysSelected()
        {
            yield return OpenSettings();
            yield return WalkTo("TextSpeed", Keyboard.downArrowKey);

            yield return Tap(Keyboard.rightArrowKey);

            Assert.AreEqual(45f, Settings.Current.textCps, 1e-4f);
            AssertSelected("TextSpeed");
        }

        [UnityTest]
        public IEnumerator ResolutionRow_LeftRight_MovesBetweenChips_AndEnterAppliesAndSaves()
        {
            yield return OpenSettings();
            yield return WalkTo("Res1080", Keyboard.downArrowKey);

            yield return Tap(Keyboard.rightArrowKey);
            yield return Tap(Keyboard.rightArrowKey);
            AssertSelected("Res2160");
            yield return Tap(Keyboard.rightArrowKey);
            AssertSelected("Res2160");

            yield return Tap(Keyboard.enterKey);

            Settings.Reload();
            Assert.AreEqual(3, Settings.Current.resolution);
            Assert.AreEqual(3840, ScreenCalls.Last().w);
            yield return Tap(Keyboard.downArrowKey);
            AssertSelected("LangKo");
            yield return Tap(Keyboard.upArrowKey);
            AssertSelected("Res2160");
        }

        [UnityTest]
        public IEnumerator Enter_OnDisplayMode_TogglesAndSaves()
        {
            yield return OpenSettings();
            yield return WalkTo("DisplayMode", Keyboard.downArrowKey);

            yield return Tap(Keyboard.enterKey);

            Settings.Reload();
            Assert.IsFalse(Settings.Current.fullscreen);
        }

        [UnityTest]
        public IEnumerator Escape_Closes_AndSelectsSettingsButton()
        {
            yield return OpenSettings();

            yield return Tap(Keyboard.escapeKey);
            yield return null;

            Assert.IsFalse(PanelOpen);
            AssertSelected("Settings");
        }

        [UnityTest]
        public IEnumerator Enter_OnClose_Closes_AndSelectsSettingsButton()
        {
            yield return OpenSettings();
            yield return WalkTo("Close", Keyboard.downArrowKey);

            yield return Tap(Keyboard.enterKey);
            yield return null;

            Assert.IsFalse(PanelOpen);
            AssertSelected("Settings");
        }

        [UnityTest]
        public IEnumerator MouseClose_Closes_AndSelectsSettingsButton()
        {
            yield return OpenSettings();

            yield return Click(FindButton("Close"));
            yield return null;

            Assert.IsFalse(PanelOpen);
            AssertSelected("Settings");
        }
    }
}
