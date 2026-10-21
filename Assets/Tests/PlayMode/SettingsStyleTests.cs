using NUnit.Framework;
using UnityEngine;

namespace NovaLux.Tests
{
    public class SettingsStyleTests : UiTestFixture
    {
        [Test]
        public void DefaultTheme_HasSettingsIcons()
        {
            var theme = Resources.Load<UiTheme>("UiTheme");

            Assert.IsNotNull(theme.iconBgm);
            Assert.IsNotNull(theme.iconSfx);
            Assert.IsNotNull(theme.iconVoice);
            Assert.IsNotNull(theme.iconTextSpeed);
            Assert.IsNotNull(theme.iconAuto);
        }
    }
}
