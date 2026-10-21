using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace NovaLux.Tests
{
    public class LetterboxTests : UiTestFixture
    {
        static void AssertRect(Rect expected, Rect actual)
        {
            Assert.AreEqual(expected.x, actual.x, 1e-4f);
            Assert.AreEqual(expected.y, actual.y, 1e-4f);
            Assert.AreEqual(expected.width, actual.width, 1e-4f);
            Assert.AreEqual(expected.height, actual.height, 1e-4f);
        }

        [Test]
        public void Fit_SixteenByNine_FillsScreen()
        {
            AssertRect(new Rect(0, 0, 1, 1), Letterbox.Fit(1920, 1080));
            AssertRect(new Rect(0, 0, 1, 1), Letterbox.Fit(1280, 720));
        }

        [Test]
        public void Fit_WiderThanSixteenByNine_PillarBoxes()
        {
            AssertRect(new Rect(0.125f, 0, 0.75f, 1), Letterbox.Fit(2560, 1080));
        }

        [Test]
        public void Fit_TallerThanSixteenByNine_LetterBoxes()
        {
            AssertRect(new Rect(0, 0.05f, 1, 0.9f), Letterbox.Fit(1920, 1200));
        }

        [UnityTest]
        public IEnumerator Layout_SetsCameraRect_AndBars()
        {
            yield return LoadTitle();

            Letterbox.Layout(2560, 1080);

            AssertRect(new Rect(0.125f, 0, 0.75f, 1), Camera.main.rect);
            var left = (RectTransform)GameObject.Find("BarLeft").transform;
            var right = (RectTransform)GameObject.Find("BarRight").transform;
            Assert.AreEqual(0.125f, left.anchorMax.x, 1e-4f);
            Assert.AreEqual(0.875f, right.anchorMin.x, 1e-4f);
        }

        [UnityTest]
        public IEnumerator Layout_SixteenByNine_BarsAreEmpty()
        {
            yield return LoadTitle();

            Letterbox.Layout(1920, 1080);

            var left = (RectTransform)GameObject.Find("BarLeft").transform;
            var top = (RectTransform)GameObject.Find("BarTop").transform;
            Assert.AreEqual(0f, left.anchorMax.x, 1e-4f);
            Assert.AreEqual(1f, top.anchorMin.y, 1e-4f);
        }
    }
}
