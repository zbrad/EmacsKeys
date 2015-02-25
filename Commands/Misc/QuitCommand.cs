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
    /// This command aborts any executing command or code and throws to the top-level command loop.  
    ///
    /// Keys: Ctrl+G
    /// </summary>
    [EmacsCommand(EmacsCommandID.Quit)]
    internal class QuitCommand : EmacsCommand
    {
        internal override void Execute(EmacsCommandContext context)
        {
            context.Manager.ClearStatus();
            // Other commands may listen the execution of the quit command and cancel their execution
        }
    }
}
