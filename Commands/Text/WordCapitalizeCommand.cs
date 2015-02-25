using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Text.Formatting;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation.Commands
{
    /// <summary>
    /// This command capitalizes the character after point if it is a word constituent character, 
    /// or the first character of the next word otherwise.  The caret moves to the end of the word.  
    /// With a prefix arg, it does it that many times, and if the arg is negative, it goes backwards that many times.  
    /// When it goes backwards, it still capitalizes the beginning of the word and moves the caret to be 
    /// before the capitalized start of the word.
    /// 
    /// Keys: Alt+C
    /// </summary>
    [EmacsCommand(EmacsCommandID.WordCapitalize, CanBeRepeated=true, UndoName="Change case")]
    internal class WordCapitalizeCommand : EmacsCommand
    {
        internal override void Execute(EmacsCommandContext context)
        {
            var word = context.TextStructureNavigator.GetNextWord(context.TextView);

            if(word.HasValue)
            {
                if (context.TextView.GetCaretPosition().Position < word.Value.Start)
                    context.EditorOperations.MoveCaret(word.Value.Start);

                context.EditorOperations.MakeUppercase();
                for (int position = context.TextView.GetCaretPosition().Position; position < word.Value.End; position++)
                {
                    context.EditorOperations.MakeLowercase();
                }
            }            
        }

        internal override void ExecuteInverse(EmacsCommandContext context)
        {
            var word = context.TextStructureNavigator.GetPreviousWord(context.TextView);

            if (word.HasValue)
            {
                if (context.TextView.GetCaretPosition().Position > word.Value.End)
                    context.EditorOperations.MoveCaret(word.Value.End);

                var wordStart = word.Value.Start.Position;

                for (int position = context.TextView.GetCaretPosition().Position - 1; position > wordStart; position--)
                {
                    context.EditorOperations.MoveCaret(position);
                    context.EditorOperations.MakeLowercase();
                }

                context.EditorOperations.MoveCaret(wordStart);
                context.EditorOperations.MakeUppercase();
                context.EditorOperations.MoveCaret(wordStart);
            }
        }
    }
}