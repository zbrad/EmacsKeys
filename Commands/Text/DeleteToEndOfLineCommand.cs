using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Text.Formatting;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation.Commands
{
    /// <summary>
    /// This command kills from point to the end of the physical line.  If invoked such that the line appears to be blank after point, 
    /// it kills to and including the newline break.  
    /// 
    /// Keys: Ctrl+K
    /// </summary>
    [EmacsCommand(EmacsCommandID.DeleteToEndOfLine, CanBeRepeated=false, IsKillCommand=true, UndoName="Cut")]
    internal class DeleteToEndOfLineCommand : EmacsCommand
    {
        internal override void Execute(EmacsCommandContext context)
        {
            // we can't use the repeating support because of the special behavior of UniversalArgument=0
            if (context.Manager.UniversalArgument == 0)
            {
                ITextCaret caret = context.TextView.Caret;
                int caretPosition = caret.Position.BufferPosition.Position;
                ITextViewLine caretViewLine = caret.ContainingTextViewLine;
                int endOfLine = caretViewLine.End.Position;
                int startOfNextLine = caretViewLine.EndIncludingLineBreak.Position;

                if (caretPosition == endOfLine)
                {
                    context.EditorOperations.Delete(caretPosition, startOfNextLine - caretPosition);
                }
                else
                {
                    // does the line contain whitespaces from caret till the end?
                    for (int whitespaceChecker = caretPosition; ; ++whitespaceChecker)
                    {
                        if (whitespaceChecker <= endOfLine)
                        {
                            if (char.IsWhiteSpace(context.TextView.TextSnapshot[whitespaceChecker]))
                            {
                                continue;
                            }
                            else
                            {
                                context.EditorOperations.DeleteToEndOfPhysicalLine();
                                break;
                            }
                        }
                        else
                        {
                            // reached end of line and every character was a whitespace
                            context.EditorOperations.Delete(caretPosition, startOfNextLine - caretPosition);
                        }
                    }
                }
            }
            else if (!context.UniversalArgument.HasValue || context.UniversalArgument > 0)
            {
                int count = context.Manager.GetUniversalArgumentOrDefault(1);
                if (count == 1)
                {
                    context.EditorOperations.DeleteToEndOfPhysicalLine();
                }
                else while (count-- > 0)
                {
                    int caretPosition = context.TextView.Caret.Position.BufferPosition.Position;
                    int nextLineStart = context.TextView.Caret.ContainingTextViewLine.EndIncludingLineBreak.Position;

                    context.EditorOperations.Delete(caretPosition, nextLineStart - caretPosition);
                }
            }
        }
    }
}