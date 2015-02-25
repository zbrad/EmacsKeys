using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using System.IO;
using EnvDTE;
using System.Windows.Forms;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Text.Editor;
using System.Security.Principal;
using System.Reflection;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    //[ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid("d88ec9a6-cdda-4b04-8e46-ca81a3997a3a")]
    [ProvideAutoLoad(UIContextGuids.SolutionExists)]
    public sealed class EmacsEmulationPackage : Package
    {
        const string InstallFilename = "EmacsSetup.bat";

        protected override void Initialize()
        {
            base.Initialize();

            var componentModel = this.GetService<SComponentModel, IComponentModel>();
            var manager = componentModel.GetService<EmacsCommandsManager>();

            try
            {
                // if the emacs keybindings are not installed and the user is not running elevated
                // then we need to copy the emacs.vsk for our extension to work
                // get an ok from the user first
                if (!manager.IsEmacsVskInstalled)
                {

                    string installPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    installPath = Path.Combine(installPath, EmacsCommandsManager.EmacsVskFile);

                    var dlg = new CopyFileConfirmationDialog();
                    dlg.StartPosition = FormStartPosition.CenterScreen;
                    if (IsAdministrator || dlg.ShowDialog() == DialogResult.OK)
                    {
                        CopyVskUsingXCopy(installPath, manager);
                    }
                }
            }
            catch (Exception)
            {
            }

            manager.CheckEmacsVskSelected();
        }

        private void CopyVskUsingXCopy(string installPath, EmacsCommandsManager manager)
        {
            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = @"xcopy.exe";
            process.StartInfo.Arguments = string.Format(@"""{0}"" ""{1}""", installPath, manager.EmacsInstallationPath);
            if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major > 5)
            {
                process.StartInfo.Verb = "runas";
            }
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.Start();
        }

        bool IsAdministrator
        {
            get
            {
                WindowsIdentity wi = WindowsIdentity.GetCurrent();
                WindowsPrincipal wp = new WindowsPrincipal(wi);

                return wp.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

    }
}