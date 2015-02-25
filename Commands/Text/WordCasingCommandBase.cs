using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation.Commands
{
    /// <summary>
    /// Base class for commands that want to do casing transformations
    /// The implemented behavior is: transform text from the caret to the end of the current word or the next word if the caret is between words.
    /// The caret moves to the end of the word.
    /// With a prefix arg, it does it that many times, and if the arg is negative, it goes backwards that many times,
    /// moving the caret to the start of words.
    /// 
    /// Keys: 
    /// </summary>
    internal abstract class WordCasingCommandBase : EmacsCommand
    {
        /// <summary>
        /// This is overriden by derived class in order to perform the specific text transformation
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        internal abstract string TransformText(string text);

        internal override void Execute(EmacsCommandContext context)
        {
            var word = context.TextStructureNavigator.GetNextWord(context.TextView);

            if (word.HasValue)
            {
                var caretPosition = context.TextView.GetCaretPosition();
                var span = new Span(caretPosition, word.Value.End - caretPosition);
                var text = context.TextView.TextSnapshot.GetText(span);

                context.TextBuffer.Replace(span, TransformText(text));
            }
        }

        internal override void ExecuteInverse(EmacsCommandContext context)
        {
            var word = context.TextStructureNavigator.GetPreviousWord(context.TextView);

            if (word.HasValue)
            {
                var caretPosition = context.TextView.GetCaretPosition();
                var span = new Span(word.Value.Start, caretPosition - word.Value.Start);
                var text = context.TextView.TextSnapshot.GetText(span);

                context.TextBuffer.Replace(span, TransformText(text));

                context.EditorOperations.MoveCaret(span.Start);
            }
        }
    }
}
