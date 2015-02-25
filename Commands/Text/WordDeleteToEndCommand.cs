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

namespace Microsoft.VisualStudio.Editor.EmacsEmulation.Commands
{
    /// <summary>
    /// This command kills from point to location forward-word would place the caret, including the prefix arg handling of forward-word.
    /// 
    /// Keys: Alt+D
    /// </summary>
    [EmacsCommand(EmacsCommandID.WordDeleteToEnd, IsKillCommand = true, InverseCommand = EmacsCommandID.WordDeleteToStart, UndoName="Cut")]
    internal class WordDeleteToEndCommand : EmacsCommand
    {
        internal override void Execute(EmacsCommandContext context)
        {
            SnapshotSpan? word = null;
            for (var counter = context.Manager.GetUniversalArgumentOrDefault(1); counter > 0; counter--)
            {
                if (word.HasValue)
                    word = context.TextStructureNavigator.GetNextWord(word.Value.End);
                else
                    word = context.TextStructureNavigator.GetNextWord(context.TextView);
            }

            if (word.HasValue)
            {
                var caretPosition = context.TextView.GetCaretPosition();

                context.EditorOperations.Delete(caretPosition, word.Value.End - caretPosition);
            }
        }
    }
}