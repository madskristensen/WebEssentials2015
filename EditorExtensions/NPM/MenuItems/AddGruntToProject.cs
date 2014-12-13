using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MadsKristensen.EditorExtensions.Grunt
{
    internal class AddGruntToProject
    {
        private DTE2 _dte;
        private OleMenuCommandService _mcs;
        private string _folder;

        public AddGruntToProject(DTE2 dte, OleMenuCommandService mcs)
        {
            _dte = dte;
            _mcs = mcs;
        }

        public void SetupCommands()
        {
            CommandID cid = new CommandID(CommandGuids.guidEditorExtensionsCmdSet, (int)CommandId.AddGrunt);
            OleMenuCommand cmd = new OleMenuCommand((s, e) => Execute(), cid);
            cmd.BeforeQueryStatus += BeforeQueryStatus;
            _mcs.AddCommand(cmd);
        }

        void BeforeQueryStatus(object sender, System.EventArgs e)
        {
            OleMenuCommand menuCommand = sender as OleMenuCommand;

            var projects = ProjectHelpers.GetSelectedProjects();

            if (projects == null || !projects.Any())
                return;

            _folder = ProjectHelpers.GetRootFolder(projects.ElementAt(0));

            if (!Directory.Exists(_folder))
                return;

            bool bower = File.Exists(Path.Combine(_folder, "bower.json"));
            bool grunt = File.Exists(Path.Combine(_folder, "gruntfile.js"));

            if (bower && grunt)
                return;

            menuCommand.Visible = true;

            if (bower && !grunt)
                menuCommand.Text = "Grunt to Project";
            else if (grunt && !bower)
                menuCommand.Text = "Bower to Project";
        }

        private void Execute()
        {
            CopyFileToProject("gruntfile.js");
            CopyFileToProject("package.json");
            CopyFileToProject("bower.json");

            NpmInstall();
        }

        private void NpmInstall()
        {
            Logger.Log("Setting up Grunt and Bower...");
            _dte.StatusBar.Text = "Installing packages...";
            _dte.StatusBar.Animate(true, vsStatusAnimation.vsStatusAnimationSync);

            string path = Path.Combine(System.Environment.GetEnvironmentVariable("VS140COMNTOOLS"), @"..\", @"IDE\Extensions\Microsoft\Web Tools\External\npm");

            var task = System.Threading.Tasks.Task.Run(() =>
            {
                ProcessStartInfo start = new ProcessStartInfo("cmd", "/c npm install grunt grunt-bower-task --save-dev -d")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WorkingDirectory = _folder,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8,
                };

                start.EnvironmentVariables["PATH"] = path;

                var p = new System.Diagnostics.Process();
                p.StartInfo = start;
                p.EnableRaisingEvents = true;
                p.OutputDataReceived += (s, e) => { Logger.Log(e.Data); };
                p.ErrorDataReceived += (s, e) => { Logger.Log(e.Data); };
                p.Start();
                p.BeginErrorReadLine();
                p.BeginOutputReadLine();
                p.Exited += (s, e) =>
                {
                    Logger.Log("\r\nGrunt and Bower ready to use. You can use the Task Runner Explorer to control Grunt.");
                    _dte.StatusBar.Animate(false, vsStatusAnimation.vsStatusAnimationSync);
                    _dte.StatusBar.Clear();
                };
                p.WaitForExit();
            });
        }

        private void CopyFileToProject(string fileName)
        {
            string file = Path.Combine(_folder, fileName);

            if (!File.Exists(file))
            {
                string assembly = Assembly.GetExecutingAssembly().Location;
                string folder = Path.GetDirectoryName(assembly);
                string source = Path.Combine(folder, "NPM\\Resources\\", fileName);

                File.Copy(source, file, false);
                ProjectHelpers.AddFileToActiveProject(file);
            }
        }
    }
}
