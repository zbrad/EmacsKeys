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
    /// This command goes to the end of the buffer and pushes a mark before doing so.  
    /// 
    /// Keys: Ctrl+End | End | Shift+End | Shift+Alt+. | Ctrl+X, ]
    /// </summary>
    [EmacsCommand(EmacsCommandID.DocumentEnd)]
    internal class DocumentEndCommand : EmacsCommand
    {
        internal override void Execute(EmacsCommandContext context)
        {
            context.MarkSession.PushMark(activateSession: false);
            context.EditorOperations.MoveToEndOfDocument();
        }
    }
}
