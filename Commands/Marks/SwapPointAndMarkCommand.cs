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
    /// This command moves point to the location of the current mark and moves the current mark to the location where point was when the user invoked this command.  
    /// This command activates the region.
    /// 
    /// Keys: Ctrl+X, Ctrl+X
    /// </summary>
    [EmacsCommand(EmacsCommandID.SwapPointAndMark)]
    internal class SwapPointAndMarkCommand : EmacsCommand
    {
        internal override void Execute(EmacsCommandContext context)
        {
            context.MarkSession.SwapPointAndMark();
        }
    }
}
