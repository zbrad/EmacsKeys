using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using EnvDTE;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation.Commands
{
    /// <summary>
    /// This command prompts for an integer (negative if the first character entered is hyphen), 
    /// and when the user types a non-digit character (base 10), we set the command’s prefix argument to the entered integer.  
    /// If the user enters no integers, then the value defaults to 4.  Repeatedly invoking the command, multiplies the current 
    /// input value (or the default of 4) by the previous accumulated value.  Entering only a hyphen simply defaults to -4.
    /// 
    /// Keys: Ctrl+U
    /// </summary>
    [EmacsCommand(EmacsCommandID.UniversalArgument)]
    internal class UniversalArgumentCommand : EmacsCommand
    {
        internal override void Execute(EmacsCommandContext context)
        {
            var session = UniversalArgumentSession.GetSession(context.TextView);
            
            session.Start();
        }        
    }
}