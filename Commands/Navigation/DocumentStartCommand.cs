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
    /// This command goes to the start of the buffer and pushes a mark before doing so.  
    /// 
    /// Keys: Ctrl+Home | Home | Shift+Home | Shift+Alt+, | Ctrl+X, [
    /// </summary>
    [EmacsCommand(EmacsCommandID.DocumentStart)]
    internal class DocumentStartCommand : EmacsCommand
    {
        internal override void Execute(EmacsCommandContext context)
        {
            context.MarkSession.PushMark(activateSession: false);
            context.EditorOperations.MoveToStartOfDocument();
        }
    }
}
