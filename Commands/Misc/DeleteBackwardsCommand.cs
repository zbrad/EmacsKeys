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
using Microsoft.VisualStudio.Text;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation.Commands
{
    /// <summary>
    /// This command deletes one character before the caret, but if that character is a tab, 
    /// then it replaces the tab with the number of spaces it represented minus one.  
    /// With a prefix arg, delete that many characters backwards (handling tabs appropriately), 
    /// and if the arg is negative, delete characters forward (with no special tab handling).
    /// 
    /// Keys: Bkspace
    /// </summary>
    [EmacsCommand(VSConstants.VSStd2KCmdID.BACKSPACE, CanBeRepeated = true, UndoName="Delete")]
    internal class DeleteBackwardsCommand : EmacsCommand
    {
        internal override void Execute(EmacsCommandContext context)
        {
            var caretPosition = context.TextView.GetCaretPosition().Position;

            if (caretPosition > 0)
            {
                if (context.TextBuffer.CurrentSnapshot.GetText(context.TextView.GetCaretPosition() - 1, 1) == "\t")
                {
                    context.TextView.Selection.Select(new Text.SnapshotSpan(context.TextView.TextSnapshot, new Span(caretPosition - 1, 1)), false);
                    context.EditorOperations.ConvertTabsToSpaces();
                    context.MarkSession.Deactivate();
                    context.EditorOperations.Backspace();
                }
                else
                {
                    context.EditorOperations.Backspace();
                }
            }
        }

        internal override void ExecuteInverse(EmacsCommandContext context)
        {
            var caretPosition = context.TextView.GetCaretPosition().Position;

            if (caretPosition < context.TextBuffer.CurrentSnapshot.Length)
            {
                context.EditorOperations.Delete();
            }
        }
    }
}