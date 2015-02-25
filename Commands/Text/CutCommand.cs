using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Text.Formatting;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using Microsoft.VisualStudio.Text;
using EnvDTE;
using Microsoft.VisualStudio.Text.Editor;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation.Commands
{
    /// <summary>
    /// If the region is active, this command kills the region, placing it on the Clipboard Ring; 
    /// otherwise, it signals an error and messages in the status bar that the region is not active.
    /// 
    /// Keys: Ctrl+W
    /// </summary>
    [EmacsCommand(VSConstants.VSStd97CmdID.Cut, UndoName="Delete")]
    internal class CutCommand : EmacsCommand
    {
        internal override void Execute(EmacsCommandContext context)
        {
            ITextSelection textSelection = context.TextView.Selection;

            if (!textSelection.IsEmpty)
            {
                // Don't support addition of box selection to the clipboard ring yet
                if (textSelection.Mode == TextSelectionMode.Stream)
                {
                    context.Manager.ClipboardRing.Add(textSelection.StreamSelectionSpan.GetText());
                }

                context.CommandRouter.ExecuteDTECommand("Edit.Cut");
            }
            else
            {
                context.Manager.UpdateStatus(Resources.OperationCannotBePerformedWithoutTextSelection);
            }
        }
    }
}