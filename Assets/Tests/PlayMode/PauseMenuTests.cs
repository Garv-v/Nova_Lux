using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace NovaLux.Tests
{
    public class PauseMenuTests : UiTestFixture
    {
        const float Frame = 1f / 30f;
        const int FadeFrames = 60;

        public override void Setup()
        {
            base.Setup();
            UseTempSettingsFile();
            Time.captureDeltaTime = Frame;
        }

        public override void TearDown()
        {
            Time.captureDeltaTime = 0f;
            base.TearDown();
        }

        static HallwayWalker Walker => Object.FindFirstObjectByType<HallwayWalker>();
        static RoomScreen Room => Object.FindFirstObjectByType<RoomScreen>();
        static bool PauseOpen => Object.FindObjectsByType<PausePanel>().Length == 1;
        static Door DoorById(string id) => Object.FindObjectsByType<Door>().Single(d => d.Id == id);

        static void PlaceAt(float x)
        {
            var p = Walker.transform.position;
            p.x = x;
            Walker.transform.position = p;
        }

        IEnumerator EnterRoom(string id)
        {
            yield return LoadHallway();
            PlaceAt(DoorById(id).X);
            yield return null;
            yield return Tap(Keyboard.upArrowKey);
            yield return WaitUntil(() => Room != null, FadeFrames, "방 열림");
            yield return WaitUntil(() => !ScreenFade.Busy, FadeFrames, "페이드 종료");
        }

        [UnityTest]
        public IEnumerator Escape_InHallway_OpensMenu_AndEscapeAgainCloses()
        {
            yield return LoadHallway();

            yield return Tap(Keyboard.escapeKey);
            Assert.IsTrue(PauseOpen);
            Assert.IsTrue(PauseMenu.IsOpen);

            yield return Tap(Keyboard.escapeKey);
            Assert.IsFalse(PauseOpen);
            Assert.IsFalse(PauseMenu.IsOpen);
        }

        [UnityTest]
        public IEnumerator Menu_HasFiveButtons_SaveAndLoadDisabled()
        {
            yield return LoadHallway();
            yield return Tap(Keyboard.escapeKey);

            Assert.IsTrue(FindButton("Resume").interactable);
            Assert.IsTrue(FindButton("Settings").interactable);
            Assert.IsTrue(FindButton("Title").interactable);
            Assert.IsFalse(FindButton("Save").interactable);
            Assert.IsFalse(FindButton("Load").interactable);
        }

        [UnityTest]
        public IEnumerator Keyboard_Down_SkipsDisabled_AndEnterOnResumeCloses()
        {
            yield return LoadHallway();
            yield return Tap(Keyboard.escapeKey);

            yield return Tap(Keyboard.downArrowKey);
            Assert.AreSame(FindButton("Resume").gameObject, Selected);
            yield return Tap(Keyboard.downArrowKey);
            Assert.AreSame(FindButton("Settings").gameObject, Selected);
            yield return Tap(Keyboard.downArrowKey);
            Assert.AreSame(FindButton("Title").gameObject, Selected);
            yield return Tap(Keyboard.upArrowKey);
            yield return Tap(Keyboard.upArrowKey);
            Assert.AreSame(FindButton("Resume").gameObject, Selected);

            yield return Tap(Keyboard.enterKey);
            Assert.IsFalse(PauseOpen);
        }

        [UnityTest]
        public IEnumerator Click_Resume_Closes()
        {
            yield return LoadHallway();
            yield return Tap(Keyboard.escapeKey);

            yield return Click(FindButton("Resume"));
            yield return null;

            Assert.IsFalse(PauseOpen);
        }

        [UnityTest]
        public IEnumerator Click_Title_LoadsTitleScene()
        {
            yield return LoadHallway();
            yield return Tap(Keyboard.escapeKey);

            yield return Click(FindButton("Title"));
            yield return null;
            yield return null;

            Assert.AreEqual(SceneNames.Title, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            Assert.IsFalse(PauseMenu.IsOpen);
        }

        [UnityTest]
        public IEnumerator WhileOpen_WalkingAndDoorEntryAreBlocked()
        {
            yield return LoadHallway();
            PlaceAt(DoorById("mine").X);
            yield return null;
            yield return Tap(Keyboard.escapeKey);
            var x0 = Walker.transform.position.x;

            yield return Hold(Keyboard.rightArrowKey, 6);
            yield return Tap(Keyboard.upArrowKey);
            for (var i = 0; i < 6; i++) yield return null;

            Assert.AreEqual(x0, Walker.transform.position.x, 1e-4f);
            Assert.IsNull(Room);
        }

        [UnityTest]
        public IEnumerator Settings_OpensOverMenu_EscapeClosesOnlySettings()
        {
            yield return LoadHallway();
            yield return Tap(Keyboard.escapeKey);

            yield return Click(FindButton("Settings"));
            yield return null;
            Assert.AreEqual(1, Object.FindObjectsByType<SettingsPanel>().Length);

            yield return Tap(Keyboard.escapeKey);
            Assert.AreEqual(0, Object.FindObjectsByType<SettingsPanel>().Length);
            Assert.IsTrue(PauseOpen);
            Assert.AreSame(FindButton("Settings").gameObject, Selected);
        }

        [UnityTest]
        public IEnumerator Escape_InRoom_OpensMenu_BlocksExit_ThenExitWorksAfterClose()
        {
            yield return EnterRoom("mine");

            yield return Tap(Keyboard.escapeKey);
            Assert.IsTrue(PauseOpen);
            Assert.IsNotNull(Room);

            yield return Tap(Keyboard.upArrowKey);
            for (var i = 0; i < 12; i++) yield return null;
            Assert.IsNotNull(Room);

            yield return Tap(Keyboard.escapeKey);
            Assert.IsFalse(PauseOpen);
            yield return Tap(Keyboard.upArrowKey);
            yield return WaitUntil(() => Room == null, FadeFrames, "방 닫힘");
        }
    }
}
