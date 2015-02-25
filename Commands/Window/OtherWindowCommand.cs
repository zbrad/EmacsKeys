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
    /// This command switches focus from one pane to the other when the window is split, 
    /// ensuring point is displayed and centering the current line if the point is out of display.
    /// 
    /// Keys: Ctrl+X, O
    /// </summary>
    [EmacsCommand(EmacsCommandID.OtherWindow)]
    internal class OtherWindowCommand : EmacsCommand
    {
        internal override void Execute(EmacsCommandContext context)
        {
            DTE vs = context.Manager.ServiceProvider.GetService<DTE>();

            if (vs.ActiveDocument != null && vs.ActiveDocument.ActiveWindow != null)
            {
                var textWindow = vs.ActiveDocument.ActiveWindow.Object as TextWindow;

                if (textWindow != null && textWindow.Panes.Count > 1)
                {
                    context.CommandRouter.ExecuteDTECommand("Window.NextSplitPane");    
                }
            }                      
        }
    }
}
