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

namespace Microsoft.VisualStudio.Editor.EmacsEmulation.Commands
{
    /// <summary>
    /// This command will simply invoke the built-in QuickReplace dialog
    /// 
    /// Keys: Shift+Alt+5
    /// </summary>
    [EmacsCommand(EmacsCommandID.FindReplace)]
    internal class FindReplaceCommand : EmacsCommand
    {        
        internal override void Execute(EmacsCommandContext context)
        {
            context.CommandRouter.ExecuteDTECommand(VsCommands.ReplaceCommandName);
        }
    }
}
