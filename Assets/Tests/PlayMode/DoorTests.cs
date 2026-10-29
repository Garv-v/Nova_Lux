using System.Collections;
using System.Linq;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace NovaLux.Tests
{
    public class DoorTests : UiTestFixture
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
        static Door DoorById(string id) => Object.FindObjectsByType<Door>().Single(d => d.Id == id);
        static bool PromptShown(string id) => GameObject.Find("Prompt_" + id) != null;

        static void PlaceAt(float x)
        {
            var p = Walker.transform.position;
            p.x = x;
            Walker.transform.position = p;
        }

        static IEnumerator WaitFadeDone() =>
            WaitUntil(() => !ScreenFade.Busy, FadeFrames, "페이드 종료");

        // 문 앞에 서서 입장 키를 누르고 방이 열려 페이드가 끝날 때까지 기다린다.
        IEnumerator EnterRoom(string id)
        {
            yield return LoadHallway();
            PlaceAt(DoorById(id).X);
            yield return null;
            yield return Tap(Keyboard.upArrowKey);
            yield return WaitUntil(() => Room != null, FadeFrames, "방 열림");
            yield return WaitFadeDone();
        }

        [UnityTest]
        public IEnumerator Prompt_ShowsOnlyInFrontOfDoor()
        {
            yield return LoadHallway();
            PlaceAt(DoorById("mine").X + 5f);
            yield return null;
            yield return null;
            Assert.IsFalse(PromptShown("mine"));
            Assert.IsFalse(PromptShown("neighbor"));

            PlaceAt(DoorById("mine").X);
            yield return null;
            yield return null;
            Assert.IsTrue(PromptShown("mine"));
            Assert.IsFalse(PromptShown("neighbor"));

            PlaceAt(DoorById("neighbor").X);
            yield return null;
            yield return null;
            Assert.IsFalse(PromptShown("mine"));
            Assert.IsTrue(PromptShown("neighbor"));
        }

        [UnityTest]
        public IEnumerator Enter_FadesThenOpensRoomWithInteractSlot()
        {
            yield return LoadHallway();
            PlaceAt(DoorById("mine").X);
            yield return null;
            yield return Tap(Keyboard.upArrowKey);
            yield return null;

            Assert.Greater(ScreenFade.Alpha, 0f);
            Assert.Less(ScreenFade.Alpha, 1f);
            Assert.IsNull(Room, "검게 덮이기 전에는 방이 열리지 않는다");

            yield return WaitUntil(() => Room != null, FadeFrames, "방 열림");
            yield return WaitFadeDone();

            Assert.AreEqual("mine", Room.Door.Id);
            Assert.AreEqual(Loc.Get("room.mine"), Room.transform.Find("Title").GetComponent<TMP_Text>().text);
            Assert.AreEqual(0f, ScreenFade.Alpha, 1e-4f);

            var slot = Room.transform.Find("InteractSlot");
            Assert.IsNotNull(slot);
            Assert.IsFalse(slot.GetComponent<Graphic>().raycastTarget);
            Assert.IsNull(slot.GetComponent<Selectable>());
        }

        [UnityTest]
        public IEnumerator Enter_WalkingDuringFade_DoesNotMoveWalker()
        {
            yield return LoadHallway();
            PlaceAt(DoorById("mine").X);
            yield return null;
            yield return Tap(Keyboard.upArrowKey);
            var x0 = Walker.transform.position.x;

            yield return Hold(Keyboard.rightArrowKey, 4);

            Assert.AreEqual(x0, Walker.transform.position.x, 1e-4f);
            yield return WaitUntil(() => Room != null, FadeFrames, "방 열림");
            Assert.AreEqual("mine", Room.Door.Id);
        }

        [UnityTest]
        public IEnumerator Enter_OutsideDoor_DoesNothing()
        {
            yield return LoadHallway();
            PlaceAt(DoorById("mine").X + 5f);
            yield return null;
            yield return Tap(Keyboard.upArrowKey);
            for (var i = 0; i < 12; i++) yield return null;

            Assert.IsNull(Room);
            Assert.IsFalse(ScreenFade.Busy);
        }

        [UnityTest]
        public IEnumerator Enter_ByEOrEnter_DoesNothing()
        {
            yield return LoadHallway();
            PlaceAt(DoorById("mine").X);
            yield return null;
            yield return Tap(Keyboard.eKey);
            yield return Tap(Keyboard.enterKey);
            for (var i = 0; i < 12; i++) yield return null;

            Assert.IsNull(Room);
            Assert.IsFalse(ScreenFade.Busy);
        }

        [UnityTest]
        public IEnumerator Exit_ByEscape_DoesNothing()
        {
            yield return EnterRoom("mine");

            yield return Tap(Keyboard.escapeKey);
            for (var i = 0; i < 12; i++) yield return null;

            Assert.IsNotNull(Room);
            Assert.IsFalse(ScreenFade.Busy);
        }

        [UnityTest]
        public IEnumerator RoundTrip_Keyboard_ReturnsToSameDoor()
        {
            yield return EnterRoom("neighbor");
            var doorX = DoorById("neighbor").X;

            yield return Tap(Keyboard.upArrowKey);
            yield return WaitUntil(() => Room == null, FadeFrames, "방 닫힘");
            yield return WaitFadeDone();

            Assert.AreEqual(doorX, Walker.transform.position.x, 1e-3f);
            Assert.IsTrue(PromptShown("neighbor"));

            yield return Hold(Keyboard.rightArrowKey, 10);
            Assert.Greater(Walker.transform.position.x, doorX + 1f);
        }

        [UnityTest]
        public IEnumerator Exit_ByClickingButton()
        {
            yield return EnterRoom("mine");

            yield return Click(FindButton("Exit"));
            yield return WaitUntil(() => Room == null, FadeFrames, "방 닫힘");
            yield return WaitFadeDone();

            Assert.AreEqual(DoorById("mine").X, Walker.transform.position.x, 1e-3f);
        }

        [UnityTest]
        public IEnumerator InRoom_WalkKeysDoNotMoveWalker()
        {
            yield return EnterRoom("mine");
            var x0 = Walker.transform.position.x;

            yield return Hold(Keyboard.rightArrowKey, 20);

            Assert.AreEqual(x0, Walker.transform.position.x, 1e-4f);
        }

        [UnityTest]
        public IEnumerator MashDuringFade_OpensOnlyOneRoom()
        {
            yield return LoadHallway();
            PlaceAt(DoorById("mine").X);
            yield return null;
            for (var i = 0; i < 6; i++) yield return Tap(Keyboard.upArrowKey);
            yield return WaitUntil(() => Room != null, FadeFrames, "방 열림");
            yield return WaitFadeDone();

            Assert.AreEqual(1, Object.FindObjectsByType<RoomScreen>().Length);
        }

        [UnityTest]
        public IEnumerator Prompt_FollowsLanguage()
        {
            yield return LoadHallway();
            PlaceAt(DoorById("mine").X);
            yield return null;
            yield return null;
            Assert.AreEqual("입장", GameObject.Find("Prompt_mine").GetComponent<TMP_Text>().text);

            Loc.Set(Language.Ja);

            Assert.AreEqual("入る", GameObject.Find("Prompt_mine").GetComponent<TMP_Text>().text);
        }

        [UnityTest]
        public IEnumerator EveryDoor_HasRoomTitleKey()
        {
            yield return LoadHallway();
            var keys = Loc.Keys.ToList();
            foreach (var door in Object.FindObjectsByType<Door>())
                Assert.Contains("room." + door.Id, keys);
        }
    }
}
