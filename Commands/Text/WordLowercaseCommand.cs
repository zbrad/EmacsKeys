using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation.Commands
{
    /// <summary>
    /// This command uppercases from the caret to the end of the current word or the next word if the caret is between words.
    /// The caret moves to the end of the word.
    /// With a prefix arg, it does it that many times, and if the arg is negative, it goes backwards that many times,
    /// moving the caret to the start of words.
    /// 
    /// Keys: Alt+L
    /// </summary>
    [EmacsCommand(EmacsCommandID.WordLowercase, CanBeRepeated=true, UndoName="Change case")]
    internal class WordLowercaseCommand : WordCasingCommandBase
    {
        internal override string TransformText(string text)
        {
            return text.ToLower();
        }
    }
}
