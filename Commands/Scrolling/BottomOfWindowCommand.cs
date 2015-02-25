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
    /// This command goes to the first column of the last line displayed in the window.
    /// 
    /// Keys: Unbound
    /// </summary>
    [EmacsCommand(EmacsCommandID.BottomOfWindow)]
    internal class BottomOfWindowCommand : EmacsCommand
    {
        internal override void Execute(EmacsCommandContext context)
        {
            context.EditorOperations.MoveToBottomOfView();
        }
    }
}
