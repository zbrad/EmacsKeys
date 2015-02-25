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
    /// This command splits the current window in half, centering the display of each window 
    /// around the current line and giving focus to the top pane.  
    ///
    /// Keys: Ctrl+X, 2
    /// </summary>
    [EmacsCommand(EmacsCommandID.SplitVertical)]
    internal class SplitVerticalCommand : EmacsCommand
    {
        internal override void Execute(EmacsCommandContext context)
        {
            DTE vs = context.Manager.ServiceProvider.GetService<DTE>();

            if (vs.ActiveDocument != null && vs.ActiveDocument.ActiveWindow != null)
            {
                var textWindow = vs.ActiveDocument.ActiveWindow.Object as TextWindow;

                if (textWindow != null && textWindow.Panes.Count == 1)
                {
                    context.CommandRouter.ExecuteDTECommand("Window.Split");
                }
            }                      
        }
    }
}
