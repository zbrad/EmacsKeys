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
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation.Commands
{
    /// <summary>
    /// This command move the window farther through the buffer or moves more lines of text up into view.  
    /// The last two lines in the display are shown at the top of the window.  
    /// If the caret location is visible after the scrolling operation, it stays where it is; otherwise, it moves to the physical start of the first line displayed.  
    /// With a prefix arg, this command shows that many more lines at the bottom of the window, and if the arg is negative, then it shows that many lines at the top of the window.
    /// The scrolling behavior is based on display lines, not physical lines, so if the buffer were all one line, you could still scroll through it.
    ///
    /// Keys: PgDn | Ctrl+V
    /// </summary>
    [EmacsCommand(EmacsCommandID.ScrollPageDown, CanBeRepeated = true, InverseCommand = EmacsCommandID.ScrollPageUp)]
    internal class ScrollPageDownCommand : EmacsCommand
    {
        internal override void Execute(EmacsCommandContext context)
        {
            var currentLineDifference = context.TextBuffer.GetLineNumber(context.TextView.Caret.Position.BufferPosition) - context.TextBuffer.GetLineNumber(context.TextView.TextViewLines.FirstVisibleLine.Start);

            context.EditorOperations.ScrollPageDown();

            context.TextView.PositionCaretOnLine(currentLineDifference);
        }
    }
}
