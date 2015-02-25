using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Text.Formatting;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation.Commands
{
    /// <summary>
    /// This command moves point to the location represented by the current mark and then pops that mark off the stack, throwing it away.
    /// 
    /// Keys: Ctrl+X, Ctrl+Space | Ctrl+X, Ctrl+Shift+2
    /// </summary>
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [EmacsCommand(EmacsCommandID.PopMark)]
    internal class PopMarkCommand : EmacsCommand
    {
        internal override void Execute(EmacsCommandContext context)
        {
            context.MarkSession.PopMark();
        }
    }
}
