using Inventor;
using JetBrains.Annotations;
using System;
using WakaTime;
using WakatimeInventorAddIn.Forms;

namespace WakatimeInventorAddIn
{
    public class WakatimeImplementation : WakaTimeIdePlugin<Application>
    {
        private ApplicationEvents appEvents;

        public WakatimeImplementation([NotNull] Application inventorApp) : base(inventorApp)
        {
        }

        public override ILogService GetLogger()
        {
            return new LogService(MyLogManager.Instance.GetCurrentClassLogger());
        }

        public override EditorInfo GetEditorInfo()
        {
            var pluginVersion = typeof(WakatimeAddInServer).Assembly.GetName().Version;
            var inventorVersion = new Version(editorObj.SoftwareVersion.Major, editorObj.SoftwareVersion.Minor, editorObj.SoftwareVersion.BuildIdentifier);

            return new EditorInfo()
            {
                Name = editorObj.SoftwareVersion.ProductName,
                PluginKey = "inventor-wakatime",
                PluginVersion = pluginVersion,
                Version = inventorVersion
            };
        }

        public override string GetActiveSolutionPath()
        {
            return editorObj.ActiveDocument.FullDocumentName;
        }

        public override void PromptApiKey()
        {
            SettingsPopup();
        }

        public override void SettingsPopup()
        {
            try
            {
                var frm = new SettingsWindow();
                frm.ShowDialog();
            }
            catch (Exception e)
            {
                Logger.Error("error at show settings window", e);
            }
        }

        public override void Dispose(bool disposing)
        {
          
        }

        public override IDownloadProgressReporter GetReporter()
        {
            return new DownloadProgressReporter();
        }

        public override void BindEditorEvents()
        {
            appEvents = editorObj.ApplicationEvents;
            appEvents.OnDocumentChange += OnInventorDocumentChanged;
            appEvents.OnCloseDocument += OnInvenorDocumentClosed;
            appEvents.OnOpenDocument += OnInventorDocumentOpened;
            
            
            appEvents.OnActivateDocument += OnInventorDocumentActivate;
            appEvents.OnActivateView += OnActivateView;


        }

        private void OnActivateView(View viewobject, EventTimingEnum beforeorafter, NameValueMap context, out HandlingCodeEnum handlingcode)
        {
            if (beforeorafter == EventTimingEnum.kAfter)
            {
                var name = (editorObj.ActiveView as DesignViewRepresentation)?.Name;
                
                this.OnDocumentChanged(viewobject.Document.FullDocumentName+"|"+ name);
                handlingcode = HandlingCodeEnum.kEventHandled;
                return;
            }
            handlingcode = HandlingCodeEnum.kEventNotHandled;
        }

        private void OnInventorDocumentActivate(_Document documentobject, EventTimingEnum beforeorafter, NameValueMap context, out HandlingCodeEnum handlingcode)
        {
            if (beforeorafter == EventTimingEnum.kAfter)
            {
                this.OnSolutionOpened(documentobject.FullDocumentName);
                this.OnDocumentOpened(documentobject.FullDocumentName);
                handlingcode = HandlingCodeEnum.kEventHandled;
                return;
            }
            handlingcode = HandlingCodeEnum.kEventNotHandled;
        }

        private void OnInventorDocumentOpened(_Document documentobject, string fulldocumentname, EventTimingEnum beforeorafter, NameValueMap context, out HandlingCodeEnum handlingcode)
        {
            if (beforeorafter == EventTimingEnum.kAfter)
            {
                this.OnSolutionOpened(fulldocumentname);
                handlingcode = HandlingCodeEnum.kEventHandled;
                return;
            }
            handlingcode = HandlingCodeEnum.kEventNotHandled;
        }

        private void OnInvenorDocumentClosed(_Document documentobject, string fulldocumentname, EventTimingEnum beforeorafter, NameValueMap context, out HandlingCodeEnum handlingcode)
        {
            if (beforeorafter == EventTimingEnum.kAfter)
            {
                this.OnDocumentChanged(fulldocumentname);
                handlingcode = HandlingCodeEnum.kEventHandled;
                return;
            }
            handlingcode = HandlingCodeEnum.kEventNotHandled;
        }

        private void OnInventorDocumentChanged(_Document documentobject, EventTimingEnum beforeorafter, CommandTypesEnum reasonsforchange, NameValueMap context, out HandlingCodeEnum handlingcode)
        {
            if (beforeorafter == EventTimingEnum.kAfter)
            {
                this.OnDocumentChanged(documentobject.FullDocumentName);
                handlingcode = HandlingCodeEnum.kEventHandled;
                return;
            }
            handlingcode = HandlingCodeEnum.kEventNotHandled;
        }
    }
}