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
    /// This command twiddles the characters before and after point, moving the caret forward one character.
    /// Thereby, repeatedly invoking this command will move the character before point through the buffer with the caret.
    /// When the caret is at the end of the line, it twiddles the two characters before it; it does NOT twiddle
    /// the character before it and the newline following it.
    /// 
    /// Keys: Ctrl+T
    /// </summary>
    [EmacsCommand(EmacsCommandID.CharTranspose, UndoName="Transpose characters")]
    internal class CharTransposeCommand : EmacsCommand
    {
        internal override void Execute(EmacsCommandContext context)
        {
            context.EditorOperations.TransposeCharacter();
        }
    }
}
