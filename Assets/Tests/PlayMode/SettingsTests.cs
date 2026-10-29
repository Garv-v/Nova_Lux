using System.Collections;
using System.IO;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace NovaLux.Tests
{
    public class SettingsTests : UiTestFixture
    {
        public override void Setup()
        {
            base.Setup();
            UseTempSettingsFile();
        }

        [Test]
        public void NoFile_UsesDefaults()
        {
            var s = Settings.Current;

            Assert.AreEqual(0.8f, s.bgm, 1e-4f);
            Assert.AreEqual(0.8f, s.sfx, 1e-4f);
            Assert.AreEqual(0.8f, s.voice, 1e-4f);
            Assert.AreEqual(40f, s.textCps, 1e-4f);
            Assert.AreEqual(1.5f, s.autoDelaySec, 1e-4f);
            Assert.IsTrue(s.fullscreen);
            Assert.AreEqual(1, s.resolution);
        }

        [Test]
        public void Reload_OldFileWithoutDisplayKeys_UsesDisplayDefaults()
        {
            File.WriteAllText(Settings.FilePath, "{\"bgm\":0.2,\"sfx\":0.8,\"voice\":0.8,\"textCps\":40,\"autoDelaySec\":1.5}");

            Settings.Reload();

            Assert.IsTrue(Settings.Current.fullscreen);
            Assert.AreEqual(1, Settings.Current.resolution);
        }

        [Test]
        public void Reload_ClampsResolutionIndex()
        {
            File.WriteAllText(Settings.FilePath, "{\"resolution\":9}");
            Settings.Reload();
            Assert.AreEqual(3, Settings.Current.resolution);

            File.WriteAllText(Settings.FilePath, "{\"resolution\":-1}");
            Settings.Reload();
            Assert.AreEqual(0, Settings.Current.resolution);
        }

        [Test]
        public void Set_DisplayChoice_SurvivesReload()
        {
            Settings.Set(s => { s.fullscreen = false; s.resolution = 3; });

            Settings.Reload();

            Assert.IsFalse(Settings.Current.fullscreen);
            Assert.AreEqual(3, Settings.Current.resolution);
        }

        [Test]
        public void Set_WritesFile_AndSurvivesReload()
        {
            Settings.Set(s => { s.bgm = 0.3f; s.textCps = 70f; });

            Settings.Reload();

            Assert.IsTrue(File.Exists(Settings.FilePath));
            Assert.AreEqual(0.3f, Settings.Current.bgm, 1e-4f);
            Assert.AreEqual(70f, Settings.Current.textCps, 1e-4f);
            Assert.AreEqual(0.8f, Settings.Current.sfx, 1e-4f);
            Assert.IsFalse(File.Exists(Settings.FilePath + ".tmp"));
        }

        [Test]
        public void Reload_ClampsOutOfRangeValues()
        {
            File.WriteAllText(Settings.FilePath, "{\"bgm\":5,\"sfx\":-1,\"voice\":0.5,\"textCps\":1000,\"autoDelaySec\":0}");

            Settings.Reload();

            var s = Settings.Current;
            Assert.AreEqual(1f, s.bgm, 1e-4f);
            Assert.AreEqual(0f, s.sfx, 1e-4f);
            Assert.AreEqual(0.5f, s.voice, 1e-4f);
            Assert.AreEqual(100f, s.textCps, 1e-4f);
            Assert.AreEqual(0.5f, s.autoDelaySec, 1e-4f);
        }

        [Test]
        public void Reload_CorruptFile_KeepsBadCopy_AndUsesDefaults()
        {
            File.WriteAllText(Settings.FilePath, "{not json");

            Settings.Reload();

            Assert.AreEqual(0.8f, Settings.Current.bgm, 1e-4f);
            Assert.IsTrue(File.Exists(Settings.FilePath + ".bad"));
            Assert.AreEqual("{not json", File.ReadAllText(Settings.FilePath + ".bad"));
        }

        [Test]
        public void Reload_MissingKeys_FallBackToDefaults()
        {
            File.WriteAllText(Settings.FilePath, "{\"bgm\":0.2}");

            Settings.Reload();

            Assert.AreEqual(0.2f, Settings.Current.bgm, 1e-4f);
            Assert.AreEqual(0.8f, Settings.Current.sfx, 1e-4f);
            Assert.AreEqual(40f, Settings.Current.textCps, 1e-4f);
            Assert.AreEqual(1.5f, Settings.Current.autoDelaySec, 1e-4f);
        }

        [UnityTest]
        public IEnumerator Panel_OpensWithFiveSliders_ClosesWithCloseButton()
        {
            yield return LoadTitle();

            yield return Click(FindButton("Settings"));

            foreach (var id in new[] { "Bgm", "Sfx", "Voice", "TextSpeed", "AutoSpeed" })
                Assert.IsNotNull(FindSlider(id), id);

            yield return Click(FindButton("Close"));

            Assert.AreEqual(0, Object.FindObjectsByType<Slider>().Length);
        }

        [UnityTest]
        public IEnumerator SliderChange_Saves_AndSurvivesSceneReloadAndReload()
        {
            yield return LoadTitle();
            yield return Click(FindButton("Settings"));
            FindSlider("Bgm").value = 0.3f;
            FindSlider("TextSpeed").value = 70f;
            FindSlider("AutoSpeed").value = 3f;

            yield return LoadTitle();
            yield return Click(FindButton("Settings"));

            Assert.AreEqual(0.3f, FindSlider("Bgm").value, 1e-4f);
            Assert.AreEqual(70f, FindSlider("TextSpeed").value, 1e-4f);
            Assert.AreEqual(3f, FindSlider("AutoSpeed").value, 1e-4f);

            Settings.Reload();

            Assert.AreEqual(0.3f, Settings.Current.bgm, 1e-4f);
            Assert.AreEqual(70f, Settings.Current.textCps, 1e-4f);
            Assert.AreEqual(3f, Settings.Current.autoDelaySec, 1e-4f);
        }

        // 끄는 동안 매 프레임 디스크에 쓰면 끊긴다. 값은 바로 반영하고, 마우스를 놓을 때 한 번 저장한다.
        [UnityTest]
        public IEnumerator SliderDrag_AppliesAtOnce_ButWritesFileOnlyWhenReleased()
        {
            yield return LoadTitle();
            yield return Click(FindButton("Settings"));
            var slider = FindSlider("Bgm");

            slider.value = 0.2f;
            slider.value = 0.4f;
            slider.value = 0.6f;

            Assert.AreEqual(0.6f, Settings.Current.bgm, 1e-4f);
            Assert.IsFalse(File.Exists(Settings.FilePath));

            ExecuteEvents.Execute(slider.gameObject, new PointerEventData(EventSystem.current),
                ExecuteEvents.pointerUpHandler);

            Assert.IsTrue(File.Exists(Settings.FilePath));
            Settings.Reload();
            Assert.AreEqual(0.6f, Settings.Current.bgm, 1e-4f);
        }

        [UnityTest]
        public IEnumerator ClosingPanel_SavesPendingSliderChange()
        {
            yield return LoadTitle();
            yield return Click(FindButton("Settings"));
            FindSlider("Sfx").value = 0.25f;
            Assert.IsFalse(File.Exists(Settings.FilePath));

            yield return Click(FindButton("Close"));

            Settings.Reload();
            Assert.AreEqual(0.25f, Settings.Current.sfx, 1e-4f);
        }

        [UnityTest]
        public IEnumerator Release_WithoutChange_DoesNotWriteFile()
        {
            yield return LoadTitle();
            yield return Click(FindButton("Settings"));
            var slider = FindSlider("Bgm");

            ExecuteEvents.Execute(slider.gameObject, new PointerEventData(EventSystem.current),
                ExecuteEvents.pointerUpHandler);

            Assert.IsFalse(File.Exists(Settings.FilePath));
        }

        [UnityTest]
        public IEnumerator DisplayModeButton_TogglesWindowed_AppliesAndSaves()
        {
            yield return LoadTitle();
            yield return Click(FindButton("Settings"));

            yield return Click(FindButton("DisplayMode"));

            Assert.AreEqual(1, ScreenCalls.Count);
            Assert.AreEqual((1920, 1080, FullScreenMode.Windowed), ScreenCalls[0]);
            Assert.AreEqual("창모드", FindButton("DisplayMode").GetComponentInChildren<TMP_Text>().text);
            Settings.Reload();
            Assert.IsFalse(Settings.Current.fullscreen);
        }

        [UnityTest]
        public IEnumerator ResolutionButtons_Apply_AndSave()
        {
            yield return LoadTitle();
            yield return Click(FindButton("Settings"));

            yield return Click(FindButton("Res2160"));

            Assert.AreEqual((3840, 2160, FullScreenMode.FullScreenWindow), ScreenCalls[ScreenCalls.Count - 1]);
            Settings.Reload();
            Assert.AreEqual(3, Settings.Current.resolution);
        }

        [UnityTest]
        public IEnumerator PanelReopen_ShowsSavedDisplayChoice()
        {
            Settings.Set(s => { s.fullscreen = false; s.resolution = 2; });
            yield return LoadTitle();
            yield return Click(FindButton("Settings"));

            Assert.AreEqual("창모드", FindButton("DisplayMode").GetComponentInChildren<TMP_Text>().text);
        }

        [UnityTest]
        public IEnumerator SliderChange_DoesNotCallScreenApply()
        {
            yield return LoadTitle();
            yield return Click(FindButton("Settings"));

            FindSlider("Bgm").value = 0.1f;

            Assert.AreEqual(0, ScreenCalls.Count);
        }
    }
}
