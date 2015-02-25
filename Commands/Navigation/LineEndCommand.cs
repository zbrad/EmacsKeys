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
    /// This command goes to the end of the physical line, not the display line as VS does with Word Wrap on.
    /// 
    /// Keys: Ctrl-E | End
    /// </summary>
    [EmacsCommand(EmacsCommandID.LineEnd)]
    internal class LineEndCommand : EmacsCommand
    {
        internal override void Execute(EmacsCommandContext context)
        {
            if (context.Manager.AfterSearch)
            {
                context.EditorOperations.MoveCaretToEndOfPhysicalLine(false);
            }
            else
            {
                context.EditorOperations.MoveCaretToEndOfPhysicalLine();
            }
        }
    }
}
