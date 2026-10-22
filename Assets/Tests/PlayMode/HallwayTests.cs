using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace NovaLux.Tests
{
    public class HallwayTests : UiTestFixture
    {
        const float Frame = 1f / 30f;

        public override void Setup()
        {
            base.Setup();
            Time.captureDeltaTime = Frame;
        }

        public override void TearDown()
        {
            Time.captureDeltaTime = 0f;
            base.TearDown();
        }

        static HallwayWalker Walker => Object.FindFirstObjectByType<HallwayWalker>();
        static Hallway Hall => Object.FindFirstObjectByType<Hallway>();

        static void PlaceAt(float x)
        {
            var p = Walker.transform.position;
            p.x = x;
            Walker.transform.position = p;
        }

        static float Center => (Hall.XMin + Hall.XMax) / 2f;

        [Test]
        public void ClampX_InMiddle_FollowsTarget()
        {
            Assert.AreEqual(3f, HallwayCamera.ClampX(3f, -27f, 27f, 8.9f), 1e-4f);
        }

        [Test]
        public void ClampX_NearEnds_StopsAtViewEdge()
        {
            Assert.AreEqual(-18.1f, HallwayCamera.ClampX(-26f, -27f, 27f, 8.9f), 1e-4f);
            Assert.AreEqual(18.1f, HallwayCamera.ClampX(26f, -27f, 27f, 8.9f), 1e-4f);
        }

        [Test]
        public void ClampX_HallwayNarrowerThanView_Centers()
        {
            Assert.AreEqual(1f, HallwayCamera.ClampX(7f, -8f, 10f, 9.5f), 1e-4f);
        }

        [UnityTest]
        public IEnumerator RightKey_MovesRight()
        {
            yield return LoadHallway();
            PlaceAt(Center);
            var x0 = Walker.transform.position.x;

            yield return Hold(Keyboard.rightArrowKey, 10);

            Assert.Greater(Walker.transform.position.x, x0 + 1f);
        }

        [UnityTest]
        public IEnumerator LeftKey_MovesLeft()
        {
            yield return LoadHallway();
            PlaceAt(Center);
            var x0 = Walker.transform.position.x;

            yield return Hold(Keyboard.leftArrowKey, 10);

            Assert.Less(Walker.transform.position.x, x0 - 1f);
        }

        [UnityTest]
        public IEnumerator DAndAKeys_Move()
        {
            yield return LoadHallway();
            PlaceAt(Center);
            var x0 = Walker.transform.position.x;

            yield return Hold(Keyboard.dKey, 10);
            var right = Walker.transform.position.x;
            yield return Hold(Keyboard.aKey, 20);

            Assert.Greater(right, x0 + 1f);
            Assert.Less(Walker.transform.position.x, x0 - 1f);
        }

        [UnityTest]
        public IEnumerator Gamepad_StickAndDpad_Move()
        {
            yield return LoadHallway();
            PlaceAt(Center);
            var x0 = Walker.transform.position.x;

            Set(Gamepad.leftStick, new Vector2(1f, 0f));
            for (var i = 0; i < 10; i++) yield return null;
            Set(Gamepad.leftStick, Vector2.zero);
            yield return null;
            var afterStick = Walker.transform.position.x;

            yield return Hold(Gamepad.dpad.left, 20);

            Assert.Greater(afterStick, x0 + 1f);
            Assert.Less(Walker.transform.position.x, afterStick - 1f);
        }

        [UnityTest]
        public IEnumerator NoInput_StaysPut()
        {
            yield return LoadHallway();
            PlaceAt(Center);
            var x0 = Walker.transform.position.x;

            for (var i = 0; i < 10; i++) yield return null;

            Assert.AreEqual(x0, Walker.transform.position.x, 1e-4f);
        }

        [UnityTest]
        public IEnumerator HoldRight_StopsAtRightEnd()
        {
            yield return LoadHallway();
            PlaceAt(Hall.XMax - 2f);

            yield return Hold(Keyboard.rightArrowKey, 90);
            var atEnd = Walker.transform.position.x;
            yield return Hold(Keyboard.rightArrowKey, 10);

            Assert.AreEqual(Hall.XMax - Walker.HalfBody, atEnd, 1e-3f);
            Assert.AreEqual(atEnd, Walker.transform.position.x, 1e-4f);
        }

        [UnityTest]
        public IEnumerator HoldLeft_StopsAtLeftEnd()
        {
            yield return LoadHallway();
            PlaceAt(Hall.XMin + 2f);

            yield return Hold(Keyboard.leftArrowKey, 90);
            var atEnd = Walker.transform.position.x;
            yield return Hold(Keyboard.leftArrowKey, 10);

            Assert.AreEqual(Hall.XMin + Walker.HalfBody, atEnd, 1e-3f);
            Assert.AreEqual(atEnd, Walker.transform.position.x, 1e-4f);
        }

        [UnityTest]
        public IEnumerator Camera_FollowsWalker_InMiddle()
        {
            yield return LoadHallway();
            PlaceAt(Center + 3f);
            yield return null;
            yield return null;

            Assert.AreEqual(Walker.transform.position.x, Camera.main.transform.position.x, 1e-3f);
        }

        [UnityTest]
        public IEnumerator Camera_NeverShowsOutsideHallway_AtBothEnds()
        {
            yield return LoadHallway();
            var cam = Camera.main;

            foreach (var x in new[] { Hall.XMin, Hall.XMax })
            {
                PlaceAt(x);
                yield return null;
                yield return null;

                var left = cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, 0f)).x;
                var right = cam.ViewportToWorldPoint(new Vector3(1f, 0.5f, 0f)).x;
                Assert.GreaterOrEqual(left, Hall.XMin - 0.01f);
                Assert.LessOrEqual(right, Hall.XMax + 0.01f);
            }
        }
    }
}
