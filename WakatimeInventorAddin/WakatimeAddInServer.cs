using Inventor;
using JetBrains.Annotations;
using System;
using System.Runtime.InteropServices;
using Attribute = System.Attribute;

namespace WakatimeInventorAddIn
{
    [Guid("91f0e9e2-cbdb-4287-8848-2674d9da29a1")]
    public class WakatimeAddInServer : ApplicationAddInServer
    {
        private WakatimeImplementation wakatime;
        public const string PanelInternalName = "Autodesk:WakaTimeAddIn:WakatimePanel";
        public const string TabInternalName = "Autodesk:WakaTimeAddIn:WakatimeTab";

        public void Activate(ApplicationAddInSite AddInSiteObject, bool FirstTime)
        {
            var inventorApp = AddInSiteObject.Application;
            wakatime = new WakatimeImplementation(inventorApp);
            
            if (FirstTime)
            {
                CreateWakaTimeRibbonPanel(GetAddInId(), inventorApp);
            }


        }

        private string GetAddInId()
        {
            var id = (GuidAttribute)Attribute.GetCustomAttribute(typeof(WakatimeAddInServer), typeof(GuidAttribute));
            return "{" + id.Value + "}";
        }

        private ButtonDefinition CreateOpenSettingsButton(string addinId, Application application)
        {
            CommandCategory slotCmdCategory = application.CommandManager.CommandCategories.Add("Slot", "Autodesk:MBTPowerToolsAddIn:OpenWakatimeSettingsCmd", addinId);

            var openViewManagerBtn = new SettingsButton(application, addinId, wakatime);
            slotCmdCategory.Add(openViewManagerBtn.ButtonDefinition);

            return openViewManagerBtn.ButtonDefinition;
        }

        private void CreateWakaTimeRibbonPanel(string addinId, [NotNull] Application application)
        {
            try
            {
                if (addinId is null) throw new ArgumentNullException(nameof(addinId));
                if (application == null) throw new ArgumentNullException(nameof(application));

                var ribbons = application.UserInterfaceManager.Ribbons;
                RibbonTab ribbonTab;
                var ribbonName = "ZeroDoc";
                var idTabWakatime = "id_Tab_Wakatime";

                if (!RibbonTabExists(application.UserInterfaceManager, ribbonName, idTabWakatime))
                {
                    ribbonTab = ribbons[ribbonName].RibbonTabs.Add("WakaTime", TabInternalName, addinId);

                }
                else
                {
                    ribbonTab = ribbons[ribbonName].RibbonTabs[idTabWakatime];

                }

                if (RibbonPanelExists(application.UserInterfaceManager, ribbonName, TabInternalName, PanelInternalName)) return;

                var panel = ribbonTab.RibbonPanels.Add("WakaTime", PanelInternalName, "{0e75678f-f093-4631-8c23-368bf96460e6}", "", false);

                panel.CommandControls.AddButton(CreateOpenSettingsButton(addinId, application));
                // check if exists
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static bool RibbonPanelExists(UserInterfaceManager userInterfaceManager, string ribbonName, string tabName, string panelName)
        {
            var ribbons = userInterfaceManager.Ribbons;
            var ribbonTabAnnotate = ribbons[ribbonName].RibbonTabs[tabName];
            foreach (RibbonPanel ribbonPanel in ribbonTabAnnotate.RibbonPanels)
            {
                if (ribbonPanel.InternalName == panelName) return true;
            }

            return false;
        }

        private static bool RibbonTabExists(UserInterfaceManager userInterfaceManager, string ribbonName, string tabName)
        {
            var ribbons = userInterfaceManager.Ribbons;
            var ribbonTabAnnotate = ribbons[ribbonName].RibbonTabs;
            foreach (RibbonTab tab in ribbons[ribbonName].RibbonTabs)
            {
                if (tab.InternalName == tabName) return true;
            }

            return false;
        }

        public void Deactivate()
        {
            wakatime.Dispose();
        }

        public void ExecuteCommand(int CommandID)
        {
            throw new NotImplementedException();
        }

        public object Automation { get; }
    }
}