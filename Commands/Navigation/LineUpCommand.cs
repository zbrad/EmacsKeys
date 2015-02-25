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
    /// This command goes up one line, placing the caret at the same relative char position from the start of the line.  
    /// If the line is shorter, then goes to the EOL.  With a prefix arg, it goes up that many lines.  
    /// If the prefix arg is negative, this command goes down that many lines.
    /// 
    /// Keys: Ctrl-P | Up Arrow
    /// </summary>
    [EmacsCommand(EmacsCommandID.LineUp, CanBeRepeated = true, InverseCommand = EmacsCommandID.LineDown)]
    internal class LineUpCommand : EmacsCommand
    {
        internal override void Execute(EmacsCommandContext context)
        {
            context.EditorOperations.MoveLineUp();
        }
    }
}
