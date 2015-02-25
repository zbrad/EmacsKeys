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
    /// This command pushes a mark on the stack that represents the caret’s current location and 
    /// activates the region, which is of course empty immediately after this command.
    /// With a prefix arg of 4, this command does pop-global-mark.  
    /// With a prefix arg of 16, this command pops the top mark and throws it away with no effect on the point.
    /// 
    /// Keys: Ctrl+Space | Ctrl+Shift+2
    /// </summary>
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [EmacsCommand(EmacsCommandID.SetMark)]
    internal class SetMarkCommand : EmacsCommand
    {
        internal override void Execute(EmacsCommandContext context)
        {
            if (context.UniversalArgument == 4)
                context.MarkSession.PopMark();
            else if (context.UniversalArgument == 16)
                context.MarkSession.RemoveTopMark();
            else
                context.MarkSession.PushMark();
        }
    }
}
