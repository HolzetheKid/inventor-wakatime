using Inventor;
using JetBrains.Annotations;
using System;
using System.Diagnostics;
using WakaTime;
using WakatimeInventorAddIn.Forms;

namespace WakatimeInventorAddIn
{
    public class WakatimeImplementation : WakaTimeIdePlugin<Application>
    {
        private ApplicationEvents appEvents;
        private UserInputEvents userEvents;
        private ModelingEvents modelingEvents;
        private SketchEvents sketchEvents;
        private RepresentationEvents represenationEvents;

        public WakatimeImplementation([NotNull] Application inventorApp) : base(inventorApp)
        {
            Logger.Debug("Start Wakatime");
            //WakaTimeConfigFile.Debug = true;
            //WakaTimeConfigFile.Proxy = null;
            //WakaTimeConfigFile.Save();
        }

        public override ILogService GetLogger()
        {
            var currentClassLogger = MyLogManager.Instance.GetCurrentClassLogger();
            LogService logService = new LogService(currentClassLogger);
            return logService;
        }

        public override EditorInfo GetEditorInfo()
        {
            var pluginVersion = typeof(WakatimeAddInServer).Assembly.GetName().Version;
            var inventorVersion = new Version(editorObj.SoftwareVersion.Major, editorObj.SoftwareVersion.Minor, editorObj.SoftwareVersion.BuildIdentifier);

            return new EditorInfo("inventor-wakatime", "inventor", pluginVersion)
            {
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
            userEvents = editorObj.CommandManager.UserInputEvents;
            modelingEvents = editorObj.ModelingEvents;
            sketchEvents = editorObj.SketchEvents;
            represenationEvents = editorObj.RepresentationEvents;

            appEvents.OnOpenDocument += OnInventorDocumentOpened;
            appEvents.OnActivateDocument += OnInventorDocumentActivate;

            modelingEvents.OnFeatureChange += OnFeatureChange;
            modelingEvents.OnNewFeature += OnFeatureChange;

            sketchEvents.OnNewSketch += OnSketchChanged;
            sketchEvents.OnSketchChange += OnSketchChanged;

            represenationEvents.OnNewDesignViewRepresentation += OnChangeDesignView;
            represenationEvents.OnActivateDesignViewRepresentation += OnChangeDesignView;
            represenationEvents.OnActivateDesignView += OnNewDesignView;
            represenationEvents.OnNewDesignView += OnNewDesignView;

            userEvents.OnSelect += OnSelected;
        }

        private void OnNewDesignView(_Document documentobject, DesignViewRepresentation representation, EventTimingEnum beforeorafter, NameValueMap context, out HandlingCodeEnum handlingcode)
        {
            if (beforeorafter == EventTimingEnum.kAfter)
            {
                var name = GetViewFullName(representation);
                Debug.WriteLine(name);
                OnDocumentChanged(name);
                handlingcode = HandlingCodeEnum.kEventHandled;
                return;
            }
            handlingcode = HandlingCodeEnum.kEventNotHandled;
        }

        private void OnChangeDesignView(_AssemblyDocument documentobject, DesignViewRepresentation representation, EventTimingEnum beforeorafter, NameValueMap context, out HandlingCodeEnum handlingcode)
        {
            if (beforeorafter == EventTimingEnum.kAfter)
            {
                var name = GetViewFullName(representation);
                Debug.WriteLine(name);
                OnDocumentChanged(name);
                handlingcode = HandlingCodeEnum.kEventHandled;
                return;
            }
            handlingcode = HandlingCodeEnum.kEventNotHandled;
        }

        private void OnSketchChanged(_Document documentobject, Sketch sketch, EventTimingEnum beforeorafter, NameValueMap context, out HandlingCodeEnum handlingcode)
        {
            if (beforeorafter == EventTimingEnum.kAfter)
            {
                var name = GetSketchFullName(sketch);
                Debug.WriteLine(name);
                OnDocumentChanged(name);
                handlingcode = HandlingCodeEnum.kEventHandled;
                return;
            }
            handlingcode = HandlingCodeEnum.kEventNotHandled;
        }

        private void OnFeatureChange(_Document documentobject, PartFeature feature, EventTimingEnum beforeorafter, NameValueMap context, out HandlingCodeEnum handlingcode)
        {
            if (beforeorafter == EventTimingEnum.kAfter)
            {
                var name = GetFeatureFullName(feature);
                Debug.WriteLine(name);
                this.OnDocumentChanged(name);
                handlingcode = HandlingCodeEnum.kEventHandled;
                return;
            }
            handlingcode = HandlingCodeEnum.kEventNotHandled;
        }

        private void OnSelected(ObjectsEnumerator justselectedentities, ref ObjectCollection moreselectedentities, SelectionDeviceEnum selectiondevice, Point modelposition, Point2d viewposition, View view)
        {
            try
            {
                if (justselectedentities.Count == 0) { return; }
                dynamic d = justselectedentities[1];
                var category = GetCategoryName(d);
                if (string.IsNullOrEmpty(category)) return;

                var name = GetName(d);
                var documentName = GetActiveSolutionPath() + $"\\{category}\\{name}";

                OnDocumentOpened(documentName);
            }
            catch (Exception e)
            {
                Logger.Error("OnSelected ", e);
            }
        }

        private string GetCategoryName(dynamic d)
        {
            try
            {
                var feature = d as PartFeature;
                if (feature != null) return "Feature";

                var modelAnnotion = d as ModelAnnotation;
                if (modelAnnotion != null) return "Annotations";

                var view = d as DesignViewRepresentation;
                if (view != null) return "Views";
            }
            catch (Exception e)
            {
                Logger.Debug(e.ToString());
            }

            return string.Empty;
        }

        private string GetName(dynamic d)
        {
            try
            {
                var name = d.Name;

                return name;
            }
            catch
            {
                return string.Empty;
            }
        }

        //private void OnActivateView(View viewobject, EventTimingEnum beforeorafter, NameValueMap context, out HandlingCodeEnum handlingcode)
        //{
        //    if (beforeorafter == EventTimingEnum.kAfter)
        //    {
        //        var name = GetViewFullName(viewobject);

        //        this.OnDocumentChanged(name);
        //        handlingcode = HandlingCodeEnum.kEventHandled;
        //        return;
        //    }
        //    handlingcode = HandlingCodeEnum.kEventNotHandled;
        //}

        private string GetViewFullName(View viewobject)
        {
            var viewName = GetName(viewobject);
            var docname = GetActiveSolutionPath();
            var name = $"{docname}\\Views\\{viewName}";
            return name;
        }

        private string GetViewFullName(DesignViewRepresentation viewobject)
        {
            var viewName = GetName(viewobject);
            var docname = GetActiveSolutionPath();
            var name = $"{docname}\\Views\\{viewName}";
            return name;
        }

        private string GetFeatureFullName(PartFeature featue)
        {
            var name = GetName(featue);
            var docname = GetActiveSolutionPath();
            var fullName = $"{docname}\\Feature\\{name}";
            return fullName;
        }

        private string GetSketchFullName(Sketch sketch)
        {
            var name = GetName(sketch);
            var docname = GetActiveSolutionPath();
            var fullname = $"{docname}\\Sketch\\{name}";
            return fullname;
        }

        private void OnInventorDocumentActivate(_Document documentobject, EventTimingEnum beforeorafter, NameValueMap context, out HandlingCodeEnum handlingcode)
        {
            if (beforeorafter == EventTimingEnum.kAfter)
            {
                this.OnSolutionOpened(documentobject.FullDocumentName);

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
    }
}