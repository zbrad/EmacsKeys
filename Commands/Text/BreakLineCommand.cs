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
    /// Inserts a new line and places the caret at the start of the new line
    /// 
    /// Keys: Enter
    /// </summary>
    [EmacsCommand(EmacsCommandID.BreakLine, CanBeRepeated = true, UndoName="Enter")]
    internal class BreakLineCommand : EmacsCommand
    {
        internal override void Execute(EmacsCommandContext context)
        {
            // Save the line number before applying the break command
            var lineNumber = context.TextView.TextViewLines.IndexOf(context.TextView.Caret.ContainingTextViewLine);

            // Execute the VS break command to support the commit of intellisense session
            context.CommandRouter.ExecuteDTECommand(VsCommands.BreakLineCommandName);

            // Check if the break command has changed the caret position.
            // If the caret position has not changed it means that someone else
            // has executed the break line command. For example: comitting an
            // intellisense session.
            if (lineNumber != context.TextView.TextViewLines.IndexOf(context.TextView.Caret.ContainingTextViewLine))
            {
                // Ensure the caret is at the beginning of the inserted line
                context.EditorOperations.MoveToStartOfLine();
            }
        }

        internal override void ExecuteInverse(EmacsCommandContext context)
        {
            // do nothing
        }
    }
}