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
    /// This command inserts a newline and indents the new blank line, leaving caret at the end of the indentation.  
    /// Note, the indentation is really inserted into the buffer, no virtual whitespace where the display shows you something that’s not really in the buffer.  
    /// The indentation is language context dependent (smart).
    /// 
    /// Keys: Ctrl+J
    /// </summary>
    [EmacsCommand(EmacsCommandID.BreakLineIndent, CanBeRepeated = true, UndoName="Indent")]
    internal class BreakLineIndentCommand : EmacsCommand
    {
        internal override void Execute(EmacsCommandContext context)
        {
            context.CommandRouter.ExecuteDTECommand(VsCommands.BreakLineCommandName);
        }

        internal override void ExecuteInverse(EmacsCommandContext context)
        {
            // do nothing
        }
    }
}
