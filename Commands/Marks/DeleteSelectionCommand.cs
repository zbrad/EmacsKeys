using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Text.Formatting;
using System.ComponentModel.Composition;
using EnvDTE;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation.Commands
{
    /// <summary>
    /// If the region is active, this command deletes the region without putting it on the Clipboard Ring or the Clipboard; 
    /// otherwise, it signals an error and messages in the status bar that the region is not active.
    /// 
    /// Keys: Ctrl+Del
    /// </summary>
    [EmacsCommand(EmacsCommandID.DeleteSelection, UndoName="Delete")]
    internal class DeleteSelectionCommand : EmacsCommand
    {
        internal override void Execute(EmacsCommandContext context)
        {
            if (!context.TextView.Selection.IsEmpty)
                context.EditorOperations.Delete();
            else
                context.Manager.UpdateStatus(Resources.OperationCannotBePerformedWithoutTextSelection);
        }
    }
}
