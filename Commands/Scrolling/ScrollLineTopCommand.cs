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
    /// This command moves the current line to the top of the window, leaving the caret where it is in the line.
    /// 
    /// Keys: Shift+Alt+1
    /// </summary>
    [EmacsCommand(EmacsCommandID.ScrollLineTop)]
    internal class ScrollLineTopCommand : EmacsCommand
    {
        internal override void Execute(EmacsCommandContext context)
        {
            context.EditorOperations.ScrollLineTop();
        }
    }
}
