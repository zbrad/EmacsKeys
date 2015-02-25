using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Text.Formatting;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
using EnvDTE;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation.Commands
{
    /// <summary>
    /// If this command is invoked with a prefix argument then it converts that ascii value 
    /// to a character and inserts it into the editor.  
    /// Otherwise the user is prompted in the status bar to “Insert ASCII character decimal value”.  
    /// As they type decimals that text is added to the status bar. Upon hitting enter the decimal 
    /// number is converted to the appropriate ascii character.  If the user types a number larger than 255, 
    /// it exits the quoted insert mode and inserts nothing. 
    /// 
    /// Keys: Ctrl+Q
    /// </summary>
    [EmacsCommand(EmacsCommandID.QuotedInsert)]
    internal class QuotedInsertCommand : EmacsCommand
    {
        internal override void Execute(EmacsCommandContext context)
        {
            if (context.Manager.UniversalArgument > 0 && context.Manager.UniversalArgument <= 255)
            {
                context.EditorOperations.InsertText(((char)context.Manager.UniversalArgument.Value).ToString());
            }
            else
            {
                context.Manager.UpdateStatus("Use c-u to enter the ASCII decimal value first");
            }
        }
    }
}
