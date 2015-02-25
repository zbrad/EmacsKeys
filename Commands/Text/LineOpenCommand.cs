using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Text.Formatting;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation.Commands
{
    /// <summary>
    /// This command inserts a newline after the caret, leaving the caret right where it is on the current line.
    /// 
    /// Keys: Ctrl+O
    /// </summary>
    [EmacsCommand(EmacsCommandID.LineOpen, CanBeRepeated = true, UndoName = "Enter")]
    internal class LineOpenCommand : EmacsCommand
    {
        internal override void Execute(EmacsCommandContext context)
        {
            // insert a newline right after the caret
            // context.editoroperations.insertnewline() is not working as expected for this scenario
            context.EditorOperations.InsertText(Environment.NewLine);

            // get the caret back to the location it was before the newline was inserted
            context.EditorOperations.MoveToPreviousCharacter();
        }
    }
}
