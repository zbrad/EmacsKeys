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
    /// This command prompts for a line number and goes to that line, placing caret at the beginning of the line and centering the 
    /// line in the window if it is not already displayed in the window.  
    /// With a prefix arg, the command goes to that line number and does not prompt.  
    /// The caret is placed at the physical beginning of the line.
    /// 
    /// Keys: Alt+G
    /// </summary>
    /// <remarks>This command uses DTE instead of just editor APIs to avoid having to implement a new GoTo UI dialog
    /// when no universal command argument exists to denote the line number.
    /// </remarks>
    [EmacsCommand(EmacsCommandID.GoToLine)]
    internal class GoToLineCommand : EmacsCommand
    {
        internal override void Execute(EmacsCommandContext context)
        {
            if (context.Manager.UniversalArgument.HasValue)
            {
                var lineNumber = context.Manager.UniversalArgument.Value - 1;

                if (lineNumber < 0)
                {
                    context.EditorOperations.MoveToStartOfDocument();
                }
                else if (lineNumber >= context.TextView.TextSnapshot.LineCount)
                {
                    context.EditorOperations.MoveToEndOfDocument();
                }
                else
                {
                    context.EditorOperations.GotoLine(lineNumber);
                }
            }
            else
            {
                // Execute the built-in VS go-to command
                context.CommandRouter.ExecuteDTECommand(VsCommands.GoToCommandName);
            }
        }
    }
}