using System;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using EnvDTE;
using EnvDTE80;
using MadsKristensen.EditorExtensions.BrowserLink.PixelPushing;
using MadsKristensen.EditorExtensions.BrowserLink.UnusedCss;
using MadsKristensen.EditorExtensions.Css;
using MadsKristensen.EditorExtensions.Images;
using MadsKristensen.EditorExtensions.JavaScript;
using MadsKristensen.EditorExtensions.Markdown;
using MadsKristensen.EditorExtensions.Settings;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace MadsKristensen.EditorExtensions
{
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(CommandGuids.guidEditorExtensionsPkgString)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    [InstalledProductRegistration("#110", "#112", WebEssentialsPackage.Version, IconResourceID = 400)]
    [ProvideOptionPage(typeof(CssOptions), "Web Essentials", "CSS", 101, 102, true, new[] { "Minify", "Minification", "W3C", "CSS3" })]
    [ProvideOptionPage(typeof(HtmlOptions), "Web Essentials", "HTML", 101, 111, true, new[] { "html", "angular", "xhtml" })]
    [ProvideOptionPage(typeof(GeneralOptions), "Web Essentials", "General", 101, 101, true, new[] { "ZenCoding", "Mustache", "Handlebars", "Comments", "Bundling", "Bundle" })]
    [ProvideOptionPage(typeof(CodeGenOptions), "Web Essentials", "Code Generation", 101, 210, true, new[] { "CodeGeneration", "codeGeneration" })]
    [ProvideOptionPage(typeof(MarkdownOptions), "Web Essentials", "Markdown", 101, 109, true, new[] { "markdown", "Markdown", "md" })]
    [ProvideOptionPage(typeof(JavaScriptOptions), "Web Essentials", "JavaScript", 101, 107, true, new[] { "JScript", "JS", "Minify", "Minification", "EcmaScript" })]
    [ProvideOptionPage(typeof(BrowserLinkOptions), "Web Essentials", "Browser Link", 101, 108, true, new[] { "HTML menu", "BrowserLink" })]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling"), PackageRegistration(UseManagedResourcesOnly = true)]
    public sealed class WebEssentialsPackage : Package
    {
        public const string Version = "0.5";

        private static DTE2 _dte;
        private static IVsRegisterPriorityCommandTarget _pct;
        private OleMenuCommand _topMenu;

        internal static DTE2 DTE
        {
            get
            {
                if (_dte == null)
                    _dte = ServiceProvider.GlobalProvider.GetService(typeof(DTE)) as DTE2;

                return _dte;
            }
        }
        internal static IVsRegisterPriorityCommandTarget PriorityCommandTarget
        {
            get
            {
                if (_pct == null)
                    _pct = ServiceProvider.GlobalProvider.GetService(typeof(SVsRegisterPriorityCommandTarget)) as IVsRegisterPriorityCommandTarget;

                return _pct;
            }
        }
        public static WebEssentialsPackage Instance { get; private set; }

        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        protected async override void Initialize()
        {
            base.Initialize();

            Instance = this;

            SettingsStore.Load();

            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;

            if (null != mcs)
            {
                TransformMenu transform = new TransformMenu(DTE, mcs);
                DiffMenu diffMenu = new DiffMenu(mcs);
                ProjectSettingsMenu projectSettingsMenu = new ProjectSettingsMenu(DTE, mcs);
                SolutionColorsMenu solutionColorsMenu = new SolutionColorsMenu(mcs);
                MarkdownMenu markdownMenu = new MarkdownMenu(DTE, mcs);
                AddIntellisenseFileMenu intellisenseFile = new AddIntellisenseFileMenu(DTE, mcs);
                PixelPushingMenu pixelPushingMenu = new PixelPushingMenu(mcs);
                ReferenceJsMenu referenceJsMenu = new ReferenceJsMenu(mcs);
                UnminifyMenu unMinifyMenu = new UnminifyMenu(mcs);

                HandleMenuVisibility(mcs);
                referenceJsMenu.SetupCommands();
                pixelPushingMenu.SetupCommands();
                intellisenseFile.SetupCommands();
                markdownMenu.SetupCommands();
                solutionColorsMenu.SetupCommands();
                projectSettingsMenu.SetupCommands();
                diffMenu.SetupCommands();
                transform.SetupCommands();
                unMinifyMenu.SetupCommands();
            }

            // Hook up event handlers
            await Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                DTE.Events.BuildEvents.OnBuildDone += BuildEvents_OnBuildDone;
                DTE.Events.SolutionEvents.Opened += delegate { SettingsStore.Load(); ShowTopMenu(); };
                DTE.Events.SolutionEvents.AfterClosing += delegate { DTE.StatusBar.Clear(); ShowTopMenu(); };

            }), DispatcherPriority.ApplicationIdle, null);
        }

        private void BuildEvents_OnBuildDone(vsBuildScope Scope, vsBuildAction Action)
        {
            if (_dte.Solution.SolutionBuild.LastBuildInfo != 0)
            {
                string text = _dte.StatusBar.Text; // respect localization of "Build failed"

                Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                {
                    _dte.StatusBar.Text = text;
                }), DispatcherPriority.ApplicationIdle, null);

                return;
            }
        }

        public static void ExecuteCommand(string commandName, string commandArgs = "")
        {
            var command = WebEssentialsPackage.DTE.Commands.Item(commandName);

            if (!command.IsAvailable)
                return;

            try
            {
                WebEssentialsPackage.DTE.ExecuteCommand(commandName, commandArgs);
            }
            catch { }
        }

        private void HandleMenuVisibility(OleMenuCommandService mcs)
        {
            CommandID commandId = new CommandID(CommandGuids.guidCssIntellisenseCmdSet, (int)CommandId.CssIntellisenseSubMenu);
            OleMenuCommand menuCommand = new OleMenuCommand((s, e) => { }, commandId);
            menuCommand.BeforeQueryStatus += menuCommand_BeforeQueryStatus;
            mcs.AddCommand(menuCommand);

            CommandID cmdTopMenu = new CommandID(CommandGuids.guidTopMenu, (int)CommandId.TopMenu);
            _topMenu = new OleMenuCommand((s, e) => { }, cmdTopMenu);
            mcs.AddCommand(_topMenu);
        }

        private void ShowTopMenu()
        {
            _topMenu.Visible = _dte.Solution != null && !string.IsNullOrEmpty(_dte.Solution.FullName);
        }

        private readonly string[] _supported = new[] { "CSS", "LESS", "SCSS", "JAVASCRIPT", "PROJECTION", "TYPESCRIPT", "MARKDOWN" };

        void menuCommand_BeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand menu = (OleMenuCommand)sender;
            var buffer = ProjectHelpers.GetCurentTextBuffer();

            menu.Visible = buffer != null && _supported.Contains(buffer.ContentType.DisplayName.ToUpperInvariant());
        }

        public static T GetGlobalService<T>(Type type = null) where T : class
        {
            return Microsoft.VisualStudio.Shell.Package.GetGlobalService(type ?? typeof(T)) as T;
        }

        public static IComponentModel ComponentModel
        {
            get { return GetGlobalService<IComponentModel>(typeof(SComponentModel)); }
        }

        ///<summary>Opens an Undo context, and returns an IDisposable that will close the context when disposed.</summary>
        ///<remarks>Use this method in a using() block to make sure that exceptions don't break Undo.</remarks>
        public static IDisposable UndoContext(string name)
        {
            WebEssentialsPackage.DTE.UndoContext.Open(name);

            return new Disposable(DTE.UndoContext.Close);
        }
    }

    [Guid(CommandGuids.guidEditorExtensionsPkgString2)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    [ProvideAutoLoad(UIContextGuids80.NoSolution)]
    [ProvideAutoLoad(UIContextGuids80.EmptySolution)]
    public sealed class CompatibilityCheckerPackage : Package
    {
        protected async override void Initialize()
        {
            try
            {
                // TODO: Remove for RTM
                using (var key = UserRegistryRoot.CreateSubKey("HTML Editor"))
                {
                    key.SetValue("KnockoutSupportEnabled", "1");
                }
            }
            catch {
            }

            JavaScriptIntellisense.Register(UserRegistryRoot);
            await CompatibilityChecker.StartCheckingCompatibility();
        }
    }
}
