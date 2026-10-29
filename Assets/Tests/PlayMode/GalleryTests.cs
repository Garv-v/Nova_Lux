using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace NovaLux.Tests
{
    public class GalleryTests : UiTestFixture
    {
        public override void Setup()
        {
            base.Setup();
            UseTempSettingsFile();
        }

        static bool PanelOpen => Object.FindObjectsByType<GalleryPanel>().Length > 0;

        static Image[] Slots => Object.FindObjectsByType<Image>().Where(i => i.name.StartsWith("Slot")).ToArray();

        [UnityTest]
        public IEnumerator Click_Gallery_OpensPanel_WithEveryCutsceneSlotLocked()
        {
            yield return LoadTitle();
            Assert.IsTrue(FindButton("Gallery").interactable);

            yield return Click(FindButton("Gallery"));

            Assert.IsTrue(PanelOpen);
            var slots = Slots;
            Assert.AreEqual(GalleryPanel.Columns * GalleryPanel.Rows, slots.Length);
            foreach (var slot in slots)
            {
                Assert.AreEqual(UiTheme.Current.lockedSlotColor, slot.color, slot.name);
                var lockIcon = slot.transform.Find("Lock").GetComponent<Image>();
                Assert.AreEqual(UiTheme.Current.iconLocked != null, lockIcon.enabled, slot.name);
                Assert.AreEqual(UiTheme.Current.lockedIconColor, lockIcon.color, slot.name);
            }
        }

        [UnityTest]
        public IEnumerator Click_Slot_DoesNothing()
        {
            yield return LoadTitle();
            yield return Click(FindButton("Gallery"));

            Assert.IsFalse(Slots.Any(s => s.raycastTarget));
            Assert.IsTrue(PanelOpen);
            Assert.AreEqual(0, QuitCalls);
        }

        [UnityTest]
        public IEnumerator Click_Close_ReturnsToTitle_WithoutErrors_AndFocusesGallery()
        {
            yield return LoadTitle();
            yield return Click(FindButton("Gallery"));

            yield return Click(FindButton("Close"));

            Assert.IsFalse(PanelOpen);
            Assert.AreEqual(SceneNames.Title, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            Assert.AreSame(FindButton("Gallery").gameObject, Selected);
        }

        [UnityTest]
        public IEnumerator Keyboard_Enter_OnGallery_Opens_DownSelectsClose_EscapeCloses()
        {
            yield return LoadTitle();
            for (var i = 0; i < 3; i++) yield return Tap(Keyboard.downArrowKey);
            Assert.AreSame(FindButton("Gallery").gameObject, Selected);

            yield return Tap(Keyboard.enterKey);
            Assert.IsTrue(PanelOpen);
            Assert.IsNull(Selected);

            yield return Tap(Keyboard.downArrowKey);
            Assert.AreSame(FindButton("Close").gameObject, Selected);
            yield return Tap(Keyboard.downArrowKey);
            Assert.AreSame(FindButton("Close").gameObject, Selected);

            yield return Tap(Keyboard.escapeKey);
            Assert.IsFalse(PanelOpen);
            Assert.AreSame(FindButton("Gallery").gameObject, Selected);
        }

        [UnityTest]
        public IEnumerator Gamepad_South_OnClose_Closes()
        {
            yield return LoadTitle();
            for (var i = 0; i < 3; i++) yield return Tap(Gamepad.dpad.down);
            yield return Tap(Gamepad.buttonSouth);
            Assert.IsTrue(PanelOpen);

            yield return Tap(Gamepad.dpad.down);
            yield return Tap(Gamepad.buttonSouth);

            Assert.IsFalse(PanelOpen);
        }
    }
}
