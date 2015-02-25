using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Text.Formatting;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation.Commands
{
    /// <summary>
    /// This command can do what VS’s c-/ does which is set focus to the command window control 
    /// and seed the input with the “>” prompt.
    /// 
    /// Keys: Alt+X
    /// </summary>
    [EmacsCommand(EmacsCommandID.ExtendedCommand)]
    internal class ExtendedCommand : EmacsCommand
    {
        internal override void Execute(EmacsCommandContext context)
        {
            var shell = context.Manager.ServiceProvider.GetService<SVsUIShell, IVsUIShell>();

            if (shell != null)
            {
                shell.PostExecCommand(typeof(VSConstants.VSStd97CmdID).GUID, (uint)VSConstants.VSStd97CmdID.GotoCommandLine, 0, 0);
            }
        }
    }
}
