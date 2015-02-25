using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation.Commands
{
    /// <summary>
	/// Activates the region
    /// 
    /// Keys: ?
    /// </summary>
    [EmacsCommand(EmacsCommandID.ActivateRegion)]
    internal class ActivateRegionCommand : EmacsCommand
    {
        internal override void Execute(EmacsCommandContext context)
        {
			context.MarkSession.Activate();            
        }        
    }
}