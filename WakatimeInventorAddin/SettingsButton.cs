using System;
using Inventor;
using JetBrains.Annotations;
using WakatimeInventorAddIn.InventorButton;
using WakatimeInventorAddIn.Properties;

namespace WakatimeInventorAddIn
{
    public class SettingsButton : Button
    {
        private readonly string addinId;
        private readonly WakatimeImplementation wakatime;

        public SettingsButton([NotNull] Application application, string addinId, [NotNull] WakatimeImplementation wakatime) : base(application)
        {
            this.addinId = addinId;
            this.wakatime = wakatime ?? throw new ArgumentNullException(nameof(wakatime));
        }

        protected override ButtonDescriptionContainer GetButtonDescription()
        {
            return new IconButtonDescriptorContainer()
            {
                ButtonDisplayType = ButtonDisplayEnum.kNoTextWithIcon,
                CommandType = CommandTypesEnum.kShapeEditCmdType,
                Description = "Wakatime Settings",
                InternalName = "Wakatime Settings",
                DisplayName = "Wakatime Settings",
                ClientId = addinId,
                Tooltip = "Translations.Properties.Resources.AIOpenViewManagerButtonToolTip",
                LargeIcon = Resources.wakatime_png,
                StandardIcon = Resources.wakatime_png
            };
        }

        protected override void ButtonDefinition_OnExecute(NameValueMap context)
        {
            wakatime.SettingsPopup();
        }
    }
}