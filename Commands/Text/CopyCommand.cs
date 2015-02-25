using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Text.Formatting;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation.Commands
{
    /// <summary>
    /// If the region is active, this command copies the region to the Clipboard Ring; otherwise, 
    /// it signals an error and messages in the status bar that the region is not active.  
    /// Note, this is not our Copy command since it is a no-op when there’s no active region.
    /// 
    /// Keys: Alt+W
    /// </summary>
    [EmacsCommand(VSConstants.VSStd97CmdID.Copy)]
    internal class CopyCommand : EmacsCommand
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

                context.CommandRouter.ExecuteDTECommand("Edit.Copy");
            }
            else
            {
                context.Manager.UpdateStatus("The region is not active");
            }
        }        
    }
}